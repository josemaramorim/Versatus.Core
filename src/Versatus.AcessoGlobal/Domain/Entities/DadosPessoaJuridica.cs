namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Informações complementares para Pessoa Jurídica.
/// Origem: servidor/objeto de negócio/acesso.global/EntidadeJuridica.cs (legado)
/// Tabela: GloEntidadeJuridica
/// </summary>
public class DadosPessoaJuridica
{
    public int IdEntidade { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public RegimeTributarioTipo RegimeTributario { get; set; }
    public EnquadramentoTipo Enquadramento { get; set; }
    public int? IdCnaePrincipal { get; set; }

    // Relacionamento reverso
    public Entidade? Entidade { get; set; }
}
