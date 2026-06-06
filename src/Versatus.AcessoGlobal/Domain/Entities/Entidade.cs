using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Entidade base que representa uma Pessoa Física ou Jurídica e seus diversos papéis no sistema.
/// Origem: servidor/objeto de negócio/acesso.global/Entidade.cs (legado)
/// Tabela: GloEntidade
/// </summary>
public class Entidade
{
    public int IdEntidade { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? EmailNFE { get; set; }
    public string? EmailFinanceiro { get; set; }
    public string? EmailVenda { get; set; }
    public string? EmailCompra { get; set; }
    public string? HomePage { get; set; }
    public string? Observacao { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? InscricaoMunicipal { get; set; }
    public string? InscricaoSuframa { get; set; }
    public bool Ativo { get; set; } = true;

    // Papéis (Roles) - Mantendo padrão booleano do legado
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public bool IsTransportadora { get; set; }
    public bool IsComissionado { get; set; }
    public bool IsAgenciaBancaria { get; set; }
    public bool IsInstituicaoFinanceira { get; set; }
    public bool IsFilial { get; set; }
    public bool IsFuncionario { get; set; }
    public bool IsObra { get; set; }
    public bool IsRepresentante { get; set; }
    public bool IsOutro { get; set; }
    public bool IsProspecto { get; set; }
    public bool IsContador { get; set; }
    public bool IsAluno { get; set; }
    public bool IsProfessor { get; set; }
    public bool IsIntermediadorComercial { get; set; }

    public EntidadeTipoPessoa TipoPessoa { get; set; }
    public IndicadorContribuinteICMS ContribuinteICMS { get; set; }
    public StatusCnpjCpf StatusCnpjCpf { get; set; }
    public int IdTipoPlataforma { get; set; } = 1309;

    // Relacionamentos 1:1 (Complementares)
    public DadosPessoaFisica? PessoaFisica { get; set; }
    public DadosPessoaJuridica? PessoaJuridica { get; set; }

    // Relacionamentos 1:N
    private readonly List<EntidadeEndereco> _enderecos = [];
    public IReadOnlyList<EntidadeEndereco> Enderecos => _enderecos.AsReadOnly();

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    public void AdicionarEndereco(EntidadeEndereco endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco);
        _enderecos.Add(endereco);
    }
}
