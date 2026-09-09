using Versatus.GestaoFinanceira.Domain.Bases;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/ItemFinanceiroBase.cs (legado) —
// região "Métodos privados e protegidos" (cálculo). Transcrito SEM refatorar (Regra 5).
// Cobre: CALC-E1-01..06, CALC-E1-10 (matriz-rot.md#E1). Golden tests em E1-T04.
//
// Cálculo puro: sem DbContext, sem IContextoExecucao, sem I/O. A conversão por índice
// econômico (CALC-E1-05) depende de um serviço externo (E5 — IndiceConversor); aqui é o
// gancho <see cref="ConverterIndice"/>, que por padrão devolve o valor sem conversão.
public abstract class CalculadoraItemFinanceiroBase
{
    /// <summary>
    /// CALC-E1-01 — executa o cálculo definido pelo <see cref="CalculoItemFinanceiro"/>.
    /// </summary>
    protected static decimal ExecutarCalculo(decimal valorBase, decimal valor, CalculoItemFinanceiro calculo)
    {
        if (calculo is CalculoItemFinanceiro.Somar or CalculoItemFinanceiro.Subtrair)
            return ArredondamentoFinanceiro.Arredondar(valorBase * valor / 100m, 2);

        if (calculo is CalculoItemFinanceiro.MultiplicarSomar or CalculoItemFinanceiro.MultiplicarDiminuir)
            return valorBase * Math.Abs(valor);

        if (calculo is CalculoItemFinanceiro.DividirSomar or CalculoItemFinanceiro.DividirSubtrair)
            return valorBase / Math.Abs(valor);

        if (calculo is CalculoItemFinanceiro.PercentualSomar or CalculoItemFinanceiro.PercentualSubtrair)
            return valorBase * (Math.Abs(valor) / 100m);

        return 0m;
    }

    /// <summary>
    /// CALC-E1-03 — nº de dias entre duas datas conforme <see cref="ItemFinanceiroAplicar"/>.
    /// </summary>
    protected static int CalcularDias(DateTime dataInicial, DateTime dataFinal, ItemFinanceiroAplicar aplicar) => aplicar switch
    {
        ItemFinanceiroAplicar.AntesVencimento => (dataInicial - dataFinal).Days,
        ItemFinanceiroAplicar.DepoisVencimento => (dataFinal - dataInicial).Days,
        _ => 0
    };

    /// <summary>
    /// CALC-E1-02 — cálculo composto por blocos de 30 dias.
    /// </summary>
    protected static decimal CalcularComposto(decimal valorBase, decimal valor, int dias, CalculoItemFinanceiro calculo)
    {
        decimal vb = valorBase;
        int d = dias;
        decimal total = 0m;

        while (d > 30)
        {
            decimal vc = ExecutarCalculo(vb, valor, calculo);
            total += vc;
            vb += vc;
            d -= 30;
        }

        if (d > 0)
        {
            decimal vc = ExecutarCalculo(vb, valor, calculo);
            total += vc / 30m * d;
        }

        return total;
    }

    /// <summary>
    /// CALC-E1-04 — divide o valor pela proporção de 30 dias quando o item não é composto e
    /// considera dias no cálculo. Sobrecarga sem <paramref name="dias"/> só divide por 30.
    /// </summary>
    protected static decimal ConsiderarDiasParaCalculo(decimal valorCalculo, bool composto, bool consideraDiasCalculo)
        => (!composto && consideraDiasCalculo) ? valorCalculo / 30m : valorCalculo;

    /// <inheritdoc cref="ConsiderarDiasParaCalculo(decimal,bool,bool)"/>
    protected static decimal ConsiderarDiasParaCalculo(decimal valorCalculo, bool composto, bool consideraDiasCalculo, int dias)
        => (!composto && consideraDiasCalculo) ? valorCalculo / 30m * dias : valorCalculo;

    /// <summary>
    /// CALC-E1-06 — orquestra o valor final do item financeiro:
    /// desconto inverte o sinal → composto (com dias) → considera dias → converte por índice.
    /// </summary>
    protected decimal DefinirValor(decimal valorBase, decimal valorCalculo, int dias, DateTime dataIndice,
        bool desconto, bool composto, bool consideraDiasCalculo, CalculoItemFinanceiro calculo,
        int? idIndiceEconomico)
    {
        if (desconto)
            valorCalculo = -valorCalculo;

        if (composto && dias > 0)
            valorCalculo = CalcularComposto(valorBase, valorCalculo, dias, calculo);

        valorCalculo = ConsiderarDiasParaCalculo(valorCalculo, composto, consideraDiasCalculo, dias);

        return RetornarIndiceConvertido(valorCalculo, dataIndice, calculo, idIndiceEconomico);
    }

    /// <summary>
    /// CALC-E1-05 — não converte para cálculos percentuais nem quando o índice é nulo/padrão;
    /// caso contrário delega a <see cref="ConverterIndice"/>.
    /// </summary>
    protected decimal RetornarIndiceConvertido(decimal valor, DateTime dataIndice,
        CalculoItemFinanceiro calculo, int? idIndiceEconomico)
    {
        if (calculo is CalculoItemFinanceiro.PercentualSomar or CalculoItemFinanceiro.PercentualSubtrair
            || idIndiceEconomico is null or 0)
            return valor;

        return ConverterIndice(valor, idIndiceEconomico.Value, dataIndice);
    }

    /// <summary>
    /// Gancho de conversão por índice econômico (RN-05-008). Por padrão devolve o valor sem
    /// conversão; o serviço concreto do épico E5 fornece a conversão real (IndiceConversor).
    /// </summary>
    protected virtual decimal ConverterIndice(decimal valor, int idIndiceEconomico, DateTime dataIndice) => valor;

}
