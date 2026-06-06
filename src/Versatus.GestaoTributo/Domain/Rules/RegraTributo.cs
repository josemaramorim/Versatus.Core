using System;
using System.Collections.Generic;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.Rules;

public class RegraTributo
{
    public int IdRegraTributo { get; set; }
    public int? IdRegraTributoPai { get; set; }
    public int IdTributo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public int IdSinteticoAnalitico { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public RegraTributo? Pai { get; set; }
    
    private readonly List<RegraTributo> _filhos = [];
    public IReadOnlyList<RegraTributo> Filhos => _filhos.AsReadOnly();

    public TributoFiscal? Tributo { get; set; }

    private readonly List<RegraTributoConfiguracao> _configuracoes = [];
    public IReadOnlyList<RegraTributoConfiguracao> Configuracoes => _configuracoes.AsReadOnly();
}
