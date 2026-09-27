namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta cross-módulo (MOD-02) para feriados — Feriado.EFeriado (legado): feriado fixo por
/// "dd/MM" (GLOFERIADO.DIAMES) ou feriado por data (GLOFERIADODATA.DATAFERIADO).
/// </summary>
public interface ICalendarioConsulta
{
    Task<bool> EhFeriadoAsync(DateTime data, CancellationToken cancellationToken = default);
}
