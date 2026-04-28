namespace Versatus.NFe.Domain.Entities;

public class NFeDocumento
{
    public Guid Id { get; set; }
    public int Serie { get; set; }
    public int Numero { get; set; }
    public DateTime DataEmissao { get; set; }
    public string ChaveAcesso { get; set; } = string.Empty;
    public Emitente Emitente { get; set; } = new();
    public Destinatario Destinatario { get; set; } = new();
    public List<ItemNFe> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "Pendente";
}

public class Emitente
{
    public string CNPJ { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    // outros campos
}

public class Destinatario
{
    public string CNPJCPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    // outros campos
}

public class ItemNFe
{
    public int NumeroItem { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    // tributos
}
