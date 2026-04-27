using System;

namespace Versatus.Framework.Excecoes;

/// <summary>
/// Exceção base para toda a hierarquia de exceções personalizadas do sistema Versatus.
/// Nenhuma exceção de negócio deve herdar diretamente de System.Exception.
/// </summary>
public abstract class VersatusException : Exception
{
    /// <summary>
    /// Código de erro específico para categorização da falha.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Momento exato em que a exceção foi capturada ou criada.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    protected VersatusException() 
        : base() 
    { 
    }

    protected VersatusException(string message) 
        : base(message) 
    { 
    }

    protected VersatusException(string message, Exception innerException) 
        : base(message, innerException) 
    { 
    }

    protected VersatusException(string format, params object[] args)
        : base(string.Format(format, args))
    {
    }
}
