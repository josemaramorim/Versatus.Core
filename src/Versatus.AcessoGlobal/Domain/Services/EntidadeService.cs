using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;

namespace Versatus.AcessoGlobal.Domain.Services;

public class EntidadeService : IEntidadeService
{
    private readonly IEntidadeRepository _repository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IGeradorSequencial _geradorSequencial;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<EntidadeService> _logger;
    private readonly AcessoGlobalDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public EntidadeService(
        IEntidadeRepository repository,
        IParametroRepository parametroRepository,
        IGeradorSequencial geradorSequencial,
        IContextoExecucao contexto,
        ILogger<EntidadeService> logger,
        AcessoGlobalDbContext context,
        IServiceProvider serviceProvider)
    {
        _repository = repository;
        _parametroRepository = parametroRepository;
        _geradorSequencial = geradorSequencial;
        _contexto = contexto;
        _logger = logger;
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<Entidade>> ListarUltimasAsync(int limite = 50, CancellationToken cancellationToken = default)
    {
        return await _repository.ListarEntidadesAsync(limite);
    }

    public async Task<Entidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Entidade> CriarAsync(CriarEntidadeDto dto, CancellationToken cancellationToken = default)
    {
        var tipoEnum = dto.TipoPessoa.ToUpper() == "J" || dto.TipoPessoa.ToUpper() == "JURIDICA"
            ? EntidadeTipoPessoa.Juridica
            : EntidadeTipoPessoa.Fisica;

        var entidade = new Entidade
        {
            Nome = dto.Nome,
            TipoPessoa = tipoEnum
        };

        if (tipoEnum == EntidadeTipoPessoa.Fisica)
        {
            entidade.PessoaFisica = new DadosPessoaFisica
            {
                Cpf = dto.Cpf ?? string.Empty
            };
        }
        else
        {
            entidade.PessoaJuridica = new DadosPessoaJuridica
            {
                Cnpj = dto.Cnpj ?? string.Empty,
                RazaoSocial = dto.RazaoSocial ?? dto.Nome
            };
        }

        return await CriarAsync(entidade, cancellationToken);
    }

    public async Task<Entidade> CriarAsync(Entidade entidade, CancellationToken cancellationToken = default)
    {
        // 1. Validações completas
        var validation = await ValidarEntidadeAsync(entidade, isNew: true, cancellationToken);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Errors[0].Mensagem);
        }

        // 2. Geração de Sequencial (Padrão legado: Tabela "Entidade")
        entidade.IdEntidade = await _geradorSequencial.ProximoAsync("Entidade", SequencialTipo.Geral, cancellationToken);

        // 3. Auditoria Inclusão
        entidade.IdUsuarioInclusao = _contexto.IdUsuario;
        entidade.DataInclusao = DateTime.Today;
        entidade.HoraInclusao = DateTime.Now;

        // 4. Salvar
        await _repository.AddAsync(entidade, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entidade {IdEntidade} ({Nome}) criada com sucesso.", entidade.IdEntidade, entidade.Nome);

        return entidade;
    }

    public async Task AtualizarAsync(Entidade entidade, CancellationToken cancellationToken = default)
    {
        // 1. Validações completas
        var validation = await ValidarEntidadeAsync(entidade, isNew: false, cancellationToken);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Errors[0].Mensagem);
        }

        // 2. Auditoria Alteração
        entidade.IdUsuarioAlteracao = _contexto.IdUsuario;
        entidade.DataAlteracao = DateTime.Today;
        entidade.HoraAlteracao = DateTime.Now;

        // 3. Atualizar
        await _repository.UpdateAsync(entidade, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entidade {IdEntidade} atualizada com sucesso.", entidade.IdEntidade);
    }

    private async Task<ValidationResult> ValidarEntidadeAsync(Entidade entidade, bool isNew, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();

        // 0. Validações Legadas e Obrigatórias
        var temPapel = 
            entidade.IsCliente || entidade.IsFornecedor || entidade.IsTransportadora || entidade.IsComissionado ||
            entidade.IsAgenciaBancaria || entidade.IsInstituicaoFinanceira || entidade.IsFilial || entidade.IsFuncionario ||
            entidade.IsObra || entidade.IsRepresentante || entidade.IsOutro || entidade.IsProspecto ||
            entidade.IsContador || entidade.IsAluno || entidade.IsProfessor || entidade.IsIntermediadorComercial;

        if (!temPapel)
        {
            errors.Add(new ValidationError("IsCliente", "Pelo menos um tipo de entidade (papel) deve ser selecionado."));
        }

        if (entidade.IsFilial)
        {
            if (entidade.TipoPessoa == EntidadeTipoPessoa.Fisica && (entidade.PessoaFisica == null || entidade.PessoaFisica.FisicaTipoJuridica != true))
            {
                errors.Add(new ValidationError("FisicaTipoJuridica", "Para entidade do tipo 'Filial' definida como pessoa 'Física', deve estar marcado 'Pessoa física com característica de jurídica'."));
            }
        }

        if (entidade.IsFuncionario)
        {
            if (entidade.TipoPessoa == EntidadeTipoPessoa.Juridica || (entidade.PessoaFisica != null && entidade.PessoaFisica.FisicaTipoJuridica == true))
            {
                errors.Add(new ValidationError("IsFuncionario", "Para a entidade do tipo 'Funcionário', deve ser pessoa física e não possuir característica de pessoa jurídica."));
            }
        }

        if (entidade.IsIntermediadorComercial)
        {
            if (entidade.TipoPessoa == EntidadeTipoPessoa.Fisica)
            {
                errors.Add(new ValidationError("TipoPessoa", "Para a entidade do tipo 'Intermediador', deve ser SOMENTE pessoa definida como jurídica."));
            }

            if (entidade.PessoaJuridica == null || string.IsNullOrWhiteSpace(entidade.PessoaJuridica.Cnpj))
            {
                errors.Add(new ValidationError("Cnpj", "Para a entidade do tipo 'Intermediador', deve ser informado o CNPJ."));
            }

            var idFilialLogada = _contexto.IdFilial;
            var filial = await _repository.GetByIdAsync(idFilialLogada, cancellationToken);
            if (filial != null && filial.PessoaJuridica != null && !string.IsNullOrEmpty(filial.PessoaJuridica.Cnpj) &&
                entidade.PessoaJuridica != null && !string.IsNullOrEmpty(entidade.PessoaJuridica.Cnpj))
            {
                var cleanCnpjFilial = new string(filial.PessoaJuridica.Cnpj.Where(char.IsDigit).ToArray());
                var cleanCnpjEntidade = new string(entidade.PessoaJuridica.Cnpj.Where(char.IsDigit).ToArray());
                if (cleanCnpjFilial.Equals(cleanCnpjEntidade))
                {
                    errors.Add(new ValidationError("Cnpj", "Para a entidade do tipo 'Intermediador', deve ser informado CNPJ DIFERENTE do CNPJ da filial logada."));
                }
            }
        }

        if (entidade.ContribuinteICMS == IndicadorContribuinteICMS.ContribuinteIsento)
        {
            entidade.InscricaoEstadual = "ISENTO";
        }

        if (!string.IsNullOrWhiteSpace(entidade.InscricaoEstadual) && entidade.InscricaoEstadual.ToUpper() != "ISENTO")
        {
            var cleanIE = new string(entidade.InscricaoEstadual.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(cleanIE))
            {
                errors.Add(new ValidationError("InscricaoEstadual", "Deve ser informado para inscrição estadual somente caracteres numéricos."));
            }
            if (cleanIE.Length < 2 || cleanIE.Length > 14)
            {
                errors.Add(new ValidationError("InscricaoEstadual", "Deve ser informado no mínimo 2 e no máximo 14 caracteres numéricos para inscrição estadual."));
            }
        }

        if (!string.IsNullOrWhiteSpace(entidade.InscricaoSuframa))
        {
            var cleanSuframa = new string(entidade.InscricaoSuframa.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(cleanSuframa))
            {
                errors.Add(new ValidationError("InscricaoSuframa", "Deve ser informado para inscrição SUFRAMA somente caracteres numéricos."));
            }
            if (cleanSuframa.Length != 9)
            {
                errors.Add(new ValidationError("InscricaoSuframa", "Deve ser informado 9 caracteres numéricos para inscrição SUFRAMA."));
            }
            if (cleanSuframa.StartsWith("00"))
            {
                errors.Add(new ValidationError("InscricaoSuframa", "Os dois primeiros caracteres da inscrição SUFRAMA, NÃO pode ser '00'."));
            }
            if (!ValidarSuframaModulo11(cleanSuframa))
            {
                errors.Add(new ValidationError("InscricaoSuframa", "O dígito verificador da inscrição SUFRAMA NÃO é válido."));
            }
        }

        // 1. Validação de Razão Social
        if (entidade.TipoPessoa == EntidadeTipoPessoa.Juridica && entidade.PessoaJuridica != null)
        {
            var razaoSocial = entidade.PessoaJuridica.RazaoSocial;
            if (!string.IsNullOrEmpty(razaoSocial))
            {
                var cleanRazao = string.Join(" ", razaoSocial.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                if (cleanRazao.Length < 3)
                {
                    errors.Add(new ValidationError("RazaoSocial", "A razão social deve ter no mínimo 3 caracteres válidos."));
                }
            }
        }

        // 2. Determinação de País (Verificar bypass para estrangeiros)
        var filialPaisId = await _repository.GetPaisIdPorFilialAsync(_contexto.IdFilial, cancellationToken) ?? 1058;
        int? entidadePaisId = null;

        var comercialAddress = entidade.Enderecos.FirstOrDefault(e => e.TipoEndereco == EnderecoTipo.ComercialResidencial);
        if (comercialAddress != null)
        {
            if (comercialAddress.Cidade != null)
            {
                entidadePaisId = comercialAddress.Cidade.IdPais;
            }
            else
            {
                entidadePaisId = await _repository.GetPaisIdPorCidadeAsync(comercialAddress.IdCidade, cancellationToken);
            }
        }

        // Se o país for estrangeiro (diferente da filial), ignora todas as validações de CPF/CNPJ
        if (entidadePaisId.HasValue && entidadePaisId.Value != filialPaisId)
        {
            return errors.Count > 0 ? ValidationResult.Fail(errors.ToArray()) : ValidationResult.Ok();
        }

        // 3. Validação de Obrigatoriedade (CPFCNPJOBRIGATORIO)
        var paramObrigatorio = await _parametroRepository.GetParametroValorAsync("CPFCNPJOBRIGATORIO", cancellationToken)
            ?? await _parametroRepository.GetParametroValorAsync("CpfCnpjObrigatorio", cancellationToken);
        var obrigatorioBloquear = paramObrigatorio == "BloquearSalvar";
        var obrigatorioAvisar = paramObrigatorio == "Avisar";

        var cpfCnpjInformado = false;
        string? cpf = null;
        string? cnpj = null;

        if (entidade.PessoaFisica != null)
        {
            cpf = entidade.PessoaFisica.Cpf;
            cpfCnpjInformado = !string.IsNullOrWhiteSpace(cpf);
        }
        
        if (entidade.PessoaJuridica != null)
        {
            cnpj = entidade.PessoaJuridica.Cnpj;
            cpfCnpjInformado = cpfCnpjInformado || !string.IsNullOrWhiteSpace(cnpj);
        }

        // Se nenhum foi fornecido e a obrigatoriedade está ativa
        if (!cpfCnpjInformado)
        {
            // Determinar o campo esperado com base na presença dos objetos de detalhe
            string sCampo = (entidade.PessoaJuridica != null || entidade.TipoPessoa == EntidadeTipoPessoa.Juridica) ? "CNPJ" : "CPF";
            if (obrigatorioBloquear)
            {
                errors.Add(new ValidationError(sCampo == "CNPJ" ? "Cnpj" : "Cpf", $"Deve ser informado o {sCampo}."));
            }
            else if (obrigatorioAvisar)
            {
                _logger.LogWarning("AVISO: Para a entidade é aconselhável preenchimento do {Campo}.", sCampo);
            }
            return errors.Count > 0 ? ValidationResult.Fail(errors.ToArray()) : ValidationResult.Ok();
        }

        // 4. Validação Matemática de Dígitos (ACEITACNPJCPFINVALIDO)
        var paramAceitaInvalido = await _parametroRepository.GetParametroValorAsync("ACEITACNPJCPFINVALIDO", cancellationToken)
            ?? await _parametroRepository.GetParametroValorAsync("AceitaCnpjCpfInvalido", cancellationToken);
        var aceitaInvalido = paramAceitaInvalido == "true" || paramAceitaInvalido == "1";

        if (!aceitaInvalido)
        {
            if (cpf != null)
            {
                if (!ValidarCPF(cpf))
                {
                    errors.Add(new ValidationError("Cpf", "CPF inválido."));
                }
            }
            
            if (cnpj != null)
            {
                if (!ValidarCNPJ(cnpj))
                {
                    errors.Add(new ValidationError("Cnpj", "CNPJ inválido."));
                }
            }
        }

        // 5. Validação de Unicidade (TIPOBLOQUEIOCPFCNPJDUPLICADO)
        if (isNew)
        {
            var paramDuplicado = await _parametroRepository.GetParametroValorAsync("TIPOBLOQUEIOCPFCNPJDUPLICADO", cancellationToken)
                ?? await _parametroRepository.GetParametroValorAsync("TipoBloqueioCpfCnpjDuplicado", cancellationToken);
            var bloquearDuplicado = string.IsNullOrEmpty(paramDuplicado) || paramDuplicado == "BloquearSalvar";
            var naoValidarDuplicado = paramDuplicado == "NaoValidar";

            if (!naoValidarDuplicado)
            {
                if (cpf != null)
                {
                    var existente = await _repository.GetByCpfAsync(cpf, cancellationToken);
                    if (existente != null && existente.IdEntidade != entidade.IdEntidade)
                    {
                        string msg = $"Já existe uma entidade cadastrada com o CPF {cpf}.";
                        if (bloquearDuplicado)
                        {
                            errors.Add(new ValidationError("Cpf", msg));
                        }
                        else
                        {
                            _logger.LogWarning("AVISO: {Mensagem}", msg);
                        }
                    }
                }
                
                if (cnpj != null)
                {
                    var existente = await _repository.GetByCnpjAsync(cnpj, cancellationToken);
                    if (existente != null && existente.IdEntidade != entidade.IdEntidade)
                    {
                        string msg = $"Já existe uma entidade cadastrada com o CNPJ {cnpj}.";
                        if (bloquearDuplicado)
                        {
                            errors.Add(new ValidationError("Cnpj", msg));
                        }
                        else
                        {
                            _logger.LogWarning("AVISO: {Mensagem}", msg);
                        }
                    }
                }
            }
        }

        return errors.Count > 0 ? ValidationResult.Fail(errors.ToArray()) : ValidationResult.Ok();
    }

    private static bool ValidarCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cleanCpf.Length != 11) return false;

        if (cleanCpf.Distinct().Count() == 1) return false;

        var tempCpf = cleanCpf[..9];
        var sum = 0;
        int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 9; i++)
            sum += (tempCpf[i] - '0') * multiplier1[i];

        var r = sum % 11;
        var digit1 = r < 2 ? 0 : 11 - r;

        tempCpf += digit1;
        sum = 0;
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 10; i++)
            sum += (tempCpf[i] - '0') * multiplier2[i];

        r = sum % 11;
        var digit2 = r < 2 ? 0 : 11 - r;

        return cleanCpf.EndsWith($"{digit1}{digit2}");
    }

    private static bool ValidarCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return false;

        var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());
        if (cleanCnpj.Length != 14) return false;

        if (cleanCnpj.Distinct().Count() == 1) return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var tempCnpj = cleanCnpj[..12];
        var sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (tempCnpj[i] - '0') * multiplier1[i];

        var r = sum % 11;
        var digit1 = r < 2 ? 0 : 11 - r;

        tempCnpj += digit1;
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (tempCnpj[i] - '0') * multiplier2[i];

        r = sum % 11;
        var digit2 = r < 2 ? 0 : 11 - r;

        return cleanCnpj.EndsWith($"{digit1}{digit2}");
    }

    private static bool ValidarSuframaModulo11(string suframa)
    {
        if (suframa.Length != 9) return false;
        
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = suframa[i] - '0';
            int weight = 9 - i;
            sum += digit * weight;
        }

        int remainder = sum % 11;
        int dv = 11 - remainder;
        if (dv >= 10) dv = 0;

        return dv == (suframa[8] - '0');
    }

    public async Task<PagedResult<Entidade>> ListarPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        string ordenarPor, 
        string direcaoOrdenacao, 
        string termoBusca, 
        string papelFiltro, 
        int? tipoPessoa = null,
        CancellationToken cancellationToken = default)
    {
        return await _repository.ListarPaginadoAsync(pagina, registrosPorPagina, ordenarPor, direcaoOrdenacao, termoBusca, papelFiltro, tipoPessoa, cancellationToken);
    }

    public async Task<Entidade?> ObterCompletoPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetCompletoPorIdAsync(id, cancellationToken);
    }

    public async Task<Result<Entidade>> SalvarCompletoAsync(SalvarEntidadeDto dto, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var tipoEnum = dto.TipoPessoa == (int)EntidadeTipoPessoa.Juridica ? EntidadeTipoPessoa.Juridica : EntidadeTipoPessoa.Fisica;
            var entidade = new Entidade
            {
                Nome = (tipoEnum == EntidadeTipoPessoa.Juridica && !string.IsNullOrWhiteSpace(dto.Apelido)) 
                    ? dto.Apelido 
                    : dto.Nome,
                Email = dto.Email,
                EmailNFE = dto.EmailNFE,
                EmailFinanceiro = dto.EmailFinanceiro,
                EmailVenda = dto.EmailVenda,
                EmailCompra = dto.EmailCompra,
                HomePage = dto.HomePage,
                Observacao = dto.Observacao,
                InscricaoEstadual = dto.InscricaoEstadual,
                InscricaoMunicipal = dto.InscricaoMunicipal,
                InscricaoSuframa = dto.InscricaoSuframa,
                Ativo = dto.Ativo,
                TipoPessoa = tipoEnum,
                IsCliente = dto.IsCliente,
                IsFornecedor = dto.IsFornecedor,
                IsTransportadora = dto.IsTransportadora,
                IsComissionado = dto.IsComissionado,
                IsAgenciaBancaria = dto.IsAgencia,
                IsInstituicaoFinanceira = dto.IsFinanceira,
                IsFilial = dto.IsFilial,
                IsFuncionario = dto.IsFuncionario,
                IsObra = dto.IsObra,
                IsRepresentante = dto.IsRepresentante,
                IsOutro = dto.IsOutro,
                IsProspecto = dto.IsProspecto,
                IsContador = dto.IsContador,
                IsAluno = dto.IsAluno,
                IsProfessor = dto.IsProfessor,
                IsIntermediadorComercial = dto.IsIntermediador,
                IdUsuarioInclusao = _contexto.IdUsuario,
                DataInclusao = DateTime.Today,
                HoraInclusao = DateTime.Now
            };

            if (tipoEnum == EntidadeTipoPessoa.Fisica)
            {
                entidade.PessoaFisica = new DadosPessoaFisica
                {
                    Cpf = dto.Cpf ?? string.Empty,
                    Rg = dto.Rg ?? string.Empty
                };
            }
            else
            {
                entidade.PessoaJuridica = new DadosPessoaJuridica
                {
                    Cnpj = dto.Cnpj ?? string.Empty,
                    RazaoSocial = dto.Nome
                };
            }

            entidade.IdEntidade = await _geradorSequencial.ProximoAsync("Entidade", SequencialTipo.Geral, cancellationToken);
            if (entidade.PessoaFisica != null) entidade.PessoaFisica.IdEntidade = entidade.IdEntidade;
            if (entidade.PessoaJuridica != null) entidade.PessoaJuridica.IdEntidade = entidade.IdEntidade;

            var defaultCidadeId = await _context.Cidades.Select(c => c.IdCidade).FirstOrDefaultAsync(cancellationToken);
            if (defaultCidadeId == 0) defaultCidadeId = 1;

            var defaultLogradouroId = await _context.TiposLogradouro.Select(t => t.IdTipoLogradouro).FirstOrDefaultAsync(cancellationToken);
            if (defaultLogradouroId == 0) defaultLogradouroId = 1;

            SincronizarEnderecos(entidade, dto.Enderecos, defaultCidadeId, defaultLogradouroId);

            var validation = await ValidarEntidadeAsync(entidade, isNew: true, cancellationToken);
            if (!validation.IsValid)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Entidade>.Fail(validation.Errors.ToArray());
            }

            await _repository.AddAsync(entidade, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            await ProcessarPapeisAsync(entidade.IdEntidade, dto, isNew: true, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return Result<Entidade>.Ok(entidade);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro ao gravar dados completos da entidade. Rollback executado.");
            throw;
        }
    }

    public async Task<Result<Entidade>> AtualizarCompletoAsync(int id, SalvarEntidadeDto dto, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entidade = await _repository.GetCompletoPorIdAsync(id, cancellationToken);
            if (entidade == null)
            {
                return Result<Entidade>.Fail(new ValidationError("IdEntidade", $"Entidade com ID {id} não encontrada para atualização."));
            }

            var tipoEnum = dto.TipoPessoa == (int)EntidadeTipoPessoa.Juridica ? EntidadeTipoPessoa.Juridica : EntidadeTipoPessoa.Fisica;

            entidade.Nome = (tipoEnum == EntidadeTipoPessoa.Juridica && !string.IsNullOrWhiteSpace(dto.Apelido)) 
                ? dto.Apelido 
                : dto.Nome;
            entidade.Email = dto.Email;
            entidade.EmailNFE = dto.EmailNFE;
            entidade.EmailFinanceiro = dto.EmailFinanceiro;
            entidade.EmailVenda = dto.EmailVenda;
            entidade.EmailCompra = dto.EmailCompra;
            entidade.HomePage = dto.HomePage;
            entidade.Observacao = dto.Observacao;
            entidade.InscricaoEstadual = dto.InscricaoEstadual;
            entidade.InscricaoMunicipal = dto.InscricaoMunicipal;
            entidade.InscricaoSuframa = dto.InscricaoSuframa;
            entidade.Ativo = dto.Ativo;
            entidade.TipoPessoa = tipoEnum;
            entidade.IsCliente = dto.IsCliente;
            entidade.IsFornecedor = dto.IsFornecedor;
            entidade.IsTransportadora = dto.IsTransportadora;
            entidade.IsComissionado = dto.IsComissionado;
            entidade.IsAgenciaBancaria = dto.IsAgencia;
            entidade.IsInstituicaoFinanceira = dto.IsFinanceira;
            entidade.IsFilial = dto.IsFilial;
            entidade.IsFuncionario = dto.IsFuncionario;
            entidade.IsObra = dto.IsObra;
            entidade.IsRepresentante = dto.IsRepresentante;
            entidade.IsOutro = dto.IsOutro;
            entidade.IsProspecto = dto.IsProspecto;
            entidade.IsContador = dto.IsContador;
            entidade.IsAluno = dto.IsAluno;
            entidade.IsProfessor = dto.IsProfessor;
            entidade.IsIntermediadorComercial = dto.IsIntermediador;

            entidade.IdUsuarioAlteracao = _contexto.IdUsuario;
            entidade.DataAlteracao = DateTime.Today;
            entidade.HoraAlteracao = DateTime.Now;

            if (tipoEnum == EntidadeTipoPessoa.Fisica)
            {
                if (entidade.PessoaFisica == null)
                {
                    entidade.PessoaFisica = new DadosPessoaFisica { IdEntidade = id };
                }
                entidade.PessoaFisica.Cpf = dto.Cpf ?? string.Empty;
                entidade.PessoaFisica.Rg = dto.Rg ?? string.Empty;
                entidade.PessoaJuridica = null;
            }
            else
            {
                if (entidade.PessoaJuridica == null)
                {
                    entidade.PessoaJuridica = new DadosPessoaJuridica { IdEntidade = id };
                }
                entidade.PessoaJuridica.Cnpj = dto.Cnpj ?? string.Empty;
                entidade.PessoaJuridica.RazaoSocial = dto.Nome;
                entidade.PessoaFisica = null;
            }

            var defaultCidadeId = await _context.Cidades.Select(c => c.IdCidade).FirstOrDefaultAsync(cancellationToken);
            if (defaultCidadeId == 0) defaultCidadeId = 1;

            var defaultLogradouroId = await _context.TiposLogradouro.Select(t => t.IdTipoLogradouro).FirstOrDefaultAsync(cancellationToken);
            if (defaultLogradouroId == 0) defaultLogradouroId = 1;

            SincronizarEnderecos(entidade, dto.Enderecos, defaultCidadeId, defaultLogradouroId);

            var validation = await ValidarEntidadeAsync(entidade, isNew: false, cancellationToken);
            if (!validation.IsValid)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Entidade>.Fail(validation.Errors.ToArray());
            }

            await _repository.UpdateAsync(entidade, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            await ProcessarPapeisAsync(entidade.IdEntidade, dto, isNew: false, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return Result<Entidade>.Ok(entidade);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro ao atualizar dados completos da entidade. Rollback executado.");
            throw;
        }
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entidade = await _repository.GetByIdAsync(id, cancellationToken);
            if (entidade == null) return;

            var clienteRepo = _serviceProvider.GetRequiredService<IClienteRepository>();
            var cliente = await clienteRepo.GetByIdAsync(id, cancellationToken);
            if (cliente != null) await _serviceProvider.GetRequiredService<IClienteService>().ExcluirAsync(id, cancellationToken);

            var fornecedorRepo = _serviceProvider.GetRequiredService<IFornecedorRepository>();
            var fornecedor = await fornecedorRepo.GetByIdAsync(id, cancellationToken);
            if (fornecedor != null) await _serviceProvider.GetRequiredService<IFornecedorService>().ExcluirAsync(id, cancellationToken);

            var funcionarioRepo = _serviceProvider.GetRequiredService<IFuncionarioRepository>();
            var funcionario = await funcionarioRepo.GetByIdAsync(id, cancellationToken);
            if (funcionario != null) await _serviceProvider.GetRequiredService<IFuncionarioService>().ExcluirAsync(id, cancellationToken);

            var transportadoraRepo = _serviceProvider.GetRequiredService<ITransportadoraRepository>();
            var transportadora = await transportadoraRepo.GetByIdAsync(id, cancellationToken);
            if (transportadora != null) await _serviceProvider.GetRequiredService<ITransportadoraService>().ExcluirAsync(id, cancellationToken);

            await _repository.DeleteAsync(entidade, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro ao excluir entidade {IdEntidade}. Rollback executado.", id);
            throw;
        }
    }

    private void SincronizarEnderecos(Entidade entidade, List<EnderecoDto>? incomingDtos, int defaultCidadeId, int defaultLogradouroId)
    {
        var dbEnderecos = _context.EntidadeEnderecos.Where(ee => ee.IdEntidade == entidade.IdEntidade).ToList();
        var incomingList = incomingDtos ?? new List<EnderecoDto>();

        var keysToKeep = incomingList.Select(x => x.Id).ToHashSet();
        var itemsToRemove = dbEnderecos.Where(item => !keysToKeep.Contains(item.IdEntidadeEndereco)).ToList();
        foreach (var item in itemsToRemove)
        {
            _context.EntidadeEnderecos.Remove(item);
        }

        foreach (var dto in incomingList)
        {
            var dbItem = dbEnderecos.FirstOrDefault(item => item.IdEntidadeEndereco == dto.Id && dto.Id > 0);
            if (dbItem == null)
            {
                var novoEnd = new EntidadeEndereco
                {
                    IdEntidade = entidade.IdEntidade,
                    IdCidade = defaultCidadeId,
                    IdTipoLogradouro = defaultLogradouroId,
                    Logradouro = dto.Logradouro,
                    Numero = int.TryParse(dto.Numero, out var num) ? num : null,
                    Cep = dto.Cep,
                    Complemento = string.Empty,
                    Bairro = null,
                    Ativo = true,
                    Padrao = false,
                    TipoEndereco = ParseEnderecoTipo(dto.Tipo)
                };
                _context.EntidadeEnderecos.Add(novoEnd);
            }
            else
            {
                dbItem.Logradouro = dto.Logradouro;
                dbItem.Numero = int.TryParse(dto.Numero, out var num) ? num : null;
                dbItem.Cep = dto.Cep;
                dbItem.TipoEndereco = ParseEnderecoTipo(dto.Tipo);
                _context.EntidadeEnderecos.Update(dbItem);
            }
        }
    }

    private EnderecoTipo ParseEnderecoTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo)) return EnderecoTipo.ComercialResidencial;
        return tipo.ToLower() switch
        {
            "comercial" => EnderecoTipo.Comercial,
            "residencial" => EnderecoTipo.Residencial,
            "entrega" => EnderecoTipo.Entrega,
            "cobrança" or "cobranca" => EnderecoTipo.Cobranca,
            _ => EnderecoTipo.ComercialResidencial
        };
    }

    private async Task ProcessarPapeisAsync(int id, SalvarEntidadeDto dto, bool isNew, CancellationToken cancellationToken)
    {
        var clienteService = _serviceProvider.GetRequiredService<IClienteService>();
        var clienteRepo = _serviceProvider.GetRequiredService<IClienteRepository>();
        var clienteExistente = await clienteRepo.GetByIdAsync(id, cancellationToken);

        if (dto.IsCliente)
        {
            var clienteDto = new CriarClienteDto
            {
                IdEntidade = id,
                Ativo = true,
                LimiteCredito = 0
            };
            if (clienteExistente == null)
            {
                await clienteService.CriarAsync(clienteDto, cancellationToken);
            }
            else
            {
                await clienteService.AtualizarAsync(id, clienteDto, cancellationToken);
            }
        }
        else if (clienteExistente != null)
        {
            await clienteService.ExcluirAsync(id, cancellationToken);
        }

        var fornecedorService = _serviceProvider.GetRequiredService<IFornecedorService>();
        var fornecedorRepo = _serviceProvider.GetRequiredService<IFornecedorRepository>();
        var fornecedorExistente = await fornecedorRepo.GetByIdAsync(id, cancellationToken);

        if (dto.IsFornecedor)
        {
            var fornecedorDto = new CriarFornecedorDto
            {
                IdEntidade = id,
                Ativo = true
            };
            if (fornecedorExistente == null)
            {
                await fornecedorService.CriarAsync(fornecedorDto, cancellationToken);
            }
            else
            {
                await fornecedorService.AtualizarAsync(id, fornecedorDto, cancellationToken);
            }
        }
        else if (fornecedorExistente != null)
        {
            await fornecedorService.ExcluirAsync(id, cancellationToken);
        }

        var funcionarioService = _serviceProvider.GetRequiredService<IFuncionarioService>();
        var funcionarioRepo = _serviceProvider.GetRequiredService<IFuncionarioRepository>();
        var funcionarioExistente = await funcionarioRepo.GetByIdAsync(id, cancellationToken);

        if (dto.IsFuncionario)
        {
            var funcionarioDto = new CriarFuncionarioDto
            {
                IdEntidade = id,
                Ativo = true
            };
            if (funcionarioExistente == null)
            {
                await funcionarioService.CriarAsync(funcionarioDto, cancellationToken);
            }
            else
            {
                await funcionarioService.AtualizarAsync(id, funcionarioDto, cancellationToken);
            }
        }
        else if (funcionarioExistente != null)
        {
            await funcionarioService.ExcluirAsync(id, cancellationToken);
        }

        var transportadoraService = _serviceProvider.GetRequiredService<ITransportadoraService>();
        var transportadoraRepo = _serviceProvider.GetRequiredService<ITransportadoraRepository>();
        var transportadoraExistente = await transportadoraRepo.GetByIdAsync(id, cancellationToken);

        if (dto.IsTransportadora)
        {
            var transportadoraDto = new CriarTransportadoraDto
            {
                IdEntidade = id,
                Ativo = true
            };
            if (transportadoraExistente == null)
            {
                await transportadoraService.CriarAsync(transportadoraDto, cancellationToken);
            }
            else
            {
                await transportadoraService.AtualizarAsync(id, transportadoraDto, cancellationToken);
            }
        }
        else if (transportadoraExistente != null)
        {
            await transportadoraService.ExcluirAsync(id, cancellationToken);
        }
    }
}

