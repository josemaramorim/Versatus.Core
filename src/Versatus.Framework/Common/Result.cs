namespace Versatus.Framework.Common;

/// <summary>
/// Resultado de operações, substitui exceções para controle de fluxo.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public ValidationResult? ValidationErrors { get; private set; }

    private Result(bool isSuccess, T? value, string? error, ValidationResult? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string error) => new(false, default, error, null);
    public static Result<T> ValidationFailure(ValidationResult errors) => new(false, default, null, errors);
}

/// <summary>
/// Resultado de validação, compatível com FluentValidation.
/// </summary>
public class ValidationResult
{
    public List<ValidationError> Errors { get; } = new();

    public bool IsValid => !Errors.Any();

    public void AddError(string property, string message)
    {
        Errors.Add(new ValidationError(property, message));
    }
}

public class ValidationError
{
    public string Property { get; }
    public string Message { get; }

    public ValidationError(string property, string message)
    {
        Property = property;
        Message = message;
    }
}