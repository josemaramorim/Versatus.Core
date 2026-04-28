using Versatus.NFe.Domain.Entities;

namespace Versatus.NFe.Application.Services;

public interface IEmissaoNFeService
{
    Task<Result<NFeDocumento>> EmitirNFeAsync(NFeDocumento documento);
    Task<Result<NFeDocumento>> ConsultarNFeAsync(string chaveAcesso);
    Task<Result<bool>> CancelarNFeAsync(string chaveAcesso, string justificativa);
}

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
