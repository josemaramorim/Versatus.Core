namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Tipo de destino de rateio/aplicação de item financeiro (centro de custo, classe,
/// projeto, plano de contas, caixa, banco). Não é o tipo de conta bancária —
/// ver TipoContaBancaria (1478).
/// Origem: Projeto.Geral.Enumerado.ContaFinanceiroTipo (legado, [TipoEnumerado(177)]) —
/// persistido em FINAPLICACAOITEMFIN e afins.
/// </summary>
public enum ContaFinanceiroTipo
{
    CentroCusto = 178,
    Classe = 179,
    Projeto = 180,
    PlanoConta = 181,
    Caixa = 182,
    Banco = 183
}
