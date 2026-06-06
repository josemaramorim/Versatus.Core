using System.Linq;
using Microsoft.Extensions.Logging;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public class EntidadeService : IEntidadeService
{
    private readonly IEntidadeRepository _repository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IGeradorSequencial _geradorSequencial;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<EntidadeService> _logger;

    public EntidadeService(
        IEntidadeRepository repository,
        IParametroRepository parametroRepository,
        IGeradorSequencial geradorSequencial,
        IContextoExecucao contexto,
        ILogger<EntidadeService> logger)
    {
        _repository = repository;
        _parametroRepository = parametroRepository;
        _geradorSequencial = geradorSequencial;
        _contexto = contexto;
        _logger = logger;
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
        await ValidarEntidadeAsync(entidade, isNew: true, cancellationToken);

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
        await ValidarEntidadeAsync(entidade, isNew: false, cancellationToken);

        // 2. Auditoria Alteração
        entidade.IdUsuarioAlteracao = _contexto.IdUsuario;
        entidade.DataAlteracao = DateTime.Today;
        entidade.HoraAlteracao = DateTime.Now;

        // 3. Atualizar
        await _repository.UpdateAsync(entidade, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entidade {IdEntidade} atualizada com sucesso.", entidade.IdEntidade);
    }

    private async Task ValidarEntidadeAsync(Entidade entidade, bool isNew, CancellationToken cancellationToken)
    {
        // 1. Validação de Razão Social
        if (entidade.TipoPessoa == EntidadeTipoPessoa.Juridica && entidade.PessoaJuridica != null)
        {
            var razaoSocial = entidade.PessoaJuridica.RazaoSocial;
            if (!string.IsNullOrEmpty(razaoSocial))
            {
                var cleanRazao = string.Join(" ", razaoSocial.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                if (cleanRazao.Length < 3)
                {
                    throw new InvalidOperationException("A razão social deve ter no mínimo 3 caracteres válidos.");
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
            return;
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
                throw new InvalidOperationException($"Deve ser informado o {sCampo}.");
            }
            else if (obrigatorioAvisar)
            {
                _logger.LogWarning("AVISO: Para a entidade é aconselhável preenchimento do {Campo}.", sCampo);
            }
            return; // Se não foi informado e não bloqueou, não há valor para validar matemática ou unicidade
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
                    throw new InvalidOperationException("CPF inválido.");
                }
            }
            
            if (cnpj != null)
            {
                if (!ValidarCNPJ(cnpj))
                {
                    throw new InvalidOperationException("CNPJ inválido.");
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
                            throw new InvalidOperationException(msg);
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
                            throw new InvalidOperationException(msg);
                        }
                        else
                        {
                            _logger.LogWarning("AVISO: {Mensagem}", msg);
                        }
                    }
                }
            }
        }
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
}

