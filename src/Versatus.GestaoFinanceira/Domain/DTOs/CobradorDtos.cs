namespace Versatus.GestaoFinanceira.Domain.DTOs;

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md §Cobrador (E3-T06).
// Campos = colunas de FINCOBRADOR — analysis/E3-caixa-banco.md §2.6.

public sealed record FiltroCobradorDto(string? Texto = null, bool? Ativo = null, int Page = 1, int Limit = 10);

public sealed record CobradorDto(
    int IdCobrador,
    int IdFilial,
    int IdEntidade,
    string Nome,
    bool Ativo,
    int? IdUsuario,
    int? IdMeioContato,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>POST/PUT — PK (sequencial por filial) e auditoria são do servidor.</summary>
public sealed record SalvarCobradorDto(
    int IdEntidade,
    string Nome,
    int? IdUsuario,
    int? IdMeioContato,
    bool Ativo = true);
