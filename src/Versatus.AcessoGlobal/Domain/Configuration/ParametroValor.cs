namespace Versatus.AcessoGlobal.Domain.Configuration;

/// <summary>
/// Representa os valores configurados para os parâmetros do sistema.
/// Tabela: GloParametroValor
/// </summary>
public class ParametroValor
{
    public int IdParametroValor { get; set; }
    public int? IdParametro { get; set; }
    public int? IdFilial { get; set; }
    public int? IdPerfil { get; set; }
    public string? Valor { get; set; }
    public int? IdEmpresa { get; set; }
    public int? IdGrupo { get; set; }

    // Relacionamentos
    public Parametro? Parametro { get; set; }
}
