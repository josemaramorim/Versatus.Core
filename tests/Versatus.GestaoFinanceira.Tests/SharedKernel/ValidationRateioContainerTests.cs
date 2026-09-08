using FluentAssertions;
using Versatus.Framework.Validation;
using Versatus.SharedKernel.Rateio;

namespace Versatus.GestaoFinanceira.Tests.SharedKernel;

/// <summary>
/// Testes do agregador de validação de rateio (<see cref="ValidationRateioContainer"/>,
/// criado em E0-T03).
/// </summary>
public class ValidationRateioContainerTests
{
    [Fact]
    public void Novo_container_e_valido_e_sem_mensagens()
    {
        var v = new ValidationRateioContainer();
        v.Valida.Should().BeTrue();
        v.Mensagens.Should().BeEmpty();
    }

    [Fact]
    public void Uma_dimensao_invalida_torna_o_conjunto_invalido()
    {
        var v = new ValidationRateioContainer
        {
            TipoCentroCusto = ValidationResult.Fail(new ValidationError("CentroCusto", "Soma inválida."))
        };

        v.Valida.Should().BeFalse();
        v.Mensagens.Should().Contain("Soma inválida.");
    }

    [Fact]
    public void Mensagens_saem_na_ordem_classe_centrocusto_projeto()
    {
        var v = new ValidationRateioContainer
        {
            TipoClasse = ValidationResult.Fail(new ValidationError("Classe", "erro-classe")),
            TipoProjeto = ValidationResult.Fail(new ValidationError("Projeto", "erro-projeto"))
        };

        var linhas = v.Mensagens.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        linhas.Should().Equal("erro-classe", "erro-projeto");
    }
}
