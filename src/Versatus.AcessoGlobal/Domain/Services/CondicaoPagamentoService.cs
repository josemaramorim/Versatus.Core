using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Finance;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Context;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;

namespace Versatus.AcessoGlobal.Domain.Services;

public class CondicaoPagamentoService : ICondicaoPagamentoService
{
    private readonly AcessoGlobalDbContext _context;
    private readonly IGeradorSequencial _geradorSequencial;
    private readonly IContextoExecucao _contexto;

    public CondicaoPagamentoService(
        AcessoGlobalDbContext context,
        IGeradorSequencial geradorSequencial,
        IContextoExecucao contexto)
    {
        _context = context;
        _geradorSequencial = geradorSequencial;
        _contexto = contexto;
    }

    public async Task<PagedResult<CondicaoPagamentoResponseDto>> ListarPaginadoAsync(
        int page,
        int limit,
        string sortBy,
        string sortOrder,
        string search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.CondicoesPagamento.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Descricao.Contains(search));
        }

        // Ordenação
        bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLower() switch
        {
            "idcondicaopagamento" => desc ? query.OrderByDescending(x => x.IdCondicaoPagamento) : query.OrderBy(x => x.IdCondicaoPagamento),
            "descricao" => desc ? query.OrderByDescending(x => x.Descricao) : query.OrderBy(x => x.Descricao),
            "idtipocondicaopagto" => desc ? query.OrderByDescending(x => x.IdTipoCondicaoPagto) : query.OrderBy(x => x.IdTipoCondicaoPagto),
            "iddisponibilidade" => desc ? query.OrderByDescending(x => x.IdDisponibilidade) : query.OrderBy(x => x.IdDisponibilidade),
            "ativo" => desc ? query.OrderByDescending(x => x.Ativo) : query.OrderBy(x => x.Ativo),
            _ => query.OrderBy(x => x.Descricao)
        };

        // Regra do SQL Server 2008: Carregar em memória antes de paginar
        var total = await query.CountAsync(cancellationToken);
        var todos = await query.ToListAsync(cancellationToken);

        var pagina = todos
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(MapToResponseDto)
            .ToList();

        return new PagedResult<CondicaoPagamentoResponseDto>(pagina, total);
    }

    public async Task<CondicaoPagamentoResponseDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cp = await _context.CondicoesPagamento
            .Include(x => x.Regras)
            .FirstOrDefaultAsync(x => x.IdCondicaoPagamento == id, cancellationToken);

        return cp == null ? null : MapToResponseDto(cp);
    }

    public async Task<CondicaoPagamentoResponseDto> CriarAsync(CriarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default)
    {
        // Validar dados antes de salvar
        ValidateCondicao(dto);

        var idCp = await _geradorSequencial.ProximoAsync("CondicaoPagamento", SequencialTipo.Geral, cancellationToken);

        var cp = new CondicaoPagamento
        {
            IdCondicaoPagamento = idCp,
            Descricao = dto.Descricao,
            Ativo = dto.Ativo,
            IdTipoCondicaoPagto = dto.IdTipoCondicaoPagto,
            IdDisponibilidade = dto.IdDisponibilidade,
            IdTipoVencimento = dto.IdTipoVencimento,
            IdGrupoCondicaoPagamento = dto.IdGrupoCondicaoPagamento,
            IdFormaCobranca = dto.IdFormaCobranca,
            IdFormaPagamento = dto.IdFormaPagamento,
            IdFormaPagamentoVista = dto.IdFormaPagamentoVista,
            OrdemConsulta = dto.OrdemConsulta,
            UtilizarPdv = dto.UtilizarPdv,
            RecebeAcrescimo = dto.RecebeAcrescimo,
            Acrescimo = dto.Acrescimo,
            RecebeDesconto = dto.RecebeDesconto,
            Desconto = dto.Desconto,
            AlteraParcelas = dto.AlteraParcelas,
            AlteraNroParcela = dto.AlteraNroParcela,
            IdParcelamentoTipo = dto.IdParcelamentoTipo,
            TipoDivisaoParcelamento = dto.TipoDivisaoParcelamento,
            QuantidadeParcela = dto.QuantidadeParcela,
            DiasParcelamento = dto.DiasParcelamento,
            UsarMesComercial = dto.UsarMesComercial,
            DiasMinimoProximoMes = dto.DiasMinimoProximoMes,
            PrimeiraParcelaAVista = dto.PrimeiraParcelaAVista,
            ObrigatorioFormaPagamento = dto.ObrigatorioFormaPagamento,
            IdParcelaArredondamento = dto.IdParcelaArredondamento,
            QuantidadeFaixa = dto.QuantidadeFaixa,
            IdDiaSemana = dto.IdDiaSemana,

            // Auditoria
            IdUsuarioInclusao = _contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Now
        };

        // Limpar campos não utilizados de acordo com o tipo
        LimparCamposInativos(cp);

        // Adicionar regras
        foreach (var r in dto.Regras)
        {
            var idRegra = await _geradorSequencial.ProximoAsync("CondicaoPagtoRegra", SequencialTipo.Geral, cancellationToken);
            cp.Regras.Add(new CondicaoPagtoRegra
            {
                IdCondicaoPagtoParcela = idRegra,
                IdCondicaoPagamento = idCp,
                NumeroDias = r.NumeroDias,
                NumeroParcela = r.NumeroParcela,
                PercentualDivisao = r.PercentualDivisao,
                DiaInicial = r.DiaInicial,
                DiaFinal = r.DiaFinal,
                DiasLiberado = r.DiasLiberado,
                PercentualValorMinimo = r.PercentualValorMinimo
            });
        }

        _context.CondicoesPagamento.Add(cp);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(cp);
    }

    public async Task RazorFixCompilacaoBugAsync() => await Task.CompletedTask;

    public async Task AtualizarAsync(int id, EditarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default)
    {
        // Validar dados antes de salvar
        ValidateCondicao(dto);

        var cp = await _context.CondicoesPagamento
            .Include(x => x.Regras)
            .FirstOrDefaultAsync(x => x.IdCondicaoPagamento == id, cancellationToken);

        if (cp == null)
        {
            throw new InvalidOperationException($"Condição de pagamento com ID {id} não encontrada.");
        }

        // Atualizar campos
        cp.Descricao = dto.Descricao;
        cp.Ativo = dto.Ativo;
        cp.IdTipoCondicaoPagto = dto.IdTipoCondicaoPagto;
        cp.IdDisponibilidade = dto.IdDisponibilidade;
        cp.IdTipoVencimento = dto.IdTipoVencimento;
        cp.IdGrupoCondicaoPagamento = dto.IdGrupoCondicaoPagamento;
        cp.IdFormaCobranca = dto.IdFormaCobranca;
        cp.IdFormaPagamento = dto.IdFormaPagamento;
        cp.IdFormaPagamentoVista = dto.IdFormaPagamentoVista;
        cp.OrdemConsulta = dto.OrdemConsulta;
        cp.UtilizarPdv = dto.UtilizarPdv;
        cp.RecebeAcrescimo = dto.RecebeAcrescimo;
        cp.Acrescimo = dto.Acrescimo;
        cp.RecebeDesconto = dto.RecebeDesconto;
        cp.Desconto = dto.Desconto;
        cp.AlteraParcelas = dto.AlteraParcelas;
        cp.AlteraNroParcela = dto.AlteraNroParcela;
        cp.IdParcelamentoTipo = dto.IdParcelamentoTipo;
        cp.TipoDivisaoParcelamento = dto.TipoDivisaoParcelamento;
        cp.QuantidadeParcela = dto.QuantidadeParcela;
        cp.DiasParcelamento = dto.DiasParcelamento;
        cp.UsarMesComercial = dto.UsarMesComercial;
        cp.DiasMinimoProximoMes = dto.DiasMinimoProximoMes;
        cp.PrimeiraParcelaAVista = dto.PrimeiraParcelaAVista;
        cp.ObrigatorioFormaPagamento = dto.ObrigatorioFormaPagamento;
        cp.IdParcelaArredondamento = dto.IdParcelaArredondamento;
        cp.QuantidadeFaixa = dto.QuantidadeFaixa;
        cp.IdDiaSemana = dto.IdDiaSemana;

        // Auditoria
        cp.IdUsuarioAlteracao = _contexto.IdUsuario;
        cp.DataAlteracao = DateTime.Today;
        cp.HoraAlteracao = DateTime.Now;

        // Limpar campos não utilizados de acordo com o tipo
        LimparCamposInativos(cp);

        // Remover regras antigas e reinserir
        _context.CondicoesPagtoRegra.RemoveRange(cp.Regras);
        cp.Regras.Clear();

        foreach (var r in dto.Regras)
        {
            var idRegra = await _geradorSequencial.ProximoAsync("CondicaoPagtoRegra", SequencialTipo.Geral, cancellationToken);
            cp.Regras.Add(new CondicaoPagtoRegra
            {
                IdCondicaoPagtoParcela = idRegra,
                IdCondicaoPagamento = id,
                NumeroDias = r.NumeroDias,
                NumeroParcela = r.NumeroParcela,
                PercentualDivisao = r.PercentualDivisao,
                DiaInicial = r.DiaInicial,
                DiaFinal = r.DiaFinal,
                DiasLiberado = r.DiasLiberado,
                PercentualValorMinimo = r.PercentualValorMinimo
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var cp = await _context.CondicoesPagamento
            .Include(x => x.Regras)
            .FirstOrDefaultAsync(x => x.IdCondicaoPagamento == id, cancellationToken);

        if (cp == null)
        {
            throw new InvalidOperationException($"Condição de pagamento com ID {id} não encontrada.");
        }

        _context.CondicoesPagamento.Remove(cp);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<GrupoCondicaoPagamentoResponseDto>> ListarGruposAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GruposCondicaoPagamento
            .Where(x => x.Ativo)
            .OrderBy(x => x.Descricao)
            .Select(x => new GrupoCondicaoPagamentoResponseDto(x.IdGrupoCondicaoPagamento, x.Descricao, x.Ativo))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FormaCobrancaResponseDto>> ListarFormasCobrancaAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FormasCobranca
            .Where(x => x.Ativo)
            .OrderBy(x => x.Descricao)
            .Select(x => new FormaCobrancaResponseDto(x.IdFormaCobranca, x.Descricao, x.SiglaForma, x.Ativo))
            .ToListAsync(cancellationToken);
    }

    #region Validações e Limpeza de Campos
    private void ValidateCondicao(CriarCondicaoPagamentoDto dto)
    {
        if (dto.IdTipoCondicaoPagto == CondicaoPagtoTipo.Semanal)
        {
            if (dto.IdDiaSemana == 0)
            {
                throw new InvalidOperationException("Para condição semanal, o dia da semana é obrigatório.");
            }
            return;
        }

        if (dto.IdTipoCondicaoPagto == CondicaoPagtoTipo.Parcelada)
        {
            if (!dto.AlteraParcelas)
            {
                if (dto.QuantidadeParcela <= 0)
                {
                    throw new InvalidOperationException("Deve ser informada a quantidade de parcelas.");
                }

                if (dto.Regras.Count == 0)
                {
                    throw new InvalidOperationException("Deve ser configurada ao menos uma parcela na grade.");
                }

                if (dto.Regras.Count != dto.QuantidadeParcela)
                {
                    throw new InvalidOperationException($"A grade de parcelas deve possuir exatamente {dto.QuantidadeParcela} parcelas.");
                }

                if (dto.TipoDivisaoParcelamento == DivisaoParcelamentoTipo.Percentual)
                {
                    var somaPercentuais = dto.Regras.Sum(r => r.PercentualDivisao);
                    if (somaPercentuais != 100)
                    {
                        throw new InvalidOperationException("A soma dos percentuais das parcelas deve ser exatamente 100%.");
                    }
                }
            }
            else
            {
                // Validações de Condição Livre
                if (dto.IdParcelaArredondamento != ParcelamentoArredondamento.Ultima)
                {
                    throw new InvalidOperationException("Para condição livre, o arredondamento deve ser definido como 'Última parcela'.");
                }

                if (dto.TipoDivisaoParcelamento == DivisaoParcelamentoTipo.Percentual)
                {
                    throw new InvalidOperationException("Para condição livre, o tipo da divisão deve ser definido como 'Quantidade'.");
                }

                if (dto.IdParcelamentoTipo == ParcelamentoTipo.DiasUteis)
                {
                    throw new InvalidOperationException("Para condição livre, não é permitido o tipo de parcelamento 'Dias úteis'.");
                }
            }

            if (dto.UsarMesComercial)
            {
                if (dto.IdParcelamentoTipo != ParcelamentoTipo.DiasEntreParcela)
                {
                    throw new InvalidOperationException("Para usar mês comercial, o parcelamento deve ser do tipo 'Dias entre parcelas'.");
                }
                if (dto.DiasParcelamento != 30)
                {
                    throw new InvalidOperationException("Para usar mês comercial, o número de dias entre parcelas deve ser igual a 30.");
                }
            }

            if (dto.PrimeiraParcelaAVista && dto.IdParcelamentoTipo != ParcelamentoTipo.DiasEntreParcela)
            {
                throw new InvalidOperationException("Para marcar 1ª parcela à vista, o tipo do parcelamento deve ser do tipo 'Dias entre parcelas'.");
            }
        }

        if (dto.IdTipoCondicaoPagto == CondicaoPagtoTipo.FaixaDias)
        {
            if (dto.Regras.Count == 0)
            {
                throw new InvalidOperationException("Devem ser informadas as faixas para o cálculo.");
            }

            // Validar ordem das faixas e cruzamento de limites
            foreach (var r in dto.Regras)
            {
                if (r.DiaFinal < r.DiaInicial)
                {
                    throw new InvalidOperationException("O dia final da faixa não pode ser inferior ao dia inicial.");
                }
                if (r.NumeroDias < 1 || r.NumeroDias > 31)
                {
                    throw new InvalidOperationException("O dia do vencimento da faixa deve estar entre 1 e 31.");
                }
                if (r.NumeroDias >= r.DiaInicial && r.NumeroDias <= r.DiaFinal)
                {
                    throw new InvalidOperationException("O dia do vencimento não pode estar dentro do intervalo da própria faixa.");
                }
            }
        }
    }

    private void LimparCamposInativos(CondicaoPagamento cp)
    {
        switch (cp.IdTipoCondicaoPagto)
        {
            case CondicaoPagtoTipo.Parcelada:
                cp.IdDiaSemana = 0;
                cp.QuantidadeFaixa = 0;
                break;
            case CondicaoPagtoTipo.FaixaDias:
                cp.IdDiaSemana = 0;
                LimparCamposParcelamento(cp);
                break;
            case CondicaoPagtoTipo.Semanal:
                cp.QuantidadeFaixa = 0;
                LimparCamposParcelamento(cp);
                break;
        }
    }

    private void LimparCamposParcelamento(CondicaoPagamento cp)
    {
        cp.QuantidadeParcela = 0;
        cp.IdParcelamentoTipo = 0;
        cp.DiasParcelamento = 0;
        cp.DiasMinimoProximoMes = 0;
        cp.TipoDivisaoParcelamento = 0;
        cp.IdFormaPagamentoVista = null;
        cp.PrimeiraParcelaAVista = false;
        cp.AlteraParcelas = false;
        cp.AlteraNroParcela = false;
        cp.ObrigatorioFormaPagamento = false;
        cp.IdParcelaArredondamento = 0;
    }
    #endregion

    private CondicaoPagamentoResponseDto MapToResponseDto(CondicaoPagamento cp)
    {
        return new CondicaoPagamentoResponseDto
        {
            IdCondicaoPagamento = cp.IdCondicaoPagamento,
            Descricao = cp.Descricao,
            Ativo = cp.Ativo,
            IdTipoCondicaoPagto = cp.IdTipoCondicaoPagto,
            IdDisponibilidade = cp.IdDisponibilidade,
            IdTipoVencimento = cp.IdTipoVencimento,
            IdGrupoCondicaoPagamento = cp.IdGrupoCondicaoPagamento,
            IdFormaCobranca = cp.IdFormaCobranca,
            IdFormaPagamento = cp.IdFormaPagamento,
            IdFormaPagamentoVista = cp.IdFormaPagamentoVista,
            OrdemConsulta = cp.OrdemConsulta,
            UtilizarPdv = cp.UtilizarPdv,
            RecebeAcrescimo = cp.RecebeAcrescimo,
            Acrescimo = cp.Acrescimo,
            RecebeDesconto = cp.RecebeDesconto,
            Desconto = cp.Desconto,
            AlteraParcelas = cp.AlteraParcelas,
            AlteraNroParcela = cp.AlteraNroParcela,
            IdParcelamentoTipo = cp.IdParcelamentoTipo,
            TipoDivisaoParcelamento = cp.TipoDivisaoParcelamento,
            QuantidadeParcela = cp.QuantidadeParcela,
            DiasParcelamento = cp.DiasParcelamento,
            UsarMesComercial = cp.UsarMesComercial,
            DiasMinimoProximoMes = cp.DiasMinimoProximoMes,
            PrimeiraParcelaAVista = cp.PrimeiraParcelaAVista,
            ObrigatorioFormaPagamento = cp.ObrigatorioFormaPagamento,
            IdParcelaArredondamento = cp.IdParcelaArredondamento,
            QuantidadeFaixa = cp.QuantidadeFaixa,
            IdDiaSemana = cp.IdDiaSemana,

            Regras = cp.Regras.Select(r => new CondicaoPagtoRegraDto
            {
                IdCondicaoPagtoParcela = r.IdCondicaoPagtoParcela,
                NumeroDias = r.NumeroDias,
                NumeroParcela = r.NumeroParcela,
                PercentualDivisao = r.PercentualDivisao,
                DiaInicial = r.DiaInicial,
                DiaFinal = r.DiaFinal,
                DiasLiberado = r.DiasLiberado,
                PercentualValorMinimo = r.PercentualValorMinimo
            }).ToList()
        };
    }
}
