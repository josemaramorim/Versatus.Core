using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IEnderecoService
{
    /// <summary>
    /// Valida e formata um endereço antes da persistência.
    /// </summary>
    Task ValidarEnderecoAsync(EntidadeEndereco endereco, CancellationToken cancellationToken = default);
}
