# Spec Funcional: Caixa e Banco (FCaixaBanco)

> **Tipo:** Spec Funcional — **template de referência do MOD-05 (Padrão A)**
> **Módulo:** Gestão Financeira (`Versatus.GestaoFinanceira`)
> **Versão:** 0.1 (rascunho — a completar na tarefa `E3-T07` com `legacy-validation-audit`)
> **Padrão de Tela:** **Padrão A — CRUD Padrão** (herda `FBaseCadastro`)
> **Legado:** `cliente/cliente.aplicativo/aplicativo.gestao.financeira/FCaixaBanco.cs` (3017 l) ·
> objeto `servidor/objeto de negócio/gestao.financeira/CaixaBanco.cs` (869 l) +
> `ContaBancaria.cs` (1704 l)
> **Contrato backend:** [`specs/modulos/MOD-05/contracts/caixa-banco.md`](../specs/modulos/MOD-05/contracts/caixa-banco.md)
> **Modelo de dados:** [`specs/modulos/MOD-05/data-model.md §3.1–3.4`](../specs/modulos/MOD-05/data-model.md)
>
> ⚠️ Esta spec estabelece **estrutura e decisões de mapeamento**. A extração 100% da
> Matriz RTV (Seção 6) é feita na tarefa `E3-T07` rodando `legacy-validation-audit` +
> `legacy-operation-audit` em `FCaixaBanco.cs` e classes-pai.

---

## 1. Objetivo

Cadastrar **caixas** (dinheiro/tesouraria) e **contas bancárias** de cada filial, que são
a base de todo movimento financeiro, liquidação, cheque e conciliação. Quando o tipo é
banco, o registro ganha o bloco `ContaBancaria` (agência, conta, dígito, titular, limite)
e — em telas de E14 — os parâmetros de boleto/remessa/retorno. Controla também os
**usuários autorizados** por caixa/banco.

---

## 2. Endpoints da API

Conforme `contracts/caixa-banco.md`:

| Método | Rota | Função |
|---|---|---|
| `GET` | `/api/financeiro/caixa-banco/paginado?page&limit&search&ativo&idTipoConta&entraFluxoCaixa` | Listagem paginada (em memória — SQL Server 2008) |
| `GET` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Registro por chave composta (inclui `ContaBancaria` quando banco) |
| `POST` | `/api/financeiro/caixa-banco` | Criar (gera sequencial por filial) |
| `PUT` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Atualizar |
| `DELETE` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Excluir (bloqueado se em uso) |
| `GET` | `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | Usuários autorizados |
| `PUT` | `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | Salvar usuários (lista) |
| `GET` | `/api/financeiro/conta-bancaria/{idFilial}/{id}` / `PUT` | Bloco bancário (núcleo) |

> **Chave composta:** todas as rotas de detalhe usam `{idFilial}/{id}` (`FINCAIXABANCO` PK
> = `IDFINCAIXABANCO, IDGLOFILIAL`). `id` = `IdCaixaBanco`.

---

## 3. Conversão de Dados e Enums (Legado vs. Frontend)

> Valores inteiros preservados de `Projeto.Geral.Enumerado`. Inventário fino na tarefa
> `E0-T01` (`specs/modulos/MOD-05/enums.md`). Frontend carrega via `useEnumOptions`.

| Enumeração | Coluna legada | Uso |
|---|---|---|
| **TipoConta** | `FINCAIXABANCO.IDTIPOCONTA` | Caixa / Banco (define abas visíveis) — `DÚVIDA: confirmar valores em E0-T01` |
| **TipoContaCaixa** | `IDTIPOCONTACAIXA` (nullable) | subtipo quando Caixa |
| **TipoContaBancaria** | `FINCONTABANCARIA.IDTIPOCONTABANCARIA` | subtipo quando Banco |
| **TipoArquivoRemessaRetorno** | `FINCONTABANCARIA.IDTIPOARQUIVOREMESSARETORNO` (nullable) | layout CNAB (só telas E14) |

Booleanos (`smallint` no banco → `bool`): `ATIVO`, `ENTRAFLUXOCAIXA`,
`PERMITEEMITIRCHEQUE`, `CONTATERCEIRO`; (E14) `GERABOLETO`, `GERAREMESSA`,
`PROCESSARETORNO`, `ENVIARSPED`, `BOLETOBENEFICIARIODIFERENTE`, `BOLETOSACADOAVALISTA`.

---

## 4. Colunas da Listagem (Grid)

| Coluna | Campo interno | Observação |
|---|---|---|
| Código | `idCaixaBanco` | por filial |
| Descrição | `descricao` | |
| Tipo | `idTipoConta` | "Caixa" / "Banco" (Chip) |
| Entra Fluxo de Caixa | `entraFluxoCaixa` | ícone sim/não |
| Saldo | `saldo` | `decimal?`, formatação monetária (read-only) |
| Situação | `ativo` | Chip verde "Ativo" / vermelho "Inativo" |

---

## 5. Campos do Formulário e Regras de Interface

Cabeçalho + abas condicionais pelo campo **Tipo** (`idTipoConta`). No legado, o layout
usa `panelGeral`, `panelTipoContaCaixa`, `panelTipoContaBanco`, `groupBoxDadosConta`,
`tabPageContaBancaria`, `tabPageContabil` — e `tabPageBoleto`/`tabPageRemessa`/`tabPageRetorno`
que **pertencem ao épico E14** (não entram nesta tela nesta rodada — ver `spec.md §1.5`).

### 5.1 Dados Gerais (cabeçalho — sempre visível)

| Campo | Componente | Obrigatório | Regra |
|---|---|---|---|
| Código | `TextField` readonly | — | `idCaixaBanco` (gerado) |
| Descrição | `TextField` | ✅ `required` | `DESCRICAO varchar(100)` — não vazia |
| Tipo | `Select` | ✅ `required` | `TipoConta`; muda abas/painéis visíveis; readonly após ter movimento |
| Entra no Fluxo de Caixa | `Switch` | — | `ENTRAFLUXOCAIXA` |
| Ativo | `Switch` | — | `ATIVO`; default `true` |
| Conta Contábil | `TextField` / lookup | — | `CONTACONTABIL varchar(20)` + `IDCONPLANOCONTABIL` (aba Contábil; só se `HabilitaProcessoContabilizacao`) |

### 5.2 Aba "Dados da Conta" — visível quando Tipo = **Banco**

| Campo | Componente | Obrigatório | Regra |
|---|---|---|---|
| Agência | `LookupEdit` (banco/agência) | ✅ | `FINCONTABANCARIA.IDGLOAGENCIA` |
| Número da Conta | `TextField` | ✅ | `NUMEROCONTA varchar(15)` |
| Dígito | `TextField` | — | `DIGITOCONTA varchar(2)` |
| Titular | `TextField` | — | `TITULAR varchar(50)` |
| CPF/CNPJ do Titular | `TextField` c/ máscara | — | `CPFCNPJ varchar(14)` |
| Tipo de Conta Bancária | `Select` | ✅ | `IDTIPOCONTABANCARIA` |
| Limite | `TextField` numérico | — | `LIMITE numeric(23,8)` → `decimal?` |
| Permite Emitir Cheque | `Switch` | — | `PERMITEEMITIRCHEQUE` |
| Conta de Terceiro | `Switch` | — | `CONTATERCEIRO` |
| Conta Vinculada | `Select` (outra conta) | — | `IDFINCONTABANCARIAVINCULADA` |
| Instituição Financeira | `Select` | — | `IDGLOINSTITUICAOFINANCEIRA` |

> Campos **read-only nesta tela** (só exibidos): `CREDITOPENDENTE`, `DEBITOPENDENTE`,
> `CHEQUEPENDENTE` — mantidos pelos Handlers de movimento.
> Campos de **boleto/remessa/retorno** (`TIPOCARTEIRA`, `ULTIMONOSSONUMERO`, `GERABOLETO`,
> `DIRETORIO*`, `IDFINITEMFINANCEIRO*`, …) **não aparecem aqui** — épico E14.

### 5.3 Aba "Usuários" — sempre visível

Grid editável (`gridFormEditorUsuario` no legado): adiciona/remove `GloUsuario` autorizado
no caixa/banco. Salvamento junto com o registro (endpoint `.../usuarios` PUT em lote).

### 5.4 Comportamento condicional

- Trocar **Tipo** de Banco→Caixa com dados bancários preenchidos: confirmar descarte.
- Tipo readonly quando o caixa/banco já tem `SaldoCaixaBanco`, `Dominio` ou `Movimento`.
- Botões "Manutenção Nosso Número" / "Manutenção Número Remessa" (legado): **removidos
  desta tela** → ficam nas telas de E14.

---

## 6. Regras de Negócio e Matriz RTV

> **A completar em `E3-T07`** com `legacy-validation-audit` em `FCaixaBanco.cs` +
> `FBaseCadastro` + `CaixaBanco.cs`/`ContaBancaria.cs`. Esqueleto:

| ID | Origem Legada | Camada | Regra / Condição | Mensagem Legada | Destino Backend (`Result<T>`) | Destino Frontend (Zod + MUI) |
|---|---|---|---|---|---|---|
| VAL-01 | `CaixaBanco.cs:Validar` (a confirmar) | Domínio | `Descricao` obrigatória | *"Informe a descrição."* (confirmar) | `if (string.IsNullOrWhiteSpace(dto.Descricao))` | `z.string().min(1)` + `required` |
| VAL-02 | idem | Domínio | `IdTipoConta` obrigatório | *"Informe o tipo da conta."* | `if (dto.IdTipoConta is null or 0)` | `z.number()` + `required` no Select |
| VAL-03 | `ContaBancaria.cs` | Domínio | Quando Banco, `NumeroConta` obrigatória | *"Informe o número da conta."* | condicional por tipo | `superRefine` |
| VAL-04 | `CaixaBanco.cs:ExecutarExcluir` | Estado | Bloquear exclusão se há `Dominio`/`SaldoCaixaBanco`/`DocumentoParcela`/`Movimento` | *"Caixa/Banco possui movimentação e não pode ser excluído."* (confirmar) | `if (await _repo.EmUsoAsync(...))` | *N/A (backend)* |
| VAL-05 | `CaixaBanco.cs:774 ValidarControleCaixaBanco` | Domínio/Parâmetro | Regra condicionada a parâmetro de controle de caixa/banco | *(confirmar)* | replicar comportamento condicional (Regra 16) | *N/A* |
| … | | | | | | |

**Herança:** `FCaixaBanco : FBaseCadastro` — incluir as validações de `FBaseCadastro`
(`ValidarCamposBase`, evento de salvar) no escopo da auditoria.

---

## 7. Critérios de Aceite

- **C1 (Listagem):** `GET /paginado` retorna `200` com itens + total; filtros `ativo`,
  `idTipoConta` funcionam; paginação em memória.
- **C2 (Detalhe por chave composta):** `GET /{idFilial}/{id}` retorna o registro e, quando
  banco, o bloco `ContaBancaria` **com as 55 propriedades** (Regra 4).
- **C3 (Criação):** `POST` gera `IdCaixaBanco` via `GeradorSequencialService` (nunca
  IDENTITY); `201`.
- **C4 (Validação):** dados inválidos → `400` com `ValidationError[]` e a mensagem legada.
- **C5 (Abas condicionais):** trocar Tipo mostra/esconde a aba "Dados da Conta"; campos
  obrigatórios da aba só validam quando visível.
- **C6 (Obrigatoriedade visual):** todo campo `✅` tem `required` no MUI (asterisco).
- **C7 (Usuários em lote):** `PUT .../usuarios` substitui a lista; `200`.
- **C8 (Testes):** `CaixaBancoServiceTests` cobre 100% da Matriz RTV (Seção 6);
  `schema.test.ts` cobre cada regra Zod. `dotnet test` + `npm test` verdes.
- **C9 (Escopo):** nenhuma aba/campo de boleto/remessa/retorno nesta tela (E14).

---

## 8. Pendências e Dúvidas

- `DÚVIDA-CB1`: valores inteiros de `TipoConta` / `TipoContaBancaria` / `TipoContaCaixa`
  (resolver em `E0-T01`).
- `DÚVIDA-CB2`: mensagens legadas exatas de `VAL-01..05` (resolver em `E3-T07` com o
  legado real).
- `DÚVIDA-CB3`: quais parâmetros de `GloParametro` condicionam `ValidarControleCaixaBanco`
  e `HabilitaProcessoContabilizacao` (Regra 16).
- `DÚVIDA-CB4`: o legado permite excluir caixa/banco em algum cenário, ou só inativar?
