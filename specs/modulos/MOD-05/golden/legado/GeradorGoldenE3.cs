// Gerador dos golden values do épico E3 (tarefa E3-T09) a partir do CÓDIGO LEGADO.
//
// Compilar e executar com o compilador do .NET Framework 4 (mesmo runtime do legado, C# 5):
//   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /out:GeradorGoldenE3.exe GeradorGoldenE3.cs
//   GeradorGoldenE3.exe <pasta de saída>
//
// Cada método "Legado*" é cópia LITERAL do corpo do método legado indicado (projeto_tag_1906),
// com a matemática em double como no original. Só o ACESSO A DADOS foi trocado por parâmetros
// (lista de feriados, lista de valores do índice) — nenhuma fórmula foi alterada.
// Os CSV gerados vão para tests/Versatus.GestaoFinanceira.Tests/E3/golden/ com origem=legado.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public static class GeradorGoldenE3
{
    // ---- geral/Funcoes.cs:712-734 — Funcoes.Arredondar (cópia literal) -------------------
    static public double Arredondar(double d, int i)
    {
        if (d == 0)
            return d;

        string s = new String('0', i);
        s = String.Format("1{0}", s);
        int dec = Convert.ToInt32(s);

        bool numNegativo = (d < 0);

        if (numNegativo)
            d = Math.Abs(d);

        double valor = ((d * dec) + 0.5);
        decimal conv = Convert.ToDecimal(valor);

        double result = (double)((Math.Truncate(conv)) / dec);
        if (numNegativo)
            result *= (-1);

        return result;
    }

    // ---- SaldoCaixaBanco.cs:273 / :284 — getters Saldo e SaldoConciliado (CALC-E3-01/02) ----
    static double LegadoSaldo(double saldoAnterior, double totalCredito, double totalDebito)
    {
        return Arredondar((saldoAnterior + (totalCredito - totalDebito)), 2);
    }

    // ---- SaldoCaixaBanco.cs:304-317 — RetornarSaldoCaixaBanco (CALC-E3-03) ------------------
    // O registro selecionado (OP-E3-08) vem do banco; aqui chega como parâmetro (null = não achou).
    static double LegadoRetornarSaldoCaixaBanco(bool inicialSemData, double? saldo)
    {
        if (inicialSemData)
            return 0;
        if (saldo == null)
            return 0;
        return Arredondar(saldo.Value, 2);
    }

    // ---- SaldoRateio.cs:284 / :295 — SaldoEconomico e SaldoFinanceiro (CALC-E3-04) --------
    static double LegadoSaldoRateio(double saldoAnterior, double totalCredito, double totalDebito)
    {
        return Arredondar(saldoAnterior + (totalCredito - totalDebito), 8);
    }

    // ---- IndiceConversor.cs:41-73 — CalcularConversao (CALC-E3-05) --------------------------
    // valorIndiceOrigem/Destino já resolvidos (RetornarIndice); idP = índice padrão do sistema.
    static double LegadoCalcularConversao(int idO, int idD, int idP, double valorOriginal,
        double valorIndiceOrigem, double valorIndiceDestino, int decimais)
    {
        double valorConvertido;
        if (idO.Equals(idD))
        {
            valorConvertido = valorOriginal;
            return valorConvertido;
        }

        double valorOrigem = valorOriginal;

        if (!idO.Equals(idP) && !idD.Equals(idP))
        {
            valorConvertido = Arredondar(valorOrigem / valorIndiceOrigem, decimais);
            valorConvertido = Arredondar(valorConvertido * valorIndiceDestino, decimais);
        }
        else if (!idD.Equals(idP))
            valorConvertido = Arredondar(valorOrigem / valorIndiceDestino, decimais);
        else
            valorConvertido = Arredondar(valorOrigem * valorIndiceOrigem, decimais);

        return valorConvertido;
    }

    // ---- acesso.global/Feriado.cs:426-432 — DiaUtil + EFeriado:393-421 (feriado por DIAMES ou data) --
    static bool LegadoDiaUtil(DateTime d, ICollection<string> feriadosDiaMes, ICollection<DateTime> feriadosData)
    {
        if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
            return false;

        string diaMes = String.Format("{0:dd}", d) + "/" + String.Format("{0:MM}", d);
        bool eFeriado = feriadosDiaMes.Contains(diaMes) || feriadosData.Contains(d.Date);
        return !eFeriado;
    }

    // ---- IndiceConversor.cs:102-116 — RetornarDataIndiceValida (CALC-E3-06) -------------------
    const int UsarDataAnterior = 106; // Projeto.Geral.Enumerado.IndiceModoCorrecao.UsarDataAnterior

    static DateTime LegadoRetornarDataIndiceValida(DateTime d, int modo, ICollection<string> fDiaMes, ICollection<DateTime> fData)
    {
        if (LegadoDiaUtil(d, fDiaMes, fData))
            return d;

        int add = 1;
        if (modo == UsarDataAnterior)
            add = -1;

        d = d.AddDays(add);
        while (!LegadoDiaUtil(d, fDiaMes, fData))
            d = d.AddDays(add);

        return d;
    }

    // ---- IndiceConversor.cs:78-97 — RetornarIndice (CALC-E3-07) ------------------------------
    // Valores = lista carregada por IndiceEconomico.CarregarListaValorIndice (ordem de DATAINDICE);
    // lista vazia com tipo de correção definido vira itens com Valor 0 (AddItemEconomico).
    const int Diario = 109; // IndiceTipoCorrecao.Diario

    static string LegadoRetornarIndice(bool ehPadrao, int tipoCorrecao, DateTime d, double[] valores)
    {
        if (ehPadrao)
            return Fmt((double)1.00);

        int i = d.Month - 1;
        if (tipoCorrecao == Diario)
            i = d.Day - 1;

        if (valores.Length == 0 || valores[i] == 0)
            return "ERRO:DataSemIndiceEconomico";

        return Fmt(valores[i]);
    }

    // ---- saída ----------------------------------------------------------------------------------
    static string Fmt(double v)
    {
        // "R" = round-trip do double; convertido para decimal com 15 dígitos significativos, como
        // o próprio legado faz ao gravar numeric via Convert.ToDecimal.
        return Convert.ToDecimal(v).ToString(CultureInfo.InvariantCulture);
    }

    static double D(string s) { return double.Parse(s, CultureInfo.InvariantCulture); }

    static void Gravar(string pasta, string arquivo, StringBuilder sb)
    {
        File.WriteAllText(Path.Combine(pasta, arquivo), sb.ToString(), new UTF8Encoding(false));
        Console.WriteLine(arquivo);
    }

    public static void Main(string[] args)
    {
        string pasta = args.Length > 0 ? args[0] : ".";
        Directory.CreateDirectory(pasta);

        // CALC-E3-01/02 — saldo e saldo conciliado (mesma fórmula, 2 casas)
        string[][] saldos = {
            new[] {"simples", "1000", "250.35", "100.10"},
            new[] {"zero", "0", "0", "0"},
            new[] {"negativo", "-500.25", "100", "50.5"},
            new[] {"soma_0_1_0_2", "0.1", "0.2", "0"},
            new[] {"meio_centavo_2_675", "2.675", "0", "0"},
            new[] {"meio_centavo_1_005", "1.005", "0", "0"},
            new[] {"meio_centavo_1_015", "1.015", "0", "0"},
            new[] {"meio_centavo_negativo", "-2.675", "0", "0"},
            new[] {"oito_casas", "1234.56789012", "0.00000001", "0.00499999"},
            new[] {"grande", "99999999.995", "0", "0"},
            new[] {"debito_maior", "10.10", "0.05", "20.20"},
            new[] {"credito_debito_iguais", "100", "33.335", "33.335"},
        };
        StringBuilder sb = new StringBuilder("caso;saldoAnterior;totalCredito;totalDebito;esperado;origem\n");
        foreach (string[] c in saldos)
            sb.AppendFormat("{0};{1};{2};{3};{4};legado\n", c[0], c[1], c[2], c[3], Fmt(LegadoSaldo(D(c[1]), D(c[2]), D(c[3]))));
        Gravar(pasta, "CALC-E3-01-02-saldo.csv", sb);

        // CALC-E3-03 — retorno do saldo (inicial sem data / sem registro / arredondamento)
        sb = new StringBuilder("caso;inicialSemData;saldo;esperado;origem\n");
        string[][] retornos = {
            new[] {"inicial_sem_data", "true", "150.55"},
            new[] {"sem_registro", "false", ""},
            new[] {"com_registro", "false", "150.55"},
            new[] {"com_registro_arredonda", "false", "150.555"},
            new[] {"com_registro_negativo", "false", "-0.005"},
        };
        foreach (string[] c in retornos)
        {
            double? saldo = c[2] == "" ? (double?)null : D(c[2]);
            sb.AppendFormat("{0};{1};{2};{3};legado\n", c[0], c[1], c[2], Fmt(LegadoRetornarSaldoCaixaBanco(c[1] == "true", saldo)));
        }
        Gravar(pasta, "CALC-E3-03-retornar-saldo.csv", sb);

        // CALC-E3-04 — saldo de rateio (8 casas)
        string[][] rateios = {
            new[] {"simples", "100.12345678", "0.00000001", "0"},
            new[] {"zero", "0", "0", "0"},
            new[] {"soma_0_1_0_2", "0.1", "0.2", "0"},
            new[] {"negativo", "-0.00000001", "0", "0.00000001"},
            new[] {"grande", "123456789012.12345678", "0.00000001", "0"},
            new[] {"um_milhao", "1234567.12345678", "0.00000001", "0"},
            new[] {"dez_milhoes", "12345678.12345678", "0.00000001", "0"},
            new[] {"cem_milhoes", "123456789.12345678", "0.00000001", "0"},
            new[] {"um_milhao_empate_par", "1234567.12345678", "0", "0"},
            new[] {"um_milhao_empate_impar", "1234567.12345677", "0", "0"},
            new[] {"dez_milhoes_empate", "12345678.12345675", "0", "0"},
            new[] {"terco", "0.33333333", "0.33333333", "0.33333333"},
            new[] {"debito_maior", "5", "1.23456789", "10.98765432"},
        };
        sb = new StringBuilder("caso;saldoAnterior;totalCredito;totalDebito;esperado;origem\n");
        foreach (string[] c in rateios)
            sb.AppendFormat("{0};{1};{2};{3};{4};legado\n", c[0], c[1], c[2], c[3], Fmt(LegadoSaldoRateio(D(c[1]), D(c[2]), D(c[3]))));
        Gravar(pasta, "CALC-E3-04-saldo-rateio.csv", sb);

        // CALC-E3-05 — conversão (idP = 1: índice padrão, parâmetro INDICEECONOMICOPADRAO)
        string[][] conversoes = {
            // caso, idO, idD, valor, vO, vD, dec
            new[] {"mesmo_indice_sem_arredondar", "2", "2", "123.456", "3.5", "3.5", "2"},
            new[] {"entre_indices", "2", "3", "1000", "3.5", "4.2", "2"},
            new[] {"entre_indices_4_casas", "2", "3", "1000", "3.5", "4.2", "4"},
            new[] {"padrao_para_indice", "1", "3", "1000", "1", "5.4321", "2"},
            new[] {"indice_para_padrao", "2", "1", "100", "1.23456", "1", "2"},
            new[] {"indice_para_padrao_meio", "2", "1", "10.05", "1.5", "1", "2"},
            new[] {"entre_indices_dizima", "2", "3", "100", "3", "7", "2"},
            new[] {"negativo", "2", "1", "-100", "1.23456", "1", "2"},
            new[] {"zero", "2", "3", "0", "3.5", "4.2", "2"},
        };
        sb = new StringBuilder("caso;idOrigem;idDestino;valor;valorIndiceOrigem;valorIndiceDestino;decimais;esperado;origem\n");
        foreach (string[] c in conversoes)
        {
            double r = LegadoCalcularConversao(int.Parse(c[1]), int.Parse(c[2]), 1, D(c[3]), D(c[4]), D(c[5]), int.Parse(c[6]));
            sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};legado\n", c[0], c[1], c[2], c[3], c[4], c[5], c[6], Fmt(r));
        }
        Gravar(pasta, "CALC-E3-05-conversao.csv", sb);

        // CALC-E3-06 — data válida do índice. Feriados = GLOFERIADO do banco de dev (8 fixos) e
        // um feriado por data (GLOFERIADODATA) de exemplo: 2026-02-17 (Carnaval, terça).
        List<string> fDiaMes = new List<string> { "01/01", "21/04", "01/05", "07/09", "12/10", "02/11", "15/11", "25/12" };
        List<DateTime> fData = new List<DateTime> { new DateTime(2026, 2, 17) };
        string[][] datas = {
            // caso, data, modo
            new[] {"dia_util", "2026-09-23", "210"},
            new[] {"sabado_proximo", "2026-09-26", "210"},
            new[] {"sabado_anterior", "2026-09-26", "106"},
            new[] {"domingo_proximo", "2026-09-27", "107"},
            new[] {"natal_sexta_proximo", "2026-12-25", "210"},
            new[] {"natal_sexta_anterior", "2026-12-25", "106"},
            new[] {"ano_novo_quinta_anterior", "2026-01-01", "106"},
            new[] {"feriado_por_data", "2026-02-17", "210"},
            new[] {"feriado_por_data_anterior", "2026-02-17", "106"},
            new[] {"modo_nao_informado", "2026-11-15", "0"},
        };
        sb = new StringBuilder("caso;data;modo;esperado;origem\n");
        foreach (string[] c in datas)
        {
            DateTime d = DateTime.ParseExact(c[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime r = LegadoRetornarDataIndiceValida(d, int.Parse(c[2]), fDiaMes, fData);
            sb.AppendFormat("{0};{1};{2};{3:yyyy-MM-dd};legado\n", c[0], c[1], c[2], r);
        }
        Gravar(pasta, "CALC-E3-06-data-indice.csv", sb);

        // CALC-E3-07 — valor do índice na lista (posição = mês-1 ou dia-1)
        double[] mensal = { 1.01, 1.02, 1.03, 0, 1.05, 1.06, 1.07, 1.08, 1.09, 1.10, 1.11, 1.12 };
        double[] diario = new double[30];
        for (int k = 0; k < diario.Length; k++) diario[k] = 5 + (k + 1) / 1000.0;
        double[] vazio = new double[0];
        sb = new StringBuilder("caso;ehPadrao;tipoCorrecao;data;lista;esperado;origem\n");
        object[][] indices = {
            new object[] {"padrao", true, 110, "2026-03-15", "mensal", mensal},
            new object[] {"mensal_marco", false, 110, "2026-03-15", "mensal", mensal},
            new object[] {"mensal_dezembro", false, 110, "2026-12-31", "mensal", mensal},
            new object[] {"mensal_valor_zero", false, 110, "2026-04-10", "mensal", mensal},
            new object[] {"diario_dia_10", false, 109, "2026-09-10", "diario", diario},
            new object[] {"diario_dia_30", false, 109, "2026-09-30", "diario", diario},
            new object[] {"lista_vazia", false, 110, "2026-03-15", "vazia", vazio},
        };
        foreach (object[] c in indices)
        {
            DateTime d = DateTime.ParseExact((string)c[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string r = LegadoRetornarIndice((bool)c[1], (int)c[2], d, (double[])c[5]);
            sb.AppendFormat("{0};{1};{2};{3};{4};{5};legado\n", c[0], ((bool)c[1]) ? "true" : "false", c[2], c[3], c[4], r);
        }
        Gravar(pasta, "CALC-E3-07-valor-indice.csv", sb);
    }
}
