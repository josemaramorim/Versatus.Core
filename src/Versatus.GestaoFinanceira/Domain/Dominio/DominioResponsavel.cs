namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioResponsavel.cs (legado)
// Tabela: FINDOMINIORESPONSAVEL (10 colunas) — PK (IDFINDOMINIO, IDGLOFILIAL, IDFINDOMINIOPAI).
// Mapa: analysis/E2-dominio.md §2.7. Item do agregado Dominio (o domínio-filho).
//
// POCO SÓ DE DADOS. Hierarquia: IdDominioPai é o domínio responsável pelo IdDominio.
// VAL-E2-10..13 e VAL-E2-35..37 são do DominioService (E2-T04).
public class DominioResponsavel
{
    /// <summary>Domínio-filho (o domínio do cadastro).</summary>
    public int IdDominio { get; set; }

    public int IdFilial { get; set; }

    /// <summary>Domínio-pai responsável (IDFINDOMINIOPAI).</summary>
    public int IdDominioPai { get; set; }

    public bool DominioPaiPrincipal { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
