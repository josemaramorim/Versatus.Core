using System.Collections.Generic;

namespace Versatus.Framework.Validation;

/// <summary>
/// Representa o resultado de uma operação que pode retornar um valor ou uma lista de erros de validação.
/// Implementado como um record selado para garantir imutabilidade, conforme BP-001.
/// </summary>
/// <typeparam name="T">Tipo do valor retornado em caso de sucesso.</typeparam>
public sealed record Result<T>(bool IsSuccess, T? Value, IReadOnlyList<ValidationError> Errors)
{
    /// <summary>
    /// Retorna um resultado de sucesso contendo o valor produzido.
    /// </summary>
    public static Result<T> Ok(T value) => new(true, value, []);

    /// <summary>
    /// Retorna um resultado de falha contendo a lista de erros detectados.
    /// </summary>
    public static Result<T> Fail(params ValidationError[] errors) => new(false, default, errors);
}
