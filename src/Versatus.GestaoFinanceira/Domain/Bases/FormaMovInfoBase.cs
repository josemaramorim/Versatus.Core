namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/FormaMovInfo.cs (legado,
//         `abstract class FormaMovInfo : ObjectPersist`)
// Base de: informações adicionais de uma forma de pagamento de movimento (as 11
//          especializações FormaMovInfo* do épico E5).
//
// POCO abstrata SÓ DE DADOS (Artigo III). O comportamento (RetornarValor / DefinirValor —
// teto em ValorDisponivel, VAL-E1-31) vive no serviço de forma de movimento (E5). A
// referência à forma de pagamento de movimento é int lógico / composição, resolvida pela
// entidade concreta em E5.
public abstract class FormaMovInfoBase
{
    /// <summary>Valor máximo possível de ser utilizado nesta forma.</summary>
    public decimal ValorDisponivel { get; set; }

    /// <summary>Se o valor informado deve ser limitado a <see cref="ValorDisponivel"/>.</summary>
    public bool LimitarValorDisponivel { get; set; }
}
