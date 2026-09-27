namespace Versatus.GestaoFinanceira.Domain.DTOs;

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md (E3-T06).
// Campos = colunas de FINCAIXABANCO / FINCAIXABANCOUSUARIO / FINCONTABANCARIA (núcleo E3 +
// ENVIARSPED/CPFCNPJ) — analysis/E3-caixa-banco.md §2. Enums trafegam pelo valor inteiro do banco.

/// <summary>Filtro da listagem paginada (GET /paginado).</summary>
public sealed record FiltroCaixaBancoDto(
    string? Texto = null,
    bool? Ativo = null,
    int? IdTipoConta = null,
    bool? EntraFluxoCaixa = null,
    int Page = 1,
    int Limit = 10);

/// <summary>Linha da grade de consulta.</summary>
public sealed record CaixaBancoListaDto(
    int IdCaixaBanco,
    int IdFilial,
    string Descricao,
    int IdTipoConta,
    bool Ativo,
    bool EntraFluxoCaixa,
    int? IdTipoContaCaixa);

public sealed record CaixaBancoUsuarioDto(
    int IdCaixaBanco,
    int IdFilial,
    int IdUsuario,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>Caixa/Banco completo; <see cref="ContaBancaria"/> só quando IdTipoConta = Banco.</summary>
public sealed record CaixaBancoDto(
    int IdCaixaBanco,
    int IdFilial,
    string Descricao,
    int IdTipoConta,
    bool Ativo,
    bool EntraFluxoCaixa,
    DateTime? UltimaDataConferida,
    decimal? Saldo,
    string? ContaContabil,
    int? IdPlanoContabil,
    int? IdTipoContaCaixa,
    IReadOnlyList<CaixaBancoUsuarioDto> Usuarios,
    ContaBancariaDto? ContaBancaria,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>POST — PK (sequencial por filial) e auditoria são do servidor.</summary>
public sealed record CriarCaixaBancoDto(
    string Descricao,
    int IdTipoConta,
    bool Ativo,
    bool EntraFluxoCaixa,
    DateTime? UltimaDataConferida,
    decimal? Saldo,
    string? ContaContabil,
    int? IdPlanoContabil,
    int? IdTipoContaCaixa,
    IReadOnlyList<int>? Usuarios,
    AtualizarContaBancariaDto? ContaBancaria);

/// <summary>PUT — mesmos campos do POST; a PK vem da rota.</summary>
public sealed record AtualizarCaixaBancoDto(
    string Descricao,
    int IdTipoConta,
    bool Ativo,
    bool EntraFluxoCaixa,
    DateTime? UltimaDataConferida,
    decimal? Saldo,
    string? ContaContabil,
    int? IdPlanoContabil,
    int? IdTipoContaCaixa,
    IReadOnlyList<int>? Usuarios,
    AtualizarContaBancariaDto? ContaBancaria);

/// <summary>PUT /usuarios — grade inteira de usuários do caixa (OP-E3-07).</summary>
public sealed record SalvarCaixaBancoUsuariosDto(IReadOnlyList<CaixaBancoUsuarioItemDto> Itens);

/// <summary>
/// Saldo do caixa na data (GET /saldo — SaldoCaixaBanco.Retornar, OP-E3-08). <c>Saldo</c> e
/// <c>SaldoConciliado</c> são calculados (CALC-E3-01/02), não colunas.
/// </summary>
public sealed record SaldoCaixaBancoDto(
    int IdCaixaBanco,
    int IdFilial,
    DateTime DataSaldo,
    decimal? SaldoAnterior,
    decimal? TotalDebito,
    decimal? TotalCredito,
    decimal Saldo,
    decimal? SaldoAnteriorConciliado,
    decimal? TotalDebitoConciliado,
    decimal? TotalCreditoConciliado,
    decimal SaldoConciliado,
    bool Conferido);

/// <summary>Conta bancária — núcleo E3. Integração bancária (boleto/remessa/retorno) → E14.</summary>
public sealed record ContaBancariaDto(
    int IdCaixaBanco,
    int IdFilial,
    int IdAgencia,
    string? Titular,
    string NumeroConta,
    string? DigitoConta,
    decimal? Limite,
    decimal? CreditoPendente,
    decimal? DebitoPendente,
    decimal? ChequePendente,
    bool ContaTerceiro,
    bool PermiteEmitirCheque,
    int? IdContaBancariaVinculada,
    int IdTipoContaBancaria,
    int? IdInstituicaoFinanceira,
    bool EnviarSped,
    string? CpfCnpj,
    int? IdUsuarioInclusao,
    DateTime? DataInclusao,
    DateTime? HoraInclusao,
    int? IdUsuarioAlteracao,
    DateTime? DataAlteracao,
    DateTime? HoraAlteracao);

/// <summary>Campos editáveis da conta bancária (PUT /conta-bancaria e bloco do caixa).</summary>
public sealed record AtualizarContaBancariaDto(
    int IdAgencia,
    string? Titular,
    string NumeroConta,
    string? DigitoConta,
    decimal? Limite,
    decimal? CreditoPendente,
    decimal? DebitoPendente,
    decimal? ChequePendente,
    bool ContaTerceiro,
    bool PermiteEmitirCheque,
    int? IdContaBancariaVinculada,
    int IdTipoContaBancaria,
    int? IdInstituicaoFinanceira,
    bool EnviarSped,
    string? CpfCnpj);
