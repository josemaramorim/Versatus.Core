# Spec Funcional: Caixa e Banco (FCaixaBanco)

> **Tipo:** Spec Funcional — **template de referência do MOD-05 (Padrão A)**
> **Módulo:** Gestão Financeira (`Versatus.GestaoFinanceira`)
> **Versão:** 1.1 (aprovada e implementada na tarefa `E3-T07`, 2026-09-27)
> **Padrão de Tela:** **Padrão A — CRUD Padrão** (`FCaixaBanco : FBaseCadastro`; Novo/Editar/Excluir + grade)
> **Legado:** `cliente/cliente.aplicativo/aplicativo.gestao.financeira/FCaixaBanco.cs` (3017 l) ·
> objetos `servidor/objeto de negócio/gestao.financeira/CaixaBanco.cs` (869 l) +
> `ContaBancaria.cs` (1704 l) + `CaixaBancoUsuario.cs` (350 l)
> **Contrato backend (implementado no E3-T06):** [`specs/modulos/MOD-05/contracts/caixa-banco.md`](../specs/modulos/MOD-05/contracts/caixa-banco.md)
> **Mapa coluna → propriedade:** [`specs/modulos/MOD-05/analysis/E3-caixa-banco.md §2`](../specs/modulos/MOD-05/analysis/E3-caixa-banco.md)
> **Matriz RTV do épico:** [`specs/modulos/MOD-05/matriz-rtv.md#E3`](../specs/modulos/MOD-05/matriz-rtv.md)

---

## 1. Resumo e Objetivo

Cadastrar **caixas** (dinheiro/tesouraria) e **contas bancárias** de cada filial — base de
movimento financeiro, liquidação, cheque e conciliação. Quando o tipo é **Banco**, o registro
ganha a aba **Conta bancária** (agência, número/dígito, titular, limite, conta vinculada, SPED).
Controla também os **usuários autorizados** por caixa/banco (quando o parâmetro
`VinculaCaixaBancoUsuario` está ligado). Boleto/remessa/retorno **não** entram nesta tela
(épico E14 — CLR-03).

---

## 2. Mapeamento de Entidades e Banco de Dados

- **Tabelas:** `FINCAIXABANCO` (PK `IDFINCAIXABANCO, IDGLOFILIAL`) · `FINCONTABANCARIA` (1:1, mesma
  PK) · `FINCAIXABANCOUSUARIO` (PK `+ IDGLOUSUARIO`).
- **Anuláveis (tipos `?`):** `UltimaDataConferida`, `Saldo`, `ContaContabil`, `IdPlanoContabil`,
  `IdTipoContaCaixa`; na conta: `Titular`, `DigitoConta`, `Limite`, `CreditoPendente`,
  `DebitoPendente`, `ChequePendente`, `IdContaBancariaVinculada`, `IdInstituicaoFinanceira`,
  `CpfCnpj`; auditoria (6 colunas).
- **NOT NULL (→ `required` na UI, Lei 10):** `Descricao`, `TipoConta`; na conta: `IdAgencia`,
  `NumeroConta`, `ContaBancariaTipo`. No legado a obrigatoriedade vem do binder
  (`binderControlContaBancaria.ValidateRequiredControls()` — `FCaixaBanco.cs:2310`), sem
  atributo no objeto.

---

## 3. Requisitos Arquiteturais & Boas Práticas

- Backend **já implementado** (E3-T05/T06): POCO em `Domain/Bancos/`, Fluent API em
  `Infrastructure/Mappings/`, serviços `ICaixaBancoService`/`IContaBancariaService` com
  `Result<T>`, controllers finos, CQRS (leitura `ReadContext`, escrita `Context`), paginação
  materializada (SQL Server 2008).
- Frontend: `src/pages/Financeiro/FCaixaBanco/` (`types.ts`, `schema.ts`,
  `CaixaBancoCadastroConfig.tsx` estendendo `BaseCadastroConfig<T>`, `index.tsx`,
  `schema.test.ts`), no mesmo padrão de `AcessoGlobal/FCondicaoPagamento`.

### 3.1 Enumerações (valores do banco — `GLOTIPOENUMERADO`, via `useEnumOptions(idPai)`)

| Enum | Coluna | `idPai` | Valores |
|---|---|---|---|
| `ContaTipo` | `FINCAIXABANCO.IDTIPOCONTA` | 174 | 175 Caixa · 176 Banco |
| `TipoContaCaixa` | `FINCAIXABANCO.IDTIPOCONTACAIXA` | 1481 | 1482 Normal · 1483 Cofre |
| `TipoContaBancaria` | `FINCONTABANCARIA.IDTIPOCONTABANCARIA` | 1478 | 1479 Conta corrente · 1480 Investimento |

Resolve `DÚVIDA-CB1` da versão 0.1.

---

## 4. Endpoints REST da Web API (implementados no E3-T06)

| Método | Rota | Função |
|---|---|---|
| `GET` | `/api/financeiro/caixa-banco/paginado?texto&ativo&idTipoConta&entraFluxoCaixa&page&limit` | Grade |
| `GET` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Registro + usuários + bloco `contaBancaria` (quando Banco) |
| `POST` | `/api/financeiro/caixa-banco` | Criar (`CriarCaixaBancoDto`, com `usuarios[]` e `contaBancaria`) |
| `PUT` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Atualizar (`AtualizarCaixaBancoDto`) |
| `DELETE` | `/api/financeiro/caixa-banco/{idFilial}/{id}` | Excluir |
| `GET`/`PUT` | `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | Usuários (a tela usa o POST/PUT do caixa, que já grava a lista) |
| `GET`/`PUT` | `/api/financeiro/conta-bancaria/{idFilial}/{id}` | Bloco bancário isolado (não usado pela tela) |
| `GET` | `/api/parametro/{chave}` (MOD-02) | `VINCULACAIXABANCOUSUARIO`, `HABILITAPROCESSOCONTABILIZACAO` |
| `GET` | `/api/TipoEnumerado/{idPai}` (MOD-02) | Opções dos enums (§3.1) |

---

## 5. Interface Gráfica Frontend (React + MUI)

### 5.1 Grade

| Coluna | Campo | Observação |
|---|---|---|
| Código | `idCaixaBanco` | |
| Descrição | `descricao` | |
| Tipo | `idTipoConta` | Chip "Caixa"/"Banco" |
| Tipo conta caixa | `idTipoContaCaixa` | "Normal"/"Cofre" (vazio quando Banco) |
| Entra fluxo de caixa | `entraFluxoCaixa` | ícone sim/não |
| Situação | `ativo` | Chip verde "Ativo" / vermelho "Inativo" |

Filtros: texto (código ou descrição), Tipo, Situação, Entra fluxo de caixa.

### 5.2 Formulário — cabeçalho e aba **Geral** (sempre visível)

| # | Campo (legado) | Propriedade | Componente | `required` | Regra de UI (legado) |
|---|---|---|---|---|---|
| 1 | Código | `idCaixaBanco` | `TextField` readonly | — | gerado pelo servidor |
| 2 | Descrição (`textEditNome`) | `descricao` | `TextField` (máx. 100) | ✅ | |
| 3 | Ativo (`checkEditAtivo`) | `ativo` | `Switch` | — | default `true` |
| 4 | Tipo conta (`comboBoxTipoConta`) | `idTipoConta` | `Select` (enum 174) | ✅ | **UI-01**, **UI-02** |
| 5 | Tipo conta caixa (`comboBoxTipoContaCaixa`) | `idTipoContaCaixa` | `Select` (enum 1481) | — | **UI-03** |
| 6 | Entra na previsão/fluxo (`checkEditEntraPrevisao`) | `entraFluxoCaixa` | `Switch` | — | |
| 7 | Usuários (`gridFormEditorUsuario`) | `usuarios[]` | grade de usuários | — | **UI-04**, VAL-E3-01/11 |

### 5.3 Aba **Conta bancária** (`tabPageContaBancaria`) — só quando Tipo = Banco (**UI-05**)

| # | Campo (legado) | Propriedade | Componente | `required` | Regra de UI (legado) |
|---|---|---|---|---|---|
| 8 | Tipo conta bancária (`comboBoxTipoContaBanco`, no `panelTipoContaBanco` do cabeçalho) | `idTipoContaBancaria` | `Select` (enum 1478) — exibido no cabeçalho, ao lado do Tipo | ✅ | **UI-06**, **UI-07** |
| 9 | Agência (`lookupAgencia`) | `idAgencia` | lookup | ✅ | **UI-08** |
| 10 | Número da conta / Dígito | `numeroConta` (máx. 15) / `digitoConta` (máx. 2) | `TextField` ×2 | ✅ / — | **UI-08** |
| 11 | Limite (`calcEditLimite`) | `limite` | `TextField` numérico | — | **UI-08** |
| 12 | Conta de terceiro (`checkGroupContaTerceiro`) | `contaTerceiro` | `Checkbox` (grupo) | — | **UI-09**, VAL-E3-17 |
| 13 | Titular | `titular` (máx. 50) | `TextField` | condicional | dentro do grupo terceiro — VAL-E3-17 |
| 14 | CPF/CNPJ titular | `cpfCnpj` (máx. 14) | `TextField` c/ máscara | condicional | dentro do grupo terceiro — VAL-E3-17 |
| 15 | Permite emitir cheque (`checkGroupCheque`) | `permiteEmitirCheque` | `Checkbox` | — | **UI-10** |
| 16 | Conta vinculada (`lookupEditContaVinculada`) | `idContaBancariaVinculada` | lookup de contas | condicional | **UI-11**, **UI-12**, VAL-E3-06/07 |
| 17 | Enviar SPED (Bloco 1601) (`checkGrouprEnviarSped`) | `enviarSped` | `Checkbox` (grupo) | — | **UI-13**, VAL-E3-02..05 |
| 18 | Instituição financeira fiscal (`lookupEditInstituicaoFinanceiraSPED`) | `idInstituicaoFinanceira` | lookup (só PJ) | condicional | **UI-14**, VAL-E3-02..05 |
| — | Crédito/Débito/Cheque pendente | `creditoPendente`… | somente exibição | — | mantidos pelos Handlers de movimento |

Fora desta tela (E14): `groupBoxDadosBoleto` (Gera boleto/remessa/retorno), abas
Boleto/Remessa/Retorno, botões "Manutenção nosso número/número remessa", impresso de cheque
(`IdDocumentoImpressao`).

### 5.4 Aba **Contábil** (`tabPageContabil`) — só com `HabilitaProcessoContabilizacao` (**UI-15**)

| # | Campo | Propriedade | Componente | `required` |
|---|---|---|---|---|
| 19 | Plano contábil (`lookupEditPlanoContabil`) | `idPlanoContabil` | lookup | — |

> `ContaContabil` (varchar 20) **não tem controle no formulário legado** (só `IdPlanoContabil`
> está ligado) — não aparece na tela; o valor existente é preservado no PUT.

### 5.5 Padrões obrigatórios (skill `migrate-crud`)

Floating label `variant="outlined"`; `required` em todo campo ✅; banner "Alterações não
salvas" ao cancelar e confirmação ao fechar a aba; Chip verde/vermelho de situação.

---

## 6. Regras de Negócio e Matriz RTV

**Herança:** `FCaixaBanco : FBaseCadastro` — o fluxo Novo/Salvar/Excluir/Desfazer vem da base
(`BaseCadastroConfig<T>` no novo sistema). Nenhuma validação de campo própria em `FBaseCadastro`
além da obrigatoriedade do binder (§2).

### 6.1 Validações de domínio (backend já implementado — `matriz-rtv.md#E3`)

| ID | Regra | Mensagem legada | Backend (E3-T05) | Frontend (Zod + MUI) |
|---|---|---|---|---|
| VAL-E3-01 | Com `VinculaCaixaBancoUsuario`: usuário repetido | "Não é permitido repetir usuário(s). Verifique." | `CaixaBancoService` | `superRefine` unicidade |
| VAL-E3-02 | Enviar SPED → instituição financeira obrigatória | "Para conta bancária, a instituição financeira para SPED, deve ser informado a instituição financeira fiscal." | idem | `superRefine` (required condicional) |
| VAL-E3-03 | Enviar SPED e Conta de terceiro não podem estar ambos marcados | "Para conta bancária, ao MARCAR a opção 'Enviar SPED (Bloco 1601)', deve ser DESMARCADO a opção 'Conta de terceiro'." | idem | `superRefine` |
| VAL-E3-04/05 | Instituição SPED deve ser PJ e ter CNPJ | (mensagens legadas) | idem (consulta MOD-02) | N/A — lookup já filtra PJ (UI-14); resto no backend |
| VAL-E3-06 | Investimento → conta vinculada obrigatória | "Para conta bancária do tipo 'Investimento', deve ser informada a conta vinculada." | idem | `superRefine` |
| VAL-E3-07 | Conta vinculada ≠ a própria conta | "…a conta vinculada deve ser DIFERENTE do ID da conta bancária." | idem | `superRefine` (em edição) |
| VAL-E3-08 | Inativar caixa com domínio-período aberto | "Para inativar este caixa deve ser fechado seu domínio período…" | idem (porta E2) | N/A (backend) |
| VAL-E3-09/10 | Parâmetros exclusivos / caixa movimentável | — | idem | N/A (usadas por outras telas) |
| VAL-E3-11 | Usuário já informado na grade | "Este usuário já foi informado." | idem | `superRefine` unicidade |
| VAL-E3-12 | Não alterar usuário de linha salva | "NÃO é permitido alterar usuário após ter sido salvo." | idem | coluna usuário readonly em linha já salva |
| VAL-E3-17 | Conta de terceiro → titular **e** CPF/CNPJ | "Para cadastrar uma conta de terceiro tem que informar o nome do titular e o CPF/CNPJ do mesmo." | `ContaBancariaService` | `superRefine` |

### 6.2 Regras de tela (achadas no `FCaixaBanco.cs` e nos setters — novas nesta auditoria)

| ID | Origem legada | Regra | Destino |
|---|---|---|---|
| UI-01 | `FCaixaBanco.cs:2637-2658` `LigarTabGeral` | Abas desabilitadas até escolher o Tipo | `index.tsx` |
| UI-02 | `FCaixaBanco.cs:2269-2283` `BinderEnableChanging` + `2657` | Tipo **bloqueado** depois de salvo (só editável na inclusão) | `index.tsx` (readonly em edição) |
| UI-03 | `CaixaBanco.cs:655` (setter `TipoConta`) + `FCaixaBanco.cs:2848-2850` | Tipo = Caixa → `TipoContaCaixa = Normal (1482)` e campo visível; Banco → vazio e oculto. Campo readonly depois de salvo | `types`/`index.tsx` |
| UI-04 | `FCaixaBanco.cs:2626-2627` | Grade de usuários só visível com `VinculaCaixaBancoUsuario` | `index.tsx` |
| UI-04a | `FCaixaBanco.cs:2412-2421` `ValidarCaixaBancoUsuario` | Com o parâmetro ligado e **sem** usuários: pedir confirmação — "Está configurado para controlar caixa/banco por usuário, sendo necessário informar qual(is) usuário(s) terá(ão) acesso a esta conta. Deseja salvar este caixa/banco sem informar usuário(s)?" | `index.tsx` (diálogo antes do salvar) |
| UI-05 | `FCaixaBanco.cs:2823-2839` `AtivarContaBancaria` | Aba Conta bancária só com Tipo = Banco | `index.tsx` |
| UI-06 | `FCaixaBanco.cs:2851` | Tipo conta bancária readonly depois de salvo | `index.tsx` |
| UI-07 | `FCaixaBanco.cs:2439-2462` `ValidarTipoContaBancaria` | Ao abrir a aba sem tipo: "Deve ser informado o tipo da conta bancária." e volta para Geral | `schema` (`required`) + `index.tsx` |
| UI-08 | `FCaixaBanco.cs:2685,2694` | Dados da conta (agência, número, dígito, limite) **só editáveis em Conta corrente**; em Investimento ficam desabilitados | `index.tsx` |
| UI-09 | `ContaBancaria.cs:1112-1123` + `432-439` `LimpaDadosTerceiro` | Desmarcar "Conta de terceiro" **limpa** titular e CPF/CNPJ | `index.tsx` (on change) |
| UI-10 | `FCaixaBanco.cs:2683,2692` | "Permite emitir cheque" só visível em Conta corrente | `index.tsx` |
| UI-11 | `FCaixaBanco.cs:2681,2693` | Grupo "Conta vinculada" só visível em Investimento | `index.tsx` |
| UI-12 | `FCaixaBanco.cs:2372-2385,2426-2434` | Lookup de conta vinculada só lista **Conta corrente**; se não for: "Para conta vinculada, deve ser informada uma conta bancária do tipo 'Conta corrente'." | lookup + `superRefine` |
| UI-13 | `ContaBancaria.cs:1183-1196` (setter `EnviarSped`) | Desmarcar "Enviar SPED" **limpa** a instituição financeira | `index.tsx` (on change) |
| UI-14 | `FCaixaBanco.cs:2387-2391` | Lookup de instituição financeira só lista Pessoa **Jurídica** | lookup |
| UI-15 | `FCaixaBanco.cs:2630-2631` | Aba Contábil só com `HabilitaProcessoContabilizacao` | `index.tsx` |
| UI-16 | `ContaBancaria.cs:1347-1375` (setter `ContaBancariaTipo`) | Mudar o tipo da conta bancária limpa a conta vinculada; para **Investimento** zera agência, número, dígito, limite, conta de terceiro, permite cheque (e boleto/remessa/retorno) | `index.tsx` (on change) — ver `DÚVIDA-CB6` |
| UI-17 | `ContaBancaria.cs:444-470` `SetDadosBancoContaVinculada` (setter `IdContaBancariaVinculada`) | Escolher a conta vinculada **copia** dela agência, número, dígito, limite, conta de terceiro, titular e CPF/CNPJ | ver `DÚVIDA-CB6` |

---

## 7. Critérios de Aceite

- **C1 (Listagem):** grade carrega `GET /paginado` com filtros; paginação no servidor (em memória).
- **C2 (Detalhe):** editar carrega o registro e, quando Banco, a aba Conta bancária preenchida.
- **C3 (Criação/Edição):** salvar envia o DTO completo (caixa + usuários + conta); `201`/`200`;
  erros do backend (`400`) exibidos com a mensagem legada.
- **C4 (Abas condicionais):** UI-01, UI-05, UI-10, UI-11, UI-15 respeitadas.
- **C5 (Obrigatoriedade visual):** todo campo ✅ tem `required` (asterisco); campos condicionais
  validam só quando visíveis.
- **C6 (Efeitos de campo):** UI-03, UI-09, UI-13, UI-16, UI-17 reproduzidos.
- **C7 (Testes):** `schema.test.ts` com 1 teste por regra Zod (VAL-E3-01, 02, 03, 06, 07, 11, 17,
  UI-07, UI-12) + campos obrigatórios; `npm run build` e `npm test` verdes.
- **C8 (Escopo):** nenhum campo/aba de boleto/remessa/retorno (E14).

---

## 8. Pendências e Dúvidas

- ~~`DÚVIDA-CB1`~~ resolvida (§3.1). ~~`DÚVIDA-CB2`~~ resolvida (mensagens em §6). ~~`DÚVIDA-CB3`~~
  resolvida: `VINCULACAIXABANCOUSUARIO` e `HABILITAPROCESSOCONTABILIZACAO` (UI-04, UI-15).
- `DÚVIDA-CB4` (exclusão × inativação): o legado permite excluir (`CaixaBanco.ExecutarExcluir`);
  bloqueio por uso vem das FKs do banco (hoje o DELETE bloqueado volta `500` — ver PR #27).
- ~~`DÚVIDA-CB5`~~ **decidida (2026-09-27): criar endpoints de consulta** — `GET /api/financeiro/lookups/*` (ver `contracts/caixa-banco.md`). Texto original: não existem na API endpoints de consulta para
  **Agência** (`GLOAGENCIA` nem mapeada no MOD-02), **Usuário**, **Instituição financeira** e
  **Plano contábil**. Conta vinculada pode usar o `/paginado` do próprio caixa-banco, mas a
  lista não traz o tipo da conta bancária (UI-12).
- ~~`DÚVIDA-CB6`~~ **decidida (2026-09-27): tela + backend** — viraram `VAL-E3-22..26` na Matriz RTV. Texto original: estão em setters do
  objeto de negócio legado e **não entraram na Matriz RTV/ROT** do E3 — o backend do E3-T05
  **não** os aplica. Implementar só na tela, ou também no backend?
- ~~`DÚVIDA-CB7`~~ **decidida (2026-09-27): rota `/financeiro/caixabanco`**, sem alterar o banco. Texto original: a rotina 29 "Caixa/Conta" (`FCaixaBanco`) já está em `GLOROTINA`
  com `RotaWeb = NULL` → o menu gera `/financeiro/caixabanco`. Registrar a tela nessa rota, ou
  gravar `RotaWeb = 'caixa-banco'` no banco (como foi feito para `condicao-pagamento`)?
