namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/Consultas/DominioUsuarioSuprimentoSangriaConsulta.cs
// View: VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA (12 colunas) — somente leitura, sem chave.
// Mapa e definição da view: analysis/E2-dominio.md §2.8.
//
// Hierarquia achatada usuário × domínio × pai/filho — base das regras de hierarquia
// VAL-E2-43..46 (decisão D1) e da central do domínio (OP-E2-17).
public class DominioUsuarioSuprimentoSangria
{
    public int IdFilial { get; set; }

    /// <summary>Domínio de quem consulta (IDDOMINIO).</summary>
    public int IdDominio { get; set; }

    public bool DominioPaiPrincipal { get; set; }

    /// <summary>Nulo na linha do próprio domínio; preenchido nas linhas pai↔filho.</summary>
    public int? IdDominioFilho { get; set; }

    public int IdDominioRetorno { get; set; }
    public string? DescricaoRetorno { get; set; }

    /// <summary>Usuário retornado (IDDOMINIOUSUARIORETORNO).</summary>
    public int IdUsuarioRetorno { get; set; }

    public bool UsuarioPrincipalDominio { get; set; }
    public string NomeUsuarioRetorno { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public bool DominioPai { get; set; }

    /// <summary>1 = linha de caixa (próprio ou filho); 0 = linha do pai visto pelo filho.</summary>
    public bool Caixa { get; set; }
}
