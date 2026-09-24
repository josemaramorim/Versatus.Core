using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/ContaBancaria.cs (legado, 1.704 linhas)
// Tabela: FINCONTABANCARIA (55 colunas) — PK (IDFINCAIXABANCO, IDGLOFILIAL), 1:1 com
// FINCAIXABANCO. Mapa: analysis/E3-caixa-banco.md §2.3.
//
// ESCOPO E3 = SÓ O NÚCLEO (12 colunas + 6 de auditoria), por decisão do usuário em
// 2026-09-24 (E3-T02). As demais colunas (carteira, nosso-número, remessa/retorno, boleto,
// SPED — lista em §2.3 "[E14]") entram nesta entidade no épico E14. Atenção: 6 delas são
// NOT NULL no banco (GERABOLETO, GERAREMESSA, PROCESSARETORNO, BOLETOBENEFICIARIODIFERENTE,
// BOLETOSACADOAVALISTA, ENVIARSPED) — o mapping do E3-T03 precisa garantir valor no INSERT.
//
// POCO SÓ DE DADOS (Artigo III). VAL-E3-02..07 / 17 → serviços (E3-T05).
public class ContaBancaria
{
    public int IdCaixaBanco { get; set; }
    public int IdFilial { get; set; }

    /// <summary>FK lógica para a agência (MOD-02) — IDGLOAGENCIA.</summary>
    public int IdAgencia { get; set; }

    public string? Titular { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public string? DigitoConta { get; set; }
    public decimal? Limite { get; set; }
    public decimal? CreditoPendente { get; set; }
    public decimal? DebitoPendente { get; set; }
    public decimal? ChequePendente { get; set; }
    public bool ContaTerceiro { get; set; }
    public bool PermiteEmitirCheque { get; set; }

    /// <summary>Conta de investimento vinculada (IDFINCONTABANCARIAVINCULADA) — VAL-E3-06/07.</summary>
    public int? IdContaBancariaVinculada { get; set; }

    /// <summary>Conta corrente ou investimento (IDTIPOCONTABANCARIA).</summary>
    public TipoContaBancaria ContaBancariaTipo { get; set; }

    /// <summary>FK lógica para a instituição financeira (MOD-02), usada no SPED — VAL-E3-02.</summary>
    public int? IdInstituicaoFinanceira { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
