using System;
using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.Classification;

public class Cfop
{
    public int IdCfop { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int? IdCfopPai { get; set; }
    public int IdSinteticoAnalitico { get; set; }
    public int? Sufixo { get; set; }
    public bool Ativo { get; set; } = true;
    public int IdSequencial { get; set; }
    public string? CodigoAcumuladorAVista { get; set; }
    public string? CodigoAcumuladorAPrazo { get; set; }
    public string? DescricaoCompleta { get; set; }
    public bool SubstituicaoIcms { get; set; }
    public int? IdTipoNaturezaOperacao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Cfop? Pai { get; set; }
    private readonly List<Cfop> _filhos = [];
    public IReadOnlyList<Cfop> Filhos => _filhos.AsReadOnly();
}
