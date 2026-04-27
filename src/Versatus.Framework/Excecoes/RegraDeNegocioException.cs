using System;

namespace Versatus.Framework.Excecoes;

/// <summary>
/// Exceção utilizada para violações de regras de negócio esperadas do sistema.
/// Deve ser usada para erros de domínio previstos (ex: "Estoque insuficiente", "Cliente bloqueado").
/// </summary>
public class RegraDeNegocioException : VersatusException
{
    public RegraDeNegocioException(string message) 
        : base(message)
    {
        ErrorCode = "BUSINESS_RULE_VIOLATION";
    }

    public RegraDeNegocioException(string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = "BUSINESS_RULE_VIOLATION";
    }

    public RegraDeNegocioException(string format, params object[] args) 
        : base(format, args)
    {
        ErrorCode = "BUSINESS_RULE_VIOLATION";
    }
}
