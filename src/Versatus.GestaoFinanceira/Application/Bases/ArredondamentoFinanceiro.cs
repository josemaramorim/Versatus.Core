namespace Versatus.GestaoFinanceira.Application.Bases;

/// <summary>
/// Arredondamento monetário do módulo — <c>Projeto.Geral.Funcoes.Arredondar(double, int)</c> do legado
/// (<c>projeto_tag_1906/geral/Funcoes.cs:712-734</c>), transcrito LITERALMENTE.
///
/// Algoritmo legado: sobre o valor absoluto, <c>truncar(Convert.ToDecimal(|v| * 10^casas + 0,5)) / 10^casas</c>
/// e restaura o sinal — 0,5 para longe do zero.
///
/// CÁLCULO INTERNO EM <c>double</c>, como no legado (decisão do usuário em 2026-09-27, E3-T09 — até
/// então era uma versão em <c>decimal</c>, por convenção do E1). Motivo: o <c>Convert.ToDecimal(double)</c> guarda só 15 dígitos
/// significativos e o <c>double</c> tem ruído binário; com 8 casas (saldo de rateio) valores a partir
/// de 1 milhão perdem a 8ª casa no legado, e uma emulação só em <c>decimal</c> não reproduz todos os
/// casos (divergia em 1 de 279 golden). Entrada e saída continuam <c>decimal</c>; nenhum outro cálculo
/// do módulo usa <c>double</c>. Paridade: golden origem=legado do E1 e do E3
/// (<c>specs/modulos/MOD-05/golden/legado/GeradorGoldenE3.cs</c>).
/// </summary>
public static class ArredondamentoFinanceiro
{
    public static decimal Arredondar(decimal valor, int casas)
    {
        double d = (double)valor;
        if (d == 0)
            return 0m;

        // --- início da transcrição de Funcoes.Arredondar (sem alterar a matemática) ---
        string s = new string('0', casas);
        s = string.Format("1{0}", s);
        int dec = Convert.ToInt32(s);

        bool numNegativo = (d < 0);

        if (numNegativo)
            d = Math.Abs(d);

        double v = ((d * dec) + 0.5);
        decimal conv = Convert.ToDecimal(v);

        double result = (double)((Math.Truncate(conv)) / dec);
        if (numNegativo)
            result *= (-1);
        // --- fim da transcrição ---

        // O legado grava o double em coluna numeric; a volta para decimal usa a mesma conversão.
        return Convert.ToDecimal(result);
    }
}
