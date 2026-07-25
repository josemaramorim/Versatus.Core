namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloModulo (banco de dados)
// Tabela: GloModulo
public class GloModulo
{
    public int IdModulo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public long? ChaveModulo { get; set; }
    public int TipoModulo { get; set; }
    public string? PrefixoRota { get; set; }
    public string? CorHex { get; set; }
    public string? IconeMui { get; set; }
}
