using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/FechamentoCaixaBase.cs (legado,
//         ObjectPersist)
// Base de: linhas de fechamento de caixa por forma de pagamento (E2 — Domínio/Período).
//
// POCO abstrata SÓ DE DADOS (Artigo III). As regras de edição (PermiteDigitarQtde /
// PermiteDigitarValor — VAL-E1-29..30) e a contagem (CalcularQtdeValorEditado / SetQtdeValor
// — OP-E1-11) vivem em Application/Bases/FechamentoCaixaService (E1-T03). A máquina de
// estados do período (abrir/fechar/fechar tesouraria) e o enum de Situacao são do épico E2.
public abstract class FechamentoCaixaBase
{
    /// <summary>Status do fechamento do domínio/período (enum definido no E2).</summary>
    public int Situacao { get; set; }

    /// <summary>Forma de pagamento desta linha do fechamento.</summary>
    public FormaPagtoTipo TipoFormaPagto { get; set; }

    /// <summary>Quantidade informada para a forma de pagamento.</summary>
    public int Quantidade { get; set; }

    /// <summary>Valor informado (contado) para a forma de pagamento.</summary>
    public decimal ValorInformado { get; set; }

    /// <summary>Valor calculado pelo sistema para a forma de pagamento.</summary>
    public decimal ValorCalculado { get; set; }

    /// <summary>Valor digitado na conferência.</summary>
    public decimal ValorConferencia { get; set; }

    /// <summary>Moedas configuradas para contagem (parâmetro <c>MoedaParaContagem</c>).</summary>
    public string MoedaContagem { get; set; } = string.Empty;

    /// <summary>Se o fechamento é para o PDV.</summary>
    public bool FechamentoPdv { get; set; }
}
