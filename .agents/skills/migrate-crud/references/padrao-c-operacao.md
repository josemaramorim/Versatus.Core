# Padrão C — Operação / Assistente (telas transacionais que não são CRUD)

> Referência das skills `migrate-crud` e `spec-generator`. Complementa o **Padrão A (CRUD)**
> e o **Padrão B (Configuração em Lote)**. Introduzido em 2026-09-03 para o MOD-05
> (Gestão Financeira), onde a maioria das telas herda `FBaseProcesso` /
> `FDocumentoSelecaoBase` e executa uma **operação de negócio transacional**, não um
> cadastro.

---

## 1. Como identificar (classificação da Seção 0)

Classifique como **Padrão C** quando **qualquer** destes for verdadeiro:

| Sinal no legado | |
| :--- | :--- |
| Herda `FBaseProcesso`, `FDocumentoSelecaoBase`, `FDocumentoOperacaoBase`, `FSuprimentoBase`, `FEstornoDocumento` (ou similar de "processo") | ✔ |
| O botão principal é **"Confirmar" / "Executar" / "Liquidar" / "Estornar" / "Reverter" / "Processar" / "Gerar" / "Fechar" / "Acertar"** — e **não** há "Novo" nem "Excluir" no sentido de cadastro | ✔ |
| A tela **seleciona** registros existentes (grid de parcelas, títulos, cheques, movimentos) e **compõe** uma ação sobre eles (formas de pagamento, rateio, distribuição) | ✔ |
| Exibe **totais de conferência** ("Total a liquidar", "Total pendente", "Diferença") que precisam fechar antes de confirmar | ✔ |
| O submit chama **um Handler transacional** (1 transação, N tabelas afetadas, rollback) — cf. `plan.md §5` do módulo e a **Matriz ROT** | ✔ |
| Há passos/abas em sequência (selecionar → informar → confirmar) | ✔ (assistente) |

> Se a tela tem grid paginado com Novo/Editar/Excluir → **Padrão A**.
> Se edita valores de itens já existentes em lote via Accordion/TreeList → **Padrão B**.
> Se executa uma operação sobre seleção → **Padrão C**.

---

## 2. Endpoints (backend) — não é CRUD

O `contracts/<area>.md` do módulo define, tipicamente:

| Fase | Rota | Verbo | CQRS | Papel |
| :--- | :--- | :--- | :--- | :--- |
| **Seleção** | `/api/<modulo>/<operacao>/<itens-elegiveis>` | GET | Read | lista paginada dos registros que a operação pode processar (com filtros) |
| **Simulação** (quando há cálculo) | `/api/<modulo>/<operacao>/simular` | POST | Read (não persiste) | recebe a seleção + parâmetros e devolve os valores calculados (juros, multa, desconto, totais, troco) para a tela conferir |
| **Execução** | `/api/<modulo>/<operacao>` | POST | Write (Handler, 1 transação) | executa; devolve o resultado (ids gerados, novos saldos, situação) |
| **Desfazer** (quando existe) | `/api/<modulo>/<operacao>/{id}/estornar` \| `/reverter` | POST | Write (Handler) | operação inversa |

**Proibido** endpoints `PUT/{id}` / `DELETE/{id}` de cadastro nessa área.
Falha de regra → `Result<T>` → `400` com `ValidationError[]` (Artigo VI).

---

## 3. Frontend — estrutura da tela de operação

Pasta: `src/pages/<Modulo>/F<Nome>/` com:

```
types.ts        — I<Nome>Selecao, I<Nome>Item, I<Nome>Forma, I<Nome>Command, defaultValues
schema.ts       — Zod: valida a COMPOSIÇÃO da operação (não um cadastro)
<Nome>OperacaoConfig.tsx  — estende BaseOperacaoConfig<TCmd, TResult> (ver §4)
index.tsx       — a View: seleção + composição + resumo + Confirmar
schema.test.ts  — Vitest, 1 teste por regra da Matriz RTV
```

### Layout de `index.tsx` (padrão visual)

1. **Cabeçalho / filtros de seleção** — data da operação, domínio/período, entidade,
   portador, faixa de vencimento. Campos MUI `variant="outlined"` floating label,
   `required` nos obrigatórios (Lei 10).
2. **Grid de elegíveis** (esquerda/topo) — checkbox de seleção; colunas read-only
   (título, parcela, vencimento, valor, saldo, acréscimo/desconto calculado). Alimentado
   pelo endpoint de seleção (paginação materializada).
3. **Painel de composição** (direita/baixo) — o que a operação exige:
   - **Formas de pagamento/movimento** (grid editável): tipo, valor, caixa/banco,
     e um sub-form condicional por tipo (Cartão / PIX / Cheque cliente / Cheque empresa /
     Depósito / Crédito / Abatimento) — abre em modal (`FDetalhe*` do legado → dialog).
   - **Rateio** (quando aplicável): centro de custo × classe × valor/percentual, com
     validação "soma do rateio = valor rateável".
   - **Troco** (quando aplicável).
4. **Barra de resumo** (fixa no rodapé): `Total selecionado`, `Total informado`,
   `Diferença` — a **Diferença deve ser 0** (ou dentro da tolerância do legado) para
   habilitar o **Confirmar**.
5. **Botão `Confirmar`** — dispara `simular` (opcional, para revisão) e depois `executar`.
   Enquanto pendente: desabilitado. Sucesso: toast + fecha/limpa. Erro `400`: exibe os
   `ValidationError` inline.
6. **Confirmação de saída com alterações não salvas** — mesmo banner/textos do Padrão A
   (`"Alterações não salvas"` / `"Deseja realmente cancelar e descartar as alterações?"`).

### Regras específicas do Padrão C

- **Sem** botões "Novo" / "Excluir" de cadastro em lugar nenhum do JSX.
- O `schema.ts` valida a **operação inteira** (`superRefine`): seleção não vazia, soma das
  formas = líquido, período aberto (quando o backend expõe), rateio fechado.
- Toda mensagem de erro reproduz **a mensagem legada exata** (Matriz RTV).
- Cálculo **nunca** é refeito no frontend como fonte de verdade — vem de `simular`
  (paridade garantida pelos golden tests do backend). O frontend só exibe.
- Reutiliza `useEnumOptions` para tipos de forma, situação, etc.

---

## 4. Base de frontend a criar (uma vez por projeto)

`src/components/operacao/BaseOperacaoConfig.tsx` + `src/hooks/useOperacaoState.ts`:

- `useOperacaoState<TCmd, TResult>()` — estado de: filtros de seleção, itens elegíveis
  (query paginada), itens selecionados, composição (formas/rateio/troco), resultado de
  `simular`, submit de `executar`, flags `isDirty` / `isSubmitting`.
- `BaseOperacaoConfig<TCmd, TResult>` (classe OOP, espelha `BaseCadastroConfig<T>`):
  - `getFiltrosSelecao()` — campos do cabeçalho.
  - `getColunasElegiveis()` — colunas do grid de seleção.
  - `getPaineisComposicao()` — quais painéis (formas, rateio, troco) e em que ordem.
  - `endpointSelecao` / `endpointSimular` / `endpointExecutar` / `endpointDesfazer`.
  - `mapSelecaoToCommand(...)` / `mapResultToView(...)` — sobrescrever quando o shape diverge.
  - `validarComposicao(cmd): ValidationError[]` — espelho leve da Matriz RTV (a fonte da
    verdade é o backend).
- **Proibido** hardcode de endpoint/módulo dentro de `useOperacaoState` (Lei 4) — tudo
  vem do `Config`.

> Enquanto essa base não existir, a 1ª tela Padrão C do projeto a cria (é uma tarefa
> `frontend` do épico, não um pré-requisito de todo o módulo).

---

## 5. Matriz RTV no Padrão C

A coluna **Destino Frontend** aponta para `zod.superRefine` / prop de componente, como no
Padrão A. A diferença é que muitas linhas `VAL-xx` são **regras de composição** (soma,
período, rateio) e não de campo único — vão para o `superRefine` do schema da operação.
As regras de **efeito** (o que a operação faz) ficam na **Matriz ROT**, não na RTV, e são
cobertas por testes de backend (integração + golden), não por `schema.test.ts`.

---

## 6. Checklist de aceite (Seção 7 da spec)

- [ ] Tela sem "Novo"/"Excluir"; botão principal = "Confirmar/Executar".
- [ ] Grid de elegíveis carrega do endpoint de seleção (paginação materializada).
- [ ] Barra de resumo com `Diferença` e Confirmar bloqueado enquanto ≠ 0.
- [ ] `simular` chamado antes de `executar` quando há cálculo; valores só exibidos.
- [ ] `executar` → `Result<T>`; `400` mostra `ValidationError` inline com mensagem legada.
- [ ] `schema.test.ts` cobre cada `VAL-xx` de composição.
- [ ] Efeitos (ids gerados, saldos, situação) validados por teste de **backend** (ROT), não no front.
- [ ] Operação inversa (estorno/reversão) acessível quando existe no legado.
