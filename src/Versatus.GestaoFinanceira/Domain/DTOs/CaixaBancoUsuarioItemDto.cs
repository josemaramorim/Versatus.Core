namespace Versatus.GestaoFinanceira.Domain.DTOs;

/// <summary>
/// Linha da grade de usuários do caixa (FINCAIXABANCOUSUARIO) enviada em lote — OP-E3-07.
/// </summary>
/// <param name="IdUsuario">Usuário da linha.</param>
/// <param name="IdUsuarioSalvo">
/// Usuário que a linha tinha quando foi carregada (linha já salva); <c>null</c> para linha nova.
/// Diferente de <paramref name="IdUsuario"/> = tentativa de alterar o usuário (VAL-E3-12).
/// </param>
public sealed record CaixaBancoUsuarioItemDto(int IdUsuario, int? IdUsuarioSalvo = null);
