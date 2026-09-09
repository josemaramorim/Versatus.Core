using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bases;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/FechamentoCaixaBase.cs (legado).
// Cobre: OP-E1-11 · VAL-E1-29, VAL-E1-30 (matriz-*.md#E1).
//
// E1-T03 = esqueleto + contratos. As regras de "pode digitar" são puras (operam sobre a
// POCO + um flag de edição de coluna) e ficam aqui; a máquina de estados do período
// (abrir/fechar/fechar tesouraria) e o enum de Situacao são do épico E2.
public abstract class FechamentoCaixaService
{
    /// <summary>Se a coluna abre a tela de complemento (contagem de dinheiro por moeda / cheques).</summary>
    protected static bool PermiteEditarColuna(FechamentoCaixaBase linha, bool temMoedaContagem, bool permiteEditarChequeRecebido)
        => linha.TipoFormaPagto == FormaPagtoTipo.Dinheiro ? temMoedaContagem : permiteEditarChequeRecebido;

    /// <summary>Se pode editar cheque recebido (forma = ChequeCliente e não é PDV).</summary>
    protected static bool PermiteEditarChequeRecebido(FechamentoCaixaBase linha)
        => linha.TipoFormaPagto == FormaPagtoTipo.ChequeCliente && !linha.FechamentoPdv;

    /// <summary>VAL-E1-29 — pode digitar QUANTIDADE se não edita a coluna de complemento e não é (PDV + ChequeCliente).</summary>
    protected static bool PermiteDigitarQtde(FechamentoCaixaBase linha, bool editaColuna)
    {
        if (editaColuna)
            return false;
        if (linha.FechamentoPdv && linha.TipoFormaPagto == FormaPagtoTipo.ChequeCliente)
            return false;
        return true;
    }

    /// <summary>VAL-E1-30 — pode digitar VALOR se não edita a coluna e a forma ≠ ChequeEmpresa.</summary>
    protected static bool PermiteDigitarValor(FechamentoCaixaBase linha, bool editaColuna)
    {
        if (editaColuna)
            return false;
        if (linha.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa)
            return false;
        return true;
    }

    /// <summary>VAL-E1-29/30 — valida a edição de quantidade/valor antes de aplicar.</summary>
    protected static ValidationResult ValidarEdicao(FechamentoCaixaBase linha, bool editaColuna, bool alterandoQtde)
    {
        bool permitido = alterandoQtde
            ? PermiteDigitarQtde(linha, editaColuna)
            : PermiteDigitarValor(linha, editaColuna);

        return permitido
            ? ValidationResult.Ok()
            : ValidationResult.Fail(new ValidationError(
                alterandoQtde ? nameof(FechamentoCaixaBase.Quantidade) : nameof(FechamentoCaixaBase.ValorInformado),
                "Esta coluna não pode ser editada para esta forma de pagamento."));
    }

    /// <summary>OP-E1-11 — acumula quantidade × valor por forma de pagamento no fechamento (contagem).</summary>
    protected abstract Task CalcularQtdeValorEditadoAsync(CancellationToken cancellationToken);
}
