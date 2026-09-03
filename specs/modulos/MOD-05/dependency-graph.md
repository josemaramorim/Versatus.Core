# MOD-05 — Grafo de Dependências

> Companion de [`spec.md`](./spec.md) §4. Fonte: `using`/`ProjectReference` reais de
> `projeto_tag_1906/servidor/objeto de negócio/gestao.financeira/` +
> `gestao.financeira.csproj`.

---

## 1. Dependências de projeto (do `.csproj` legado)

`Gestao.Financeira.csproj` referencia:

| Referência legada | Novo equivalente | Tipo de vínculo |
| :--- | :--- | :--- |
| `Geral` | **`Versatus.SharedKernel`** (a criar — E0, escopo mínimo — CLR-02) | ProjectReference |
| `Servidor.Framework` (+ `Factory.cs` linkado) | `Versatus.Framework` | ProjectReference |
| `Servidor.Interface` | contratos redistribuídos por módulo | — |
| `Acesso.Global` | `Versatus.AcessoGlobal` | ProjectReference — bases de rateio (`RateioMovto`/`RateioMovtoItem`/`ManutencaoRateio`) migradas no MOD-02 antes do E5 (CLR-01) |
| `Servidor.Strangler` | `Servidor.Strangler` (legado, mantido) | só DTOs do AcessoGlobal — nada a reconciliar do lado financeiro (CLR-10) |
| `Versatus.Eval` | — | fora de escopo |
| `Versatus.Language` | estratégia de i18n | — |
| `BarcodeLib`, `Boleto.Net` | substituto .NET 10 (E14 — `research.md`) | ProjectReference futura |

Nenhuma `ProjectReference` para `Faturamento`, `GestaoContrato`, `GestaoFrota`,
`GestaoOS`, `GestaoTransporte`, `NFSe`, `GestaoContabil` no novo projeto — todos são
**`int` lógico**.

---

## 2. DAG de épicos

```
E0 SharedKernel
   │
E1 Bases (DocumentoFinanceiroBase, OperacaoDocumentoBase, ItemFinanceiroBase,
   │       ParcelaGeral/ParcelaBase, FormaMovInfo, FechamentoCaixaBase)
   │
   ├──────────────┐
E2 Domínio/Período │
   │              │
E3 Caixa e Banco ◄┘   (E3 usa E2: todo caixa/banco pertence a um domínio)
   │
   ├───────────────────────────┐
E4 Documento e Parcela         │
   │        │                  │
   │        │              E5 Movimento e Formas de Pagamento (usa E2, E3)
   │        │                  │
   │        └────────┬─────────┤
   │             E6 Liquidação (usa E4 + E5)
   │                  │
   │        ┌─────────┼──────────────┐
   │    E7 Estorno   E8 Reversão   (E8 usa E4 + E6)
   │        (usa E6)  │
   │                  │
E9 Cheques (usa E3 + E5) ─────────┐
E10 Adiantamentos/Acertos (usa E4 + E5)
E11 DRE / Projeção / SelecaoDocumento (usa E4 + E5)
E12 Prog. Cobrança / Transação entre Filiais (usa E4)
E13 Consultas (usa os épicos das entidades correspondentes)
E14 Integração Bancária — CNAB/boleto/OFX (usa E3 + E6 + E9)   ← fase dedicada final
```

Ordem de execução linear sugerida: **E0 → E1 → E2 → E3 → E4 → E5 → E6 → E7 → E8 → E9 →
E10 → E11 → E12 → E13 → E14**. E9/E10/E11/E12 podem ser paralelizados após E5+E6.

---

## 3. Contenção de agregados (intra-módulo)

```
Documento (FinDocumento)
├─ DocumentoParcela[]        (FinDocumentoParcela)        ← título; agregado próprio
│  ├─ DocumentoParcelaImage[](FinDocumentoParcelaImagem)
│  └─ DoctoItemFinanceiro[]  (FinDoctoItemFinanceiro)
│     └─ DoctoMovtoItemFinanceiro[]
├─ DocumentoMovto[]          (FinDocumentoMovto)
├─ DocumentoTributo[]        (FinDocumentoTributo)
├─ DocumentoComissionado[]   (FinDocumentoComissionado)
├─ DocumentoCartao?          (FinDocumentoCartao)
└─ rateio → MovtoFinanceiroRateio / ContraPartidaRateioDoctoFinanceiro

MovimentoFinanceiro (FinMovimento)
├─ FormaPagamentoMov[]                       (forma aplicada)
│  └─ FormaMovInfo (1:1 por tipo: Dinheiro/Cartao/Pix/ChequeCliente/ChequeEmpresa/
│                   Credito/Deposito/Abatimento/Crediario/Financeira/Outros)
├─ MovtoFinanceiroRateio[]                   (FinMovimentoRateio)
└─ MovtoFinanceiroCheque  (especialização: movimento que é cheque emitido)

Liquidacao  (opera E4+E5; grava)
├─ LiquidacaoFormaMovimento[]               (FinLiquidacaoFormaMovimento)
├─ MovimentoFinanceiro (gerado)
└─ baixa DocumentoParcela.ValorLiquidado + SaldoCaixaBanco

LiquidacaoEstorno (FinLiquidacaoEstorno)   ← inverso de Liquidacao
├─ LiquidacaoEstornoFormaPagto[]           (FinLiquidacaoEstornoFormaPagto)
└─ LiquidacaoEstornoItemFin[]

Reversao (FinReversao)
├─ ReversaoDoctoParcela[]                  (FinReversaoDoctoParcela)
├─ ReversaoItemFinanceiro[]               (FinReversaoItemFinanceiro)
└─ AplicacaoItemFinReversao

Dominio (FinDominio)
└─ DominioPeriodo[] (FinDominioPeriodo)
   ├─ DominioPeriodoFormaPagto[]
   ├─ DominioPeriodoLacto[]
   ├─ DominioPeriodoFechamento[] → DominioPeriodoFechamentoDetalhe[]
   ├─ DominioPeriodoLog[]
   ├─ DominioResponsavel[]
   └─ DominioUsuario[]

CaixaBanco (FinCaixaBanco)
├─ CaixaBancoUsuario[]
├─ SaldoCaixaBanco[]  (por data)
└─ ContaBancaria? (FinContaBancaria)  ← quando é banco
   └─ TalaoCheque[] (FinTalaoCheque)

ChequeRecebido (FinChequeRecebido)
└─ ChequeRecebidoMovto[] (FinChequeRecebidoMovto)   ← máquina de estados
ChequeEmitidoMovto[] (FinChequeEmitidoMovto)         ← ligado a TalaoCheque + MovtoFinanceiroCheque

Adiantamento (FinAdiantamento)
└─ AdtoAcerto (FinAdtoAcerto)
   ├─ AdtoAcertoDistribuicao[] (FinAdtoAcertoDistribuicao)
   │  └─ AdtoAcertoDistribuicaoRateio[]
   ├─ AdtoAcertoMovto[] (FinAdtoAcertoMovto)
   └─ AdtoLanctoEntidade[]

DRE (FinDRE)
└─ DRETitulo[] (FinDRETitulo)
   ├─ DRETituloClasse[] (FinDRETituloClasse)
   └─ DRETituloOperacao[] (FinDRETituloOperacao)

ProjecaoFluxoCaixa (FinProjecaoFluxoCaixa)
└─ ProjecaoFluxoCaixaLacto[] (FinProjecaoFluxoCaixaLacto)

ProgramacaoCobranca (FinProgramacaoCobranca)
└─ ProgramacaoCobrancaParcela[] (FinProgramacaoCobrancaParcela)

TransacaoFilial (FinTransacaoFilial)
├─ TransacaoFinanceira[] (FinTransacaoFinanceira)
└─ FilialMovimento[] (FinFilialMovto)
```

---

## 4. Referências cross-módulo por `int` lógico (sem navegação EF)

| Campo no MOD-05 | Aponta para | Módulo dono |
| :--- | :--- | :--- |
| `IdEntidade` | `GloEntidade` | MOD-02 AcessoGlobal |
| `IdFilial` | `GloFilial` | MOD-02 |
| `IdOperacao` | `GloOperacao` | MOD-02 |
| `IdTipoDocumento` | `GloTipoDocumento` | MOD-02 |
| `IdCondicaoPagamento` | `GloCondicaoPagamento` | MOD-02 |
| `IdIndiceEconomico` | índice econômico | MOD-02 |
| `IdUsuario*` (inclusão/alteração/liberação) | `GloUsuario` | MOD-02 |
| `IdPortador` / `IdFormaCobranca` | portador / forma de cobrança | MOD-02 |
| `IdCentroCusto` / `IdClasse` (no rateio) | centro de custo / classe | MOD-02 |
| `IdBanco` | `GloBanco` | MOD-02 |
| `IdOrigem` + `ProcessoOrigem` | documento de venda / compra / contrato / OS / … | MOD-04/06/10/11/… |

---

## 5. Bibliotecas de terceiros (E14)

| Lib legada | Uso | Ação |
| :--- | :--- | :--- |
| `BoletoNet` | remessa/retorno CNAB 240/400, boleto | avaliar `BoletoNetCore` ou `BoletoNet.Core` (net10) — `research.md` |
| `BarcodeLib` | código de barras do boleto | `research.md` |
| OFX (leitura de extrato) | conciliação bancária | biblioteca OFX .NET ou parser próprio — `research.md` |
