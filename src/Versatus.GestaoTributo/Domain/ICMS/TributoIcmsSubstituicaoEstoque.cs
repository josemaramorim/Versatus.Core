using System;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class TributoIcmsSubstituicaoEstoque
{
    public int IdTributoIcmsSubstituicaoEstoque { get; set; }
    public int IdFilial { get; set; }
    public int IdEstEstoque { get; set; }
    public int IdEmpresa { get; set; }
    public DateTime DataDocumento { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? BaseCalculo { get; set; }
    public decimal? Aliquota { get; set; }
    public decimal? ValorTributo { get; set; }
    public decimal? AliquotaFcp { get; set; }
    public decimal? ValorFcp { get; set; }
    public decimal? BaseCalculoSt { get; set; }
    public decimal? AliquotaSt { get; set; }
    public decimal? ValorSt { get; set; }
    public decimal? AliquotaFcpSt { get; set; }
    public decimal? ValorFcpSt { get; set; }
    public int IdOrigem { get; set; }
    public int IdProcessoOrigem { get; set; }
    public bool Monofasico { get; set; }
    public bool IcmsSubstituidoAnterior { get; set; }

    // Auditoria (Inclusão é não-nula, Alteração é nula)
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
