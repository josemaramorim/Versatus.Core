namespace Versatus.GestaoFinanceira.Domain.DTOs;

/// <summary>Item de lista de consulta (lookup) usado pelos campos de busca das telas — E3-T07.</summary>
public sealed record ItemLookupDto(int Id, string Descricao);
