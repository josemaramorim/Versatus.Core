namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa as informações de um Banco no sistema.
/// Origem: servidor/objeto de negócio/acesso.global/Banco.cs (legado)
/// Tabela: GloBanco
/// </summary>
public class Banco
{
    public int IdBanco { get; set; }
    
    /// <summary>
    /// Código do banco para cobrança. Mapeia para CodigoBancoCobranca no legado.
    /// </summary>
    public int Codigo { get; set; }

    /// <summary>
    /// Nome/Descrição do banco. Mapeia para Descricao no legado.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
