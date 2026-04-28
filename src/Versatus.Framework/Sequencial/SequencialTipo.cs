namespace Versatus.Framework.Sequencial;

/// <summary>
/// Define os escopos de geração de sequencial.
/// </summary>
public enum SequencialTipo
{
    /// <summary>
    /// Sequencial global para todo o sistema.
    /// </summary>
    Geral = 0,

    /// <summary>
    /// Sequencial isolado por Empresa.
    /// </summary>
    Empresa = 1,

    /// <summary>
    /// Sequencial isolado por Filial.
    /// </summary>
    Filial = 2
}
