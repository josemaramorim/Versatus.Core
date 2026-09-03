---
name: legacy-operation-audit
description: Audita e extrai 100% das operações e transações de negócio legadas de telas que NÃO são CRUD (Liquidar, Estornar, Reverter, Acertar, Fechar caixa, Conciliar) — máquina de estados, pré-condições, efeitos colaterais, ordem de persistência e rollback — gerando a Matriz ROT (Rastreabilidade de Operações e Transações). Complementa legacy-validation-audit.
---

# Skill: legacy-operation-audit (Auditoria de Operações e Transações Legadas)

## 📌 Objetivo

A `legacy-validation-audit` cobre **validações** (o que impede salvar). Esta skill cobre o
que a maioria das telas financeiras/fiscais realmente faz: **operações transacionais** —
mudam estado, geram lançamentos, movimentam saldo, revertem. Garante que **100% dessas
operações** sejam migradas fielmente para Handlers .NET 10, com testes de integração e
paridade.

Produz a **Matriz ROT (Rastreabilidade de Operações e Transações)**, incorporada à
`spec.md` do módulo e a `specs/modulos/MOD-XX/matriz-rot.md`.

---

## 🔍 Quando usar

- Telas sem botão Novo/Excluir cujo botão principal é "Executar", "Confirmar", "Liquidar",
  "Estornar", "Reverter", "Fechar", "Processar", "Gerar", "Conciliar".
- Classes de `Operação` no inventário (`OperacaoDocumentoBase`, `Liquidacao`,
  `LiquidacaoEstorno`, `Reversao`, `AdtoAcerto`, `FechamentoCaixaBase`, `TransacaoFilial`…).
- Sempre em paralelo com `legacy-validation-audit` para o mesmo formulário/entidade.

---

## 🌲 1. Resolução recursiva de herança

Igual à `legacy-validation-audit`: subir toda a árvore
(`Operacao → OperacaoDocumentoBase → ObjectGenerator …`), lendo cada classe-pai por
inteiro e varrendo `override` / `base.<Metodo>()`.

---

## 🔎 2. Varredura por palavras-chave (Grep Audit)

Nos arquivos da operação + classes-pai + utilitários (`View/*Util.cs`, `Objeto negocio/*`):

```
Executar    Processar    Confirmar     Efetivar     Aplicar
Persistir   Salvar       Gravar        Excluir
Estornar    Reverter     Cancelar      Desfazer
Fechar      Abrir        Reabrir       Bloquear     Liberar
Calcular    Ratear       Distribuir    Atualizar<Saldo|Valor|Situacao>
BeginTransaction   Commit   Rollback   Transacao
OnBeforeExecutar*  OnAfterExecutar*  OnBefore*Persist*  OnAfter*Persist*
```

---

## 📋 3. A Matriz ROT

Para **cada operação identificada**, uma linha:

| ID | Origem Legada (Arquivo:Método:Linha) | Operação | Pré-condições (estado + validações que a habilitam) | Passos na ordem exata (o que persiste, em que sequência) | Entidades/Tabelas afetadas | Efeitos colaterais (saldo, situação, período, sequencial, lançamento espelho) | Condição de Rollback | Destino .NET (Handler/UseCase) | Teste de paridade (golden) |
|----|--------------------------------------|----------|------------------|--------------|--------------|--------------------|--------------------|--------------------|--------------------|
| **OP-01** | `Liquidacao.cs:Executar():430` | Liquidar parcela | Parcela `Aberta` ou `Parcial`; período do domínio aberto; formas somam o valor | 1) valida formas 2) cria `MovimentoFinanceiro` 3) baixa `DocumentoParcela.ValorLiquidado` 4) atualiza `SituacaoDocumento` 5) grava `Liquidacao` + `LiquidacaoFormaMovimento` 6) atualiza `SaldoCaixaBanco` | `FinLiquidacao`, `FinMovimentoFinanceiro`, `FinDocumentoParcela`, `FinDocumento`, saldo | Saldo do caixa/banco += valor; parcela → `Liquidada` se total; sequencial de movimento consumido | Qualquer passo falha → nada persiste | `LiquidarParcelaHandler` | `LiquidarParcela_ValoresBatemComLegado` |

### Campos que NÃO podem faltar
- **Ordem de persistência** literal (o legado depende dela).
- **Toda** tabela tocada, mesmo indiretamente (saldo, sequencial, log de período).
- **Máquina de estados**: estado antes → estado depois, por entidade.
- **Operação inversa** correspondente (Liquidação ↔ Estorno; Movimento ↔ Reversão) —
  linkar os IDs (`OP-01` inverso de `OP-07`).

---

## 🔁 4. Máquina de Estados (por entidade)

Para cada entidade de estado (`SituacaoDocumento`, situação de parcela, situação de cheque,
status de período), montar a tabela de transições que as operações disparam:

| Entidade | Estado origem | Evento/Operação | Estado destino | Guardas |
|----------|---------------|-----------------|----------------|---------|
| Documento | Aberto | Liberar (OP-03) | Liberado | usuário com permissão X |
| Documento | Liberado | Liquidar total (OP-01) | Liquidado | — |
| Documento | Liquidado | Estornar (OP-07) | Liberado | período aberto |

---

## 🛡️ 5. Dupla trava por testes

1. **Teste de integração** por linha `OP-xx`: executa o Handler contra banco (InMemory ou
   SQL de teste), afirma estado final de **todas** as entidades/tabelas afetadas e o
   rollback quando um passo falha.
2. **Golden test de paridade** por linha `OP-xx`: mesma entrada rodada no legado (ou
   valores capturados dele) → compara saldo/valor/situação resultante número a número.
   Para cálculos, delega à skill `legacy-calc-parity`.
3. `sdd-analyze` reprova o módulo se existir método `Executar*`/`Estornar*`/`Reverter*`/
   `Fechar*` no legado sem linha `OP-xx`.

---

## 📎 Saída

- Seção "Matriz ROT" na `spec.md` + arquivo `specs/modulos/MOD-XX/matriz-rot.md` +
  "Máquina de Estados".
- Commit na branch `docs/` do módulo: `docs(mod-XX): Matriz ROT e máquina de estados`.
