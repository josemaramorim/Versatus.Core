using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.Services;

public class EnderecoService : IEnderecoService
{
    public Task ValidarEnderecoAsync(EntidadeEndereco endereco, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endereco.Logradouro))
            throw new ArgumentException("O logradouro é obrigatório.");

        if (endereco.Numero <= 0)
            throw new ArgumentException("O número deve ser maior que zero.");

        if (endereco.Cep?.Length != 8)
            throw new ArgumentException("O CEP deve conter exatamente 8 dígitos.");

        if (endereco.IdCidade <= 0)
            throw new ArgumentException("A cidade é obrigatória.");

        return Task.CompletedTask;
    }
}
