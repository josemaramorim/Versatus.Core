using System.Collections.Generic;

namespace Versatus.Framework.Validation;

/// <summary>
/// Representa o resultado de uma operação de validação de forma estruturada.
/// Padrão obrigatório conforme DEC-005 para evitar exceções em regras de negócio verificáveis.
/// </summary>
public record ValidationResult(bool IsValid, IReadOnlyList<ValidationError> Errors)
{
    /// <summary>
    /// Retorna um resultado indicando sucesso, sem erros.
    /// </summary>
    public static ValidationResult Ok() => new(true, []);

    /// <summary>
    /// Retorna um resultado indicando falha, contendo os erros de validação detectados.
    /// </summary>
    public static ValidationResult Fail(params ValidationError[] errors) => new(false, errors);
}
