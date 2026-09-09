using FluentAssertions;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Domain.Bases;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// VAL-E1-29 / VAL-E1-30 (matriz-rtv.md#E1) — regras puras de "pode digitar quantidade /
/// valor" no fechamento de caixa, transcritas de <c>FechamentoCaixaBase.cs</c> (legado).
/// </summary>
public class FechamentoCaixaRegrasTests
{
    private sealed class Servico : FechamentoCaixaService
    {
        public static bool PodeQtde(FechamentoCaixaBase l, bool editaColuna) => PermiteDigitarQtde(l, editaColuna);
        public static bool PodeValor(FechamentoCaixaBase l, bool editaColuna) => PermiteDigitarValor(l, editaColuna);
        public static bool EditaChequeRecebido(FechamentoCaixaBase l) => PermiteEditarChequeRecebido(l);
        public static bool EditaColuna(FechamentoCaixaBase l, bool temMoeda, bool editaCheque) => PermiteEditarColuna(l, temMoeda, editaCheque);
        public static ValidationResult Validar(FechamentoCaixaBase l, bool editaColuna, bool alterandoQtde) => ValidarEdicao(l, editaColuna, alterandoQtde);

        protected override System.Threading.Tasks.Task CalcularQtdeValorEditadoAsync(System.Threading.CancellationToken ct) => throw new NotSupportedException();
    }

    private static FechamentoCaixaBase Linha(FormaPagtoTipo forma, bool pdv = false) => new Fake { TipoFormaPagto = forma, FechamentoPdv = pdv };
    private sealed class Fake : FechamentoCaixaBase { }

    // VAL-E1-29 — Quantidade
    [Fact]
    public void VAL_E1_29_bloqueia_quantidade_quando_edita_coluna()
        => Servico.PodeQtde(Linha(FormaPagtoTipo.Dinheiro), editaColuna: true).Should().BeFalse();

    [Fact]
    public void VAL_E1_29_bloqueia_quantidade_para_cheque_cliente_em_pdv()
        => Servico.PodeQtde(Linha(FormaPagtoTipo.ChequeCliente, pdv: true), editaColuna: false).Should().BeFalse();

    [Fact]
    public void VAL_E1_29_permite_quantidade_no_caso_geral()
        => Servico.PodeQtde(Linha(FormaPagtoTipo.Deposito), editaColuna: false).Should().BeTrue();

    // VAL-E1-30 — Valor
    [Fact]
    public void VAL_E1_30_bloqueia_valor_quando_edita_coluna()
        => Servico.PodeValor(Linha(FormaPagtoTipo.Dinheiro), editaColuna: true).Should().BeFalse();

    [Fact]
    public void VAL_E1_30_bloqueia_valor_para_cheque_empresa()
        => Servico.PodeValor(Linha(FormaPagtoTipo.ChequeEmpresa), editaColuna: false).Should().BeFalse();

    [Fact]
    public void VAL_E1_30_permite_valor_no_caso_geral()
        => Servico.PodeValor(Linha(FormaPagtoTipo.Deposito), editaColuna: false).Should().BeTrue();

    // Regras de apoio
    [Fact]
    public void PermiteEditarChequeRecebido_so_para_cheque_cliente_fora_do_pdv()
    {
        Servico.EditaChequeRecebido(Linha(FormaPagtoTipo.ChequeCliente)).Should().BeTrue();
        Servico.EditaChequeRecebido(Linha(FormaPagtoTipo.ChequeCliente, pdv: true)).Should().BeFalse();
        Servico.EditaChequeRecebido(Linha(FormaPagtoTipo.Dinheiro)).Should().BeFalse();
    }

    [Fact]
    public void PermiteEditarColuna_dinheiro_depende_de_moeda_de_contagem()
    {
        Servico.EditaColuna(Linha(FormaPagtoTipo.Dinheiro), temMoeda: true, editaCheque: false).Should().BeTrue();
        Servico.EditaColuna(Linha(FormaPagtoTipo.Dinheiro), temMoeda: false, editaCheque: false).Should().BeFalse();
    }

    [Fact]
    public void ValidarEdicao_devolve_erro_quando_bloqueado()
    {
        var r = Servico.Validar(Linha(FormaPagtoTipo.ChequeEmpresa), editaColuna: false, alterandoQtde: false);
        r.IsValid.Should().BeFalse();
        r.Errors.Should().ContainSingle(e => e.Campo == nameof(FechamentoCaixaBase.ValorInformado));
    }
}
