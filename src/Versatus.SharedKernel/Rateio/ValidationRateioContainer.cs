using System.Text;
using Versatus.Framework.Validation;

namespace Versatus.SharedKernel.Rateio;

/// <summary>
/// Agrega o resultado de validação das três dimensões de rateio (classe, centro de custo,
/// projeto).
/// Origem: <c>Projeto.Geral.ValidationRateioContainer</c> (legado). Modernizado: removido
/// <c>MarshalByRefObject</c> / <c>[Serializable]</c>; usa
/// <c>Versatus.Framework.Validation.ValidationResult</c>.
/// </summary>
public sealed class ValidationRateioContainer
{
    /// <summary>Validação da dimensão "classe".</summary>
    public ValidationResult TipoClasse { get; set; } = ValidationResult.Ok();

    /// <summary>Validação da dimensão "centro de custo".</summary>
    public ValidationResult TipoCentroCusto { get; set; } = ValidationResult.Ok();

    /// <summary>Validação da dimensão "projeto".</summary>
    public ValidationResult TipoProjeto { get; set; } = ValidationResult.Ok();

    /// <summary>True quando as três dimensões estão válidas.</summary>
    public bool Valida => TipoClasse.IsValid && TipoCentroCusto.IsValid && TipoProjeto.IsValid;

    /// <summary>
    /// Mensagens de erro das dimensões inválidas, uma por linha, na ordem
    /// classe → centro de custo → projeto.
    /// </summary>
    public string Mensagens
    {
        get
        {
            var sb = new StringBuilder();
            Acrescentar(sb, TipoClasse);
            Acrescentar(sb, TipoCentroCusto);
            Acrescentar(sb, TipoProjeto);
            return sb.ToString();

            static void Acrescentar(StringBuilder sb, ValidationResult resultado)
            {
                if (resultado.IsValid)
                    return;
                foreach (var erro in resultado.Errors)
                    sb.AppendLine(erro.Mensagem);
            }
        }
    }
}
