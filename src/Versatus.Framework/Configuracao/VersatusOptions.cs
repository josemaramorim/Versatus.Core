namespace Versatus.Framework.Configuracao;

/// <summary>
/// Configurações globais do Framework Versatus.
/// </summary>
public class VersatusOptions
{
    /// <summary>
    /// Tempo máximo de espera para transações de banco de dados (em segundos).
    /// </summary>
    public int TimeoutTransacao { get; set; } = 30;

    /// <summary>
    /// Número máximo de tentativas em caso de falha de conexão ou concorrência.
    /// </summary>
    public int MaximoTentativas { get; set; } = 3;
}
