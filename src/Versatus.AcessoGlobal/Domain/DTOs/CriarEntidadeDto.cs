namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CriarEntidadeDto
{
    public string Nome { get; init; } = string.Empty;
    public string TipoPessoa { get; init; } = "F";
    public string? Cpf { get; init; }
    public string? Cnpj { get; init; }
    public string? RazaoSocial { get; init; }
}
