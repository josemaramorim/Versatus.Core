namespace Versatus.AcessoGlobal.Domain.Organization;

/// <summary>
/// Entidade que representa uma Filial de uma Empresa.
/// Mapeada da tabela legada GloFilial.
/// </summary>
public class Filial
{
    /// <summary>
    /// Identificador único da filial (IdFilial).
    /// </summary>
    public int IdFilial { get; set; }

    /// <summary>
    /// Chave estrangeira para a Empresa à qual esta filial pertence.
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// Nome da Filial (obtido da Entidade vinculada no legado).
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da Filial (obtido da Entidade vinculada no legado).
    /// </summary>
    public string CNPJ { get; set; } = string.Empty;

    /// <summary>
    /// Inscrição Estadual (obtida da Entidade vinculada no legado).
    /// </summary>
    public string InscricaoEstadual { get; set; } = string.Empty;

    /// <summary>
    /// Define se a filial está ativa.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Logomarcas (armazenadas em byte[] no legado)
    public byte[]? Logomarca { get; set; }
    public byte[]? LogomarcaMedia { get; set; }
    public byte[]? LogomarcaGrande { get; set; }

    // Relacionamentos
    public Empresa? Empresa { get; set; }
    
    // Nota: O vínculo com Entidade e Endereço será implementado na Fase 5.
    // public int IdEntidade { get; set; } 
}
