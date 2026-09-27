using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>
/// Conversão entidade ↔ DTO do épico E3 (contracts/caixa-banco.md). Fica no domínio para que o
/// controller só repasse <c>record</c>s (Artigo IV.2 — controller não monta agregado).
/// </summary>
internal static class BancosDtoMapper
{
    public static CaixaBancoListaDto ParaListaDto(CaixaBanco x) => new(
        x.IdCaixaBanco, x.IdFilial, x.Descricao, (int)x.TipoConta, x.Ativo, x.EntraFluxoCaixa, (int?)x.TipoContaCaixa);

    public static CaixaBancoUsuarioDto ParaDto(CaixaBancoUsuario x) => new(
        x.IdCaixaBanco, x.IdFilial, x.IdUsuario,
        x.IdUsuarioInclusao, x.DataInclusao, x.HoraInclusao, x.IdUsuarioAlteracao, x.DataAlteracao, x.HoraAlteracao);

    public static CaixaBancoDto ParaDto(CaixaBanco x, ContaBancaria? conta) => new(
        x.IdCaixaBanco, x.IdFilial, x.Descricao, (int)x.TipoConta, x.Ativo, x.EntraFluxoCaixa,
        x.UltimaDataConferida, x.Saldo, x.ContaContabil, x.IdPlanoContabil, (int?)x.TipoContaCaixa,
        [.. x.Usuarios.OrderBy(u => u.IdUsuario).Select(ParaDto)],
        x.TipoConta == ContaTipo.Banco && conta is not null ? ParaDto(conta) : null,
        x.IdUsuarioInclusao, x.DataInclusao, x.HoraInclusao, x.IdUsuarioAlteracao, x.DataAlteracao, x.HoraAlteracao);

    public static ContaBancariaDto ParaDto(ContaBancaria x) => new(
        x.IdCaixaBanco, x.IdFilial, x.IdAgencia, x.Titular, x.NumeroConta, x.DigitoConta,
        x.Limite, x.CreditoPendente, x.DebitoPendente, x.ChequePendente, x.ContaTerceiro, x.PermiteEmitirCheque,
        x.IdContaBancariaVinculada, (int)x.ContaBancariaTipo, x.IdInstituicaoFinanceira, x.EnviarSped, x.CpfCnpj,
        x.IdUsuarioInclusao, x.DataInclusao, x.HoraInclusao, x.IdUsuarioAlteracao, x.DataAlteracao, x.HoraAlteracao);

    public static CobradorDto ParaDto(Cobrador x) => new(
        x.IdCobrador, x.IdFilial, x.IdEntidade, x.Nome, x.Ativo, x.IdUsuario, x.IdMeioContato,
        x.IdUsuarioInclusao, x.DataInclusao, x.HoraInclusao, x.IdUsuarioAlteracao, x.DataAlteracao, x.HoraAlteracao);

    public static CaixaBanco ParaEntidade(int idCaixaBanco, int idFilial, string descricao, int idTipoConta, bool ativo,
        bool entraFluxoCaixa, DateTime? ultimaDataConferida, decimal? saldo, string? contaContabil, int? idPlanoContabil,
        int? idTipoContaCaixa, IReadOnlyList<int>? usuarios)
    {
        var caixa = new CaixaBanco
        {
            IdCaixaBanco = idCaixaBanco,
            IdFilial = idFilial,
            Descricao = descricao,
            TipoConta = (ContaTipo)idTipoConta,
            Ativo = ativo,
            EntraFluxoCaixa = entraFluxoCaixa,
            UltimaDataConferida = ultimaDataConferida,
            Saldo = saldo,
            ContaContabil = contaContabil,
            IdPlanoContabil = idPlanoContabil,
            TipoContaCaixa = (TipoContaCaixa?)idTipoContaCaixa,
        };

        foreach (var idUsuario in usuarios ?? [])
            caixa.AdicionarUsuario(new CaixaBancoUsuario { IdCaixaBanco = idCaixaBanco, IdFilial = idFilial, IdUsuario = idUsuario });

        return caixa;
    }

    public static ContaBancaria ParaEntidade(int idCaixaBanco, int idFilial, AtualizarContaBancariaDto x) => new()
    {
        IdCaixaBanco = idCaixaBanco,
        IdFilial = idFilial,
        IdAgencia = x.IdAgencia,
        Titular = x.Titular,
        NumeroConta = x.NumeroConta,
        DigitoConta = x.DigitoConta,
        Limite = x.Limite,
        CreditoPendente = x.CreditoPendente,
        DebitoPendente = x.DebitoPendente,
        ChequePendente = x.ChequePendente,
        ContaTerceiro = x.ContaTerceiro,
        PermiteEmitirCheque = x.PermiteEmitirCheque,
        IdContaBancariaVinculada = x.IdContaBancariaVinculada,
        ContaBancariaTipo = (TipoContaBancaria)x.IdTipoContaBancaria,
        IdInstituicaoFinanceira = x.IdInstituicaoFinanceira,
        EnviarSped = x.EnviarSped,
        CpfCnpj = x.CpfCnpj,
    };

    public static Cobrador ParaEntidade(int idCobrador, int idFilial, SalvarCobradorDto x) => new()
    {
        IdCobrador = idCobrador,
        IdFilial = idFilial,
        IdEntidade = x.IdEntidade,
        Nome = x.Nome,
        Ativo = x.Ativo,
        IdUsuario = x.IdUsuario,
        IdMeioContato = x.IdMeioContato,
    };
}
