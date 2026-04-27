namespace Versatus.Framework.Validation;

/// <summary>
/// Representa um erro estruturado de validação ou regra de negócio.
/// Utilizado para retornar falhas esperadas ao invés de usar o fluxo de exceções.
/// </summary>
public record ValidationError(string Campo, string Mensagem);
