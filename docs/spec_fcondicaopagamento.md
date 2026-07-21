# Spec Funcional: Condição de Pagamento (FCondicaoPagamento)

> **Tipo:** Spec Funcional  
> **Módulo:** Acesso Global  
> **Versão:** 1.0  
> **Padrão de Tela:** Padrão A - CRUD Padrão  
> **Baseado em:** Golden Pattern de CRUD Padrão do Versatus (.NET 10 + React OOP)

---

## 1. Objetivo

Gerenciar o cadastro de Condições de Pagamento, as quais definem como os vencimentos, parcelas, faixas e dias da semana são calculados e gerados no faturamento (vendas, PDV, OS, etc.), além de definir regras de acréscimo/desconto padrão e disponibilidade por tipo (Pagar, Receber ou Ambas).

---

## 2. Endpoints da API

| Método | Rota | Função |
|---|---|---|
| `GET` | `/api/condicaopagamento/paginado?page&limit&sortBy&sortOrder&search` | Listagem paginada com paginação em memória (Compatibilidade SQL Server 2008) |
| `GET` | `/api/condicaopagamento/{id}` | Obter registro por ID (com a lista de regras/parcelas/faixas) |
| `POST` | `/api/condicaopagamento` | Criar nova condição de pagamento |
| `PUT` | `/api/condicaopagamento/{id}` | Atualizar condição de pagamento existente |
| `DELETE` | `/api/condicaopagamento/{id}` | Remover condição de pagamento |

---

## 3. Conversão de Dados e Enums (Legado vs. Frontend)

> [!IMPORTANT]
> O banco de dados legado usa os seguintes enums (tabela `GloTipoEnumerado` / parent ID correspondente). O frontend deve carregar dinamicamente ou mapear essas opções usando `useEnumOptions` do hook de enums correspondente:

### Enums do Sistema

| Enumeração | Parent ID | Valores Legados / Banco | Significado |
|---|---|---|---|
| **CondicaoPagtoTipo** | `35` | `36`<br>`37`<br>`38` | Parcelada<br>Faixa Dias<br>Semanal |
| **ParcelamentoArredondamento** | `45` | `46`<br>`47` | Primeira Parcela<br>Última Parcela |
| **Disponibilidade** | `55` | `56`<br>`57`<br>`101` | Pagamento (Contas a Pagar)<br>Recebimento (Contas a Receber)<br>Ambas |
| **VencimentoTipo** | `58` | `59`<br>`60`<br>`61` | Normal<br>Antecipa Dia Útil<br>Prorroga Dia Útil |
| **ParcelamentoTipo** | `118` | `119`<br>`120`<br>`693` | Dia Fixo<br>Dias Entre Parcela<br>Dias Úteis |
| **DiaSemana** | `163` | `164`<br>`165`<br>`166`<br>`167`<br>`168`<br>`169`<br>`170` | Domingo<br>Segunda<br>Terça<br>Quarta<br>Quinta<br>Sexta<br>Sábado |
| **DivisaoParcelamentoTipo** | `602` | `603`<br>`604` | Percentual<br>Quantidade |

---

## 4. Colunas da Listagem (Grid)

| Coluna | Campo interno | Observação |
|---|---|---|
| Código | `idCondicaoPagamento` | Identificador único |
| Descrição | `descricao` | Nome / descrição amigável |
| Tipo | `idTipoCondicaoPagto` | Exibe descrição do enum (ex: "Parcelada") |
| Disponibilidade | `idDisponibilidade` | Exibe "Pagamento", "Recebimento" ou "Ambas" |
| Situação | `ativo` | Exibe "Ativo" ou "Inativo" (Chips coloridos) |

---

## 5. Campos do Formulário e Regras de Interface

O formulário é composto por um painel de dados gerais no cabeçalho e abas condicionais com base no campo **Tipo Condição Pagamento** (`idTipoCondicaoPagto`).

### 5.1 Dados Gerais (Cabeçalho)

| Campo | Tipo | Obrigatório | Regras e Comportamento |
|---|---|---|---|
| Código | Texto | Readonly | Mapeia para `idCondicaoPagamento` |
| Descrição | Texto | ✅ | Mínimo 3 caracteres |
| Tipo Condição Pagto | Select | ✅ | Valores de `CondicaoPagtoTipo` (Muda as abas visíveis) |
| Disponibilidade | Select | ✅ | Valores de `Disponibilidade` |
| Vencimento em Dia Útil | Select | ✅ | Valores de `VencimentoTipo` |
| Grupo Condição Pagto | Select (Lookup) | — | Busca da tabela/API `GloGrupoCondicaoPagamento` |
| Forma de Cobrança | Select (Lookup) | — | Busca da tabela/API `GloFormaCobranca` |
| Forma de Pagamento | Select (Lookup) | — | Busca da tabela/API `GloFormaPagamento` |
| Ordem de Consulta | Número | ✅ | Padrão `0` (Intervalo 0 a 100) |
| Utilizar no PDV | Switch/Checkbox | — | Padrão `false` |
| Ativo | Switch/Checkbox | — | Padrão `true` |

### 5.2 Regras de Acréscimo e Desconto (Card lateral/abaixo)

| Campo | Tipo | Comportamento |
|---|---|---|
| Recebe Acréscimo | Checkbox | Exclusivo com "Recebe Desconto". Se marcado, ativa campo Acréscimo. |
| Acrescimo (%) | Decimal | Só habilitado se "Recebe Acréscimo" for true. Valor entre 0 e 99.99%. |
| Recebe Desconto | Checkbox | Exclusivo com "Recebe Acréscimo". Se marcado, ativa campo Desconto. |
| Desconto (%) | Decimal | Só habilitado se "Recebe Desconto" for true. Valor entre 0 e 99.99%. |

---

### 5.3 Abas Condicionais

#### Aba 1: Parcelamento (Visível se `idTipoCondicaoPagto` = `Parcelada` [36])

* **Configurações Gerais de Parcelamento**:
  - **Condição do tipo livre (altera parcelas)**: Checkbox (`alteraParcelas`). Se marcado, define `quantidadeParcela = 0`, `tipoDivisao = Quantidade (604)` e `arredondamento = Ultima (47)`.
  - **Permitir alterar número da parcela**: Checkbox (`alteraNroParcela`).
  - **Tipo parcelamento**: Select (`idParcelamentoTipo`). Se `quantidadeParcela <= 1`, força `idParcelamentoTipo` como `DiasEntreParcela (120)`.
  - **Tipo de divisão**: Select (`tipoDivisao`). Opções do enum `DivisaoParcelamentoTipo`.
  - **Quantidade parcelas**: SpinEdit (`quantidadeParcela`).
  - **Dias entre parcelas / Dia fixo / Dia útil**: Campo cujo label e significado variam com o `idParcelamentoTipo` (ex.: "Dias entre parcelas", "Dia útil mês" ou "Dia fixo mês").
  - **Usar mês comercial? (30 dias)**: Checkbox (`usarMesComercial`). Só permitido para parcelamento do tipo `DiasEntreParcela` com dias entre parcelas igual a `30`.
  - **Dias limite vencimento no mesmo mês**: SpinEdit (`diasMinimoProximoMes`). Só editável se `idParcelamentoTipo = DiasEntreParcela`.
  - **Primeira parcela à vista**: Checkbox (`primeiraParcelaAVista`). Se marcado, habilita o campo "Forma de pagamento padrão da 1ª parcela" (`idFormaPagamentoVista`) e define `obrigatorioFormaPagamento = true`.

* **Grade de Parcelas**:
  - Grid editável exibindo as parcelas geradas. Colunas:
    - Nº Parcela (`numeroParcela`) - Readonly
    - Dias (`numeroDias`) - Habilitado
    - Liberado (`diasLiberado`) - Habilitado
    - % Divisão (`percentualDivisao`) - Habilitado se `tipoDivisao = Percentual (603)` e `alteraParcelas = false`.
    - % Valor Mínimo (`percentualValorMinimo`) - Habilitado

#### Aba 2: Faixa (Visível se `idTipoCondicaoPagto` = `FaixaDias` [37])

* **Configurações Gerais**:
  - **Quantidade Faixas**: SpinEdit (`quantidadeFaixa`).
* **Grade de Faixas**:
  - Grid contendo faixas de dias do mês. Colunas:
    - Faixa nº (`numeroParcela`) - Readonly
    - Inicial (`diaInicial`) - Dia do mês
    - Final (`diaFinal`) - Dia do mês
    - Dia Vencimento (`numeroDias`) - Dia do vencimento

#### Aba 3: Semanal (Visível se `idTipoCondicaoPagto` = `Semanal` [38])

* **Configurações Gerais**:
  - **Dia da semana**: Select (`idDiaSemana`). Opções do enum `DiaSemana`.

---

## 6. Regras de Negócio e Validações (Back-end & Front-end)

| Nº | Regra de Validação | Severidade |
|---|---|---|
| **1** | **Semanal - Dia da Semana**: Se o tipo for `Semanal (38)`, deve ser informado o dia da semana (`idDiaSemana` não pode ser nulo ou zero). | ❌ Bloqueia |
| **2** | **Parcelada - Quantidade de Parcelas**: Se o tipo for `Parcelada (36)` e não for condição livre, a quantidade de parcelas deve ser maior que zero. | ❌ Bloqueia |
| **3** | **Parcelada - Grade de Parcelas**: A quantidade de linhas no grid de parcelas deve bater com a propriedade `QuantidadeParcelas` (quando não livre). | ❌ Bloqueia |
| **4** | **Parcelada - Percentual Total**: Se o tipo de divisão for `Percentual (603)` (e não livre), a soma dos percentuais de divisão das parcelas deve ser exatamente `100%`. | ❌ Bloqueia |
| **5** | **Parcelada - Condição Livre**: Se for condição do tipo livre (`alteraParcelas = true`), o arredondamento deve ser definido como `Ultima Parcela (47)` e o tipo de parcelamento não pode ser `Dias Uteis`. A grade de parcelas deve conter apenas 1 linha configurável. | ❌ Bloqueia |
| **6** | **Parcelada - Mês Comercial**: Para usar mês comercial (`usarMesComercial = true`), o parcelamento deve ser obrigatoriamente do tipo `DiasEntreParcela` com `dias = 30`. | ❌ Bloqueia |
| **7** | **Parcelada - Primeira à Vista**: Se marcar primeira parcela à vista, o tipo de parcelamento deve ser obrigatoriamente `DiasEntreParcela`. | ❌ Bloqueia |
| **8** | **Faixa de Dias - Validação**: Se o tipo for `FaixaDias (37)`, deve-se definir faixas onde o dia final não é inferior ao dia inicial, e as faixas não podem ter interseções ou dias repetidos entre si, ordenadas em ordem crescente. | ❌ Bloqueia |
| **9** | **Faixa de Dias - Vencimento**: O dia de vencimento de cada faixa deve ser entre 1 e 31, e não pode estar contido no intervalo determinado pela própria faixa. | ❌ Bloqueia |
| **10** | **Limpeza Automática**: Ao salvar, campos de tipos não ativos devem ser limpos: se parcelada, limpa faixas e semanal; se faixa, limpa semanal e parcelas; se semanal, limpa parcelas e faixas. | Auto |

---

## 7. Arquivos Criados / Modificados

* **Backend**:
  - `src/Versatus.AcessoGlobal/Domain/Finance/CondicaoPagamento.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Finance/CondicaoPagtoRegra.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Finance/GrupoCondicaoPagamento.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Finance/FormaCobranca.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Infrastructure/Mappings/CondicaoPagamentoMapping.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Infrastructure/Mappings/CondicaoPagtoRegraMapping.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Infrastructure/Mappings/GrupoCondicaoPagamentoMapping.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Infrastructure/Mappings/FormaCobrancaMapping.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Services/ICondicaoPagamentoService.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Services/CondicaoPagamentoService.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Api/Controllers/CondicaoPagamentoController.cs` [NEW]
  - `src/Versatus.AcessoGlobal/Domain/Entities/Enums.cs` [MODIFY]
  - `src/Versatus.AcessoGlobal/Infrastructure/AcessoGlobalDbContext.cs` [MODIFY]
* **Frontend**:
  - `src/Versatus.Frontend/src/pages/AcessoGlobal/FCondicaoPagamento/types.ts` [NEW]
  - `src/Versatus.Frontend/src/pages/AcessoGlobal/FCondicaoPagamento/schema.ts` [NEW]
  - `src/Versatus.Frontend/src/pages/AcessoGlobal/FCondicaoPagamento/CondicaoPagamentoCadastroConfig.tsx` [NEW]
  - `src/Versatus.Frontend/src/pages/AcessoGlobal/FCondicaoPagamento/index.tsx` [NEW]
