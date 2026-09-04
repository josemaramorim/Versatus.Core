namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Estratégia de montagem do texto de histórico de liquidação/estorno. Épicos E6/E7.
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoHistoricoLiquidacaoEstorno (legado) —
/// não-persistido (comportamento em memória, nunca vira coluna).
/// </summary>
public enum TipoHistoricoLiquidacaoEstorno
{
    Parcela = 1,
    ParcelaQtde = 2,
    Entidade = 3,
    EntidadeRazao = 4,
    EntidadeNomeRazao = 5,
    Vencimento = 6
}
