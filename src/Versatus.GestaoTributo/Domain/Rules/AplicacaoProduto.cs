using System;
using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.Rules;

public class AplicacaoProduto
{
    public int IdAplicacaoProduto { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int IdSinteticoAnalitico { get; set; }
    public int? IdAplicacaoProdutoPai { get; set; }
    public bool MovimentaEstoque { get; set; } = true;
    public bool Ativo { get; set; } = true;
    public string? SufixoCFOP { get; set; }
    public bool SubstituicaoICMS { get; set; }
    public int? IdTipoNaturezaOperacao { get; set; }
    public bool AplicarSubstituicaoExterna { get; set; }
    public bool UsaRegraTributoIvaProduto { get; set; }
    public int? IdRegraTributoIva { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public AplicacaoProduto? Pai { get; set; }
    
    private readonly List<AplicacaoProduto> _filhos = [];
    public IReadOnlyList<AplicacaoProduto> Filhos => _filhos.AsReadOnly();

    public RegraTributo? RegraTributoIva { get; set; }

    private readonly List<AplicacaoProdutoTributo> _tributos = [];
    public IReadOnlyList<AplicacaoProdutoTributo> Tributos => _tributos.AsReadOnly();

    private readonly List<AplicacaoEspecial> _especiais = [];
    public IReadOnlyList<AplicacaoEspecial> Especiais => _especiais.AsReadOnly();

    private readonly List<AplicacaoNaturezaOperacao> _naturezas = [];
    public IReadOnlyList<AplicacaoNaturezaOperacao> Naturezas => _naturezas.AsReadOnly();
}
