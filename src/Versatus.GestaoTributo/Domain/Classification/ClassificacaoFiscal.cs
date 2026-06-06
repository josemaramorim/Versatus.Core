using System;
using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.Classification;

public class ClassificacaoFiscal
{
    public int IdClassificacaoFiscal { get; set; }
    public int? IdClassificacaoFiscalPai { get; set; }
    public int IdSinteticoAnalitico { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Ncm { get; set; }
    public string? Nbm { get; set; }
    public string? ExtIpi { get; set; }
    public string? GeneroIpi { get; set; } // Legado: GENEROTIPI -> GeneroIpi
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public string? CodigoAtividadeCP { get; set; }
    public int? IdTipoEscalaRelevante { get; set; }
    public DateTime? VigenciaFinalNcm { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public ClassificacaoFiscal? Pai { get; set; }
    private readonly List<ClassificacaoFiscal> _filhos = [];
    public IReadOnlyList<ClassificacaoFiscal> Filhos => _filhos.AsReadOnly();

    private readonly List<ClassificacaoFiscalTributo> _tributos = [];
    public IReadOnlyList<ClassificacaoFiscalTributo> Tributos => _tributos.AsReadOnly();
}
