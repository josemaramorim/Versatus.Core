---
name: legacy-calc-parity
description: Extrai as fórmulas de cálculo financeiro/fiscal do legado (juros, multa, desconto, conversão por índice, rateio proporcional, arredondamento) e gera golden tests de paridade numérica com valores capturados do sistema/banco legado. Use em qualquer épico que envolva cálculo monetário para provar fidelidade matemática.
---

# Skill: legacy-calc-parity (Paridade de Cálculo Legado)

## 📌 Objetivo

"Conversão fiel" de módulo financeiro/fiscal só é verificável se os **números batem**.
Esta skill isola cada fórmula do legado, documenta seus parâmetros e regras de
arredondamento, e produz **golden tests** que comparam o resultado do código novo com
valores reais do legado — centavo a centavo.

Alimenta as linhas de cálculo da **Matriz ROT** e as tarefas `parity` do `tasks.md`.

---

## 🔎 1. Identificar as fórmulas

Varrer o módulo (entidade + classes-pai + `View/*Util.cs`, `Objeto negocio/*Util.cs`,
`IndiceConversor.cs`, helpers do `Geral`) por:

```
Calcular   Juros   Multa   Desconto   Acrescimo   Correcao
Math.Round   Round(   Truncate   decimal.Round   Arredond
* 100   / 100   ValorConvertido   FatorConversao   Indice
Rateio   Ratear   Proporcional   Distribuir   Residuo   Sobra
```

Para cada fórmula, uma ficha:

| Campo | Conteúdo |
| :--- | :--- |
| ID | `CALC-01` |
| Origem | `Documento.cs:CalcularJurosMulta():1180` |
| O que calcula | Juros + multa de parcela vencida |
| Entradas | valor da parcela, dias de atraso, `% juros a.m.`, `% multa`, parâmetros `GloParametro` (quais) |
| Fórmula legada (transcrita) | `juros = valor * (pctJurosMes/30/100) * diasAtraso; multa = valor * pctMulta/100` |
| Arredondamento | `Math.Round(x, 2, MidpointRounding.AwayFromZero)` — em que ponto e quantas casas |
| Ordem das operações | multa antes de juros? juros sobre valor ou sobre valor+multa? |
| Casos especiais | atraso 0, valor 0, parcela já parcialmente liquidada, feriado/dia útil |
| Precisão da coluna | `numeric(18,4)` / `numeric(15,2)` (de `data-model.md`) |

> **Regra 5 (não refatorar):** transcreva a fórmula **como está**, mesmo que pareça
> estranha (ex.: dividir por 30 fixo em vez de dias do mês). Divergência aparente vira
> `DÚVIDA:`, não "correção".

---

## 🎯 2. Capturar os golden values

Fonte, em ordem de preferência:

1. **Banco legado** (`localhost\SQLEXPRESS2008` / `versatus`): SELECT em registros reais já
   calculados (ex.: `FinMovimentoFinanceiro`, parcelas liquidadas) → entrada + resultado
   esperado.
2. **Execução do legado** com entradas controladas, quando disponível.
3. **Cálculo manual** conferido pelo usuário, quando 1 e 2 não forem possíveis (registrar
   que a origem é manual).

Montar `specs/modulos/MOD-XX/golden/CALC-01.md` (ou `.csv`) com ≥ 5 casos por fórmula,
incluindo os limites (0, atraso mínimo, valores altos, arredondamento no meio-centavo).

---

## 🧪 3. Gerar os testes

Para cada `CALC-xx`, esqueleto em
`tests/Versatus.<Modulo>.Tests/Parity/<Calc>ParityTests.cs`:

```csharp
// Origem: Documento.cs:CalcularJurosMulta() (legado) — CALC-01
public class JurosMultaParityTests
{
    public static IEnumerable<object[]> GoldenCases => LerGolden("golden/CALC-01.csv");

    [Theory]
    [MemberData(nameof(GoldenCases))]
    public void CalculoJurosMulta_BateComLegado(decimal valor, int diasAtraso,
        decimal pctJuros, decimal pctMulta, decimal esperadoJuros, decimal esperadoMulta)
    {
        var (juros, multa) = CalculadoraFinanceira.JurosMulta(valor, diasAtraso, pctJuros, pctMulta);
        Assert.Equal(esperadoJuros, juros);   // igualdade EXATA de decimal, sem tolerância
        Assert.Equal(esperadoMulta, multa);
    }
}
```

Regras:
- Comparar `decimal` com **igualdade exata** (sem `Assert.Equal(expected, actual, 2)` de
  tolerância — a casa decimal faz parte da paridade).
- Um arquivo de teste por fórmula; um caso por linha do golden.
- Se o novo cálculo diverge do golden, é **bug do novo**, não do golden — não ajuste o
  golden para passar.

---

## 📎 Saída

- Fichas `CALC-xx` na `matriz-rot.md` (seção Cálculos) + `golden/CALC-xx.*`.
- Esqueletos de teste em `tests/Versatus.<Modulo>.Tests/Parity/` (na fase de implementação).
- `sdd-analyze` reprova o módulo se houver `Calcular*` legado sem `CALC-xx` e sem golden.
