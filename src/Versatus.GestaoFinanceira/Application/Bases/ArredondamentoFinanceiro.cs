namespace Versatus.GestaoFinanceira.Application.Bases;

/// <summary>
/// Arredondamento monetário do módulo — equivalente a
/// <c>Projeto.Geral.Funcoes.Arredondar(double, int)</c> do legado
/// (<c>projeto_tag_1906/geral/Funcoes.cs:712</c>).
///
/// Algoritmo legado: trabalha sobre o valor absoluto, faz
/// <c>truncar(|v| * 10^casas + 0,5) / 10^casas</c> e restaura o sinal — arredonda
/// **0,5 para longe do zero**. O legado usa passo intermediário em <c>double</c>; aqui é
/// tudo em <c>decimal</c> (constituição — dinheiro é <c>decimal</c>).
///
/// PARIDADE: E1-T04 (golden) confirma ausência de divergência com o legado. Se aparecer
/// caso de borda dependente do <c>double</c>, aquela tarefa decide (replicar o
/// intermediário <c>double</c> ou aceitar o <c>decimal</c>).
/// </summary>
public static class ArredondamentoFinanceiro
{
    public static decimal Arredondar(decimal valor, int casas)
    {
        if (valor == 0m)
            return 0m;

        bool negativo = valor < 0m;
        decimal v = Math.Abs(valor);

        decimal fator = 1m;
        for (int i = 0; i < casas; i++)
            fator *= 10m;

        decimal resultado = Math.Truncate(v * fator + 0.5m) / fator;
        return negativo ? -resultado : resultado;
    }
}
