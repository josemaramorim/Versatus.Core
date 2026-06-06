using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Representa as informações específicas de uma Transportadora.
/// Origem: servidor/objeto de negócio/acesso.global/Transportadora.cs (legado)
/// Tabela: GloTransportadora
/// </summary>
public class Transportadora
{
    public int IdTransportadora { get; set; }
    
    public string? Rntrc { get; set; }
    public bool Ativo { get; set; } = true;
    
    public TipoProprietario ProprietarioTipo { get; set; }
    public TipoTransportador TransportadorTipo { get; set; }

    // FKs
    public int? IdCategoria { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Entidade? Entidade { get; set; }
    public Categoria? Categoria { get; set; }
}
