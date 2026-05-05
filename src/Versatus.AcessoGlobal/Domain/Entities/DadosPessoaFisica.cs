namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Informações complementares para Pessoa Física.
/// Origem: servidor/objeto de negócio/acesso.global/EntidadeFisica.cs (legado)
/// Tabela: GloEntidadeFisica
/// </summary>
public class DadosPessoaFisica
{
    public int IdEntidade { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string? Rg { get; set; }
    public string? OrgaoEmissorRg { get; set; }
    public DateTime? DataEmissaoRg { get; set; }
    public DateTime? DataNascimento { get; set; }
    public SexoTipo Sexo { get; set; }
    public EstadoCivilTipo EstadoCivil { get; set; }
    public bool FisicaTipoJuridica { get; set; }

    // Relacionamento reverso
    public Entidade? Entidade { get; set; }
}
