namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodo.cs (legado, ObjectMaster)
// Tabela: FINDOMINIOPERIODO (16 colunas) — PK (IDFINDOMINIOPERIODO, IDGLOFILIAL).
// Mapa: analysis/E2-dominio.md §2.2.
//
// POCO SÓ DE DADOS. O estado do período é derivado das datas: DataFechamento != null ⇔
// fechado (matriz-rot.md#E2, máquina de estados). Não há reabertura: abrir de novo cria outra
// linha. Os lançamentos (FINDOMINIOPERIODOLANCTO) não são agregado — têm origem e destino em
// períodos diferentes e são gravados pelo handler na mesma transação (OP-E2-03/04).
//
// As colunas DATAHORA* existem junto das DATA* por compatibilidade de relatórios (comentário do
// legado): o legado chama de "Hora" e grava nelas a hora do sistema. Mantidas as duas (Regra 4/5).
public class DominioPeriodo
{
    public int IdDominioPeriodo { get; set; }
    public int IdFilial { get; set; }
    public int IdDominio { get; set; }

    public int? IdUsuarioFechamento { get; set; }

    public DateTime DataAbertura { get; set; }

    /// <summary>Hora da abertura (coluna DATAHORAABERTURA).</summary>
    public DateTime HoraAbertura { get; set; }

    public DateTime? DataFechamento { get; set; }

    /// <summary>Hora do fechamento (coluna DATAHORAFECHAMENTO).</summary>
    public DateTime? HoraFechamento { get; set; }

    /// <summary>Preenchida no fechamento quando ControleCaixaTesouraria (OP-E2-05).</summary>
    public DateTime? DataFechamentoTesouraria { get; set; }

    /// <summary>Hora do fechamento da tesouraria (coluna DATAHORAFECHAMENTOTESOURARIA).</summary>
    public DateTime? HoraFechamentoTesouraria { get; set; }

    // Agregado: fechamento por forma de pagamento (FINDOMINIOPERIODOFECHAMENTO) — Artigo V.
    private readonly List<DominioPeriodoFechamento> _fechamentos = [];
    public IReadOnlyList<DominioPeriodoFechamento> Fechamentos => _fechamentos.AsReadOnly();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    public void AdicionarFechamento(DominioPeriodoFechamento fechamento)
    {
        ArgumentNullException.ThrowIfNull(fechamento);
        _fechamentos.Add(fechamento);
    }
}
