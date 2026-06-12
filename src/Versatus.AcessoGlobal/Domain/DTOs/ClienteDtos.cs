using System;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CriarClienteDto
{
    public int? IdEntidade { get; init; }
    public CriarEntidadeDto? NovaEntidade { get; init; }
    public string? LocalTrabalho { get; init; }
    public string? TelefoneTrabalho { get; init; }
    public string? Profissao { get; init; }
    public string? InscricaoProdutor { get; init; }
    public string? CodigoAlternativo { get; init; }
    public bool Ativo { get; init; } = true;
    public bool? Bloqueado { get; init; }
    public bool? ItemFinanceiroPadrao { get; init; } = true;
    public bool EnviarCNDNFe { get; init; }
    public decimal? RendaMensal { get; init; }
    public decimal? LimiteCredito { get; init; }
    public decimal? ValorAluguel { get; init; }
    public DateTime? DataAdmissao { get; init; }
    public DateTime? HoraCobranca { get; init; }
    public TipoImovel ImovelTipo { get; init; } = TipoImovel.Proprio;
    public SituacaoClienteSPC SituacaoSPC { get; init; } = SituacaoClienteSPC.Normal;
    public int? IdCategoria { get; init; }
    public int? IdClienteConceito { get; init; }
    public int? IdDiaSemanaCobranca { get; init; }
}

public record ClienteResponseDto(
    int IdCliente,
    string Nome,
    string? CodigoAlternativo,
    decimal? LimiteCredito,
    bool Ativo,
    string SituacaoSPC
);
