using System;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CriarTransportadoraDto
{
    public int? IdEntidade { get; init; }
    public CriarEntidadeDto? NovaEntidade { get; init; }
    public string? Rntrc { get; init; }
    public bool Ativo { get; init; } = true;
    public TipoProprietario ProprietarioTipo { get; init; } = TipoProprietario.Outros;
    public TipoTransportador TransportadorTipo { get; init; } = TipoTransportador.Nenhum;
    public int? IdCategoria { get; init; }
}

public record TransportadoraResponseDto(
    int IdTransportadora,
    string Nome,
    string? Rntrc,
    bool Ativo
);
