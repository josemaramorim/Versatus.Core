namespace Versatus.GestaoFinanceira.Domain.DTOs;

// Campos = colunas de FINDOMINIO / FINDOMINIOUSUARIO / FINDOMINIORESPONSAVEL —
// analysis/E2-dominio.md §2. O contrato REST (contracts/dominio-periodo.md) é do E2-T06.

/// <summary>Filtro da listagem paginada.</summary>
public sealed record FiltroDominioDto(
    string? Texto = null,
    bool? Ativo = null,
    int Page = 1,
    int Limit = 10);

/// <summary>Linha da grade de consulta.</summary>
public sealed record DominioListaDto(
    int IdDominio,
    int IdFilial,
    string Descricao,
    int IdCaixaBanco,
    bool Ativo,
    int? IdDominioPeriodo);

public sealed record DominioUsuarioDto(
    int IdDominio,
    int IdUsuario,
    int IdFilial,
    bool UsuarioPrincipal,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

public sealed record DominioResponsavelDto(
    int IdDominio,
    int IdFilial,
    int IdDominioPai,
    bool DominioPaiPrincipal,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>Domínio completo. Saldos e período corrente são só leitura (não vêm do cadastro).</summary>
public sealed record DominioDto(
    int IdDominio,
    int IdFilial,
    int IdCaixaBanco,
    int? IdDominioPeriodo,
    string Descricao,
    bool Ativo,
    bool Tesouraria,
    bool AbrirPeriodo,
    bool AbrirOutrosPeriodos,
    bool FecharPeriodo,
    bool FecharOutrosPeriodos,
    bool MovimentoBanco,
    bool ConsultaTodosPeriodos,
    decimal? Saldo,
    decimal? SaldoDinheiro,
    decimal? SaldoChequeRecebido,
    decimal? SaldoCartao,
    IReadOnlyList<DominioUsuarioDto> Usuarios,
    IReadOnlyList<DominioResponsavelDto> Responsaveis,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>
/// Linha da grade de usuários do domínio.
/// <paramref name="IdUsuarioSalvo"/>: usuário que a linha tinha ao ser carregada (linha salva);
/// <c>null</c> para linha nova. Diferente de <paramref name="IdUsuario"/> = tentativa de alterar o
/// usuário (VAL-E2-30).
/// </summary>
public sealed record DominioUsuarioItemDto(int IdUsuario, bool UsuarioPrincipal, int? IdUsuarioSalvo = null);

/// <summary>
/// Linha da grade de domínios responsáveis. <paramref name="IdDominioPaiSalvo"/> segue a mesma regra
/// de <see cref="DominioUsuarioItemDto.IdUsuarioSalvo"/> (VAL-E2-36).
/// </summary>
public sealed record DominioResponsavelItemDto(int IdDominioPai, bool DominioPaiPrincipal, int? IdDominioPaiSalvo = null);

/// <summary>POST/PUT do cadastro — PK (sequencial por filial), auditoria, saldos e período são do servidor.</summary>
public sealed record SalvarDominioDto(
    string Descricao,
    int IdCaixaBanco,
    bool Ativo,
    bool Tesouraria,
    bool AbrirPeriodo,
    bool AbrirOutrosPeriodos,
    bool FecharPeriodo,
    bool FecharOutrosPeriodos,
    bool MovimentoBanco,
    bool ConsultaTodosPeriodos,
    IReadOnlyList<DominioUsuarioItemDto>? Usuarios,
    IReadOnlyList<DominioResponsavelItemDto>? Responsaveis);

/// <summary>Data/hora do último fechamento do domínio (Dominio.GetDataUltimoFechamento).</summary>
public sealed record UltimoFechamentoDto(DateTime DataFechamento, DateTime? HoraFechamento);

/// <summary>Linha da central do domínio (FCentralDominio.CentralDominio).</summary>
public sealed record CentralDominioItemDto(
    int? IdDominioPai,
    int IdUsuario,
    int IdDominio,
    string Dominio,
    int IdCaixaBanco,
    int IdDominioPeriodo,
    DateTime? DataAbertura,
    DateTime? HoraAbertura,
    DateTime? DataFechamento,
    DateTime? HoraFechamento,
    decimal? SaldoDinheiro,
    decimal? SaldoChequeRecebido,
    decimal? SaldoCartao,
    decimal? Saldo);

/// <summary>Grades da central do domínio do usuário logado (OP-E2-17).</summary>
public sealed record CentralDominioDto(
    bool PodeAbrir,
    bool PodeFechar,
    IReadOnlyList<CentralDominioItemDto> CaixasParaAbrir,
    IReadOnlyList<CentralDominioItemDto> CaixasParaFechar,
    IReadOnlyList<CentralDominioItemDto> Consulta);

/// <summary>Informação de um período aberto (PeriodosAbertos.GetInfoPeriodo).</summary>
public sealed record InfoPeriodoDto(
    Versatus.SharedKernel.Enums.PeriodoStatus Status,
    int IdUsuario,
    int IdCaixaBanco,
    string NomeUsuario,
    string DescricaoCaixaBanco);
