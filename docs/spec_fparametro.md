# Spec Funcional: Configuracao de Parametros (FParametro)

> **Tipo:** Spec Funcional  
> **Versao:** 2.0  
> **Modulo:** AcessoGlobal  
> **Status:** Implementado e Compilado

---

## 1. Objetivo

Permitir que o usuario **visualize e altere** os valores dos parametros globais do sistema, organizados por **escopo** (Sistema, Filial, Perfil, Grupo, Empresa) e agrupados em categorias funcionais (Accordion expansivel). A tela e **exclusivamente de edicao em lote** - nao e possivel criar ou excluir parametros pela interface.

> [!IMPORTANT]
> **Os botoes Novo e Excluir sao completamente ocultos nessa tela.** O usuario pode apenas alterar o valor de parametros existentes. A persistencia e feita em lote via botao Confirmar (Salvar).

---

## 2. Divergencia Fundamental com a Spec v1.0

> [!WARNING]
> A spec v1.0 descrevia um CRUD padrao com paginacao, criacao e exclusao. Isso foi **abandonado** pois nao reflete o comportamento do desktop legado (WinForms). A tela real do legado (FParametro.cs) e uma **TreeList de configuracao em lote**, nao um grid CRUD.

O comportamento correto implementado e:
- Selecao de escopo no topo
- Parametros carregados de GloParametro + GloParametroValor
- Edicao inline de cada valor
- Salvamento em lote de todas as alteracoes

---

## 3. Estrutura do Banco de Dados

### Tabela GloParametro (definicao do parametro)

| Coluna BD | Propriedade C# | Descricao |
|---|---|---|
| IdGloParametro | IdParam | PK |
| Nome | Chave | Chave unica identificadora (ex: CPFCNPJOBRIGATORIO) |
| Descricao | Descricao | Descricao funcional do parametro |
| Objeto | Valor | Valor padrao/template |
| IdTipoValor | Tipo | Tipo do campo (ver tabela de tipos) |
| Agrupador | Agrupador | ID numerico da categoria de exibicao (1-59) |
| Visivel | Visivel | Se o parametro deve ser exibido na tela |
| IdGloRotina | IdRotina | FK para a rotina de menu ao qual pertence |
| IdTipoParametro | TipoParametro | FK para o escopo (Sistema=159, Filial=160, etc.) |

### Tabela GloParametroValor (valor personalizado por escopo)

| Coluna BD | Descricao |
|---|---|
| IdGloParametroValor | PK |
| IdGloParametro | FK para o parametro |
| Valor | Valor personalizado (override do padrao) |
| Marcado | true se este parametro esta ativado para o escopo |
| IdGloFilial | Preenchido para escopos Filial e Perfil |
| IdGloEmpresa | Preenchido para escopos Empresa, Filial e Perfil |
| IdGloGrupo | Preenchido para escopo Grupo |
| IdGloPerfil | Preenchido **apenas** para escopo Perfil |

---

## 4. Escopo de Parametros (TipoParametro)

| ID | Label | Comportamento |
|---|---|---|
| 159 | Sistema | Parametros globais do sistema. Nao requer selecao adicional. |
| 160 | Filial | Parametros especificos por filial. |
| 161 | Perfil de Acesso | Requer selecao de um perfil via dropdown carregado de /api/parametro/perfis. |
| 350 | Grupo | Parametros especificos por grupo. |
| 351 | Empresa | Parametros especificos por empresa. |

---

## 5. Endpoints da API (Implementados)

| Metodo | Rota | Funcao |
|---|---|---|
| GET | /api/parametro/escopo?tipoParametro=&idPerfil= | Carrega todos os parametros visiveis (Visivel=true) do escopo com valor personalizado |
| PUT | /api/parametro/salvar-valores | Persiste a lista de alteracoes em lote em GloParametroValor |
| GET | /api/parametro/perfis | Lista os perfis de acesso disponiveis (GloPerfil) |
| GET | /api/parametro/enum-opcoes?enumNome= | Retorna as opcoes de um enum dinamico por nome |

> [!NOTE]
> Os endpoints CRUD originais (GET paginado, POST, DELETE) continuam existentes no controller para uso interno, mas **nao sao usados pela tela de configuracao**.

---

## 6. Tipagem do Valor (IdTipoValor)

| ID no banco | Tipo | Componente React | Observacao |
|---|---|---|---|
| 153 | Int | TextField type=number step=1 | Inteiro |
| 154 | Numeric | TextField type=number step=0.01 | Decimal |
| 155 | String | TextField | Texto livre (padrao) |
| 156 | Smallint | Select com Sim/Nao | Booleano |
| 157 | DateTime | TextField type=date | Data |
| 233 | Lookup | TextField (codigo FK) | Busca externa |
| 234 | Enumerado | EnumField com chamada a /api/parametro/enum-opcoes | Enum dinamico |
| 374 | Automatico | TextField | Inferencia automatica |
| 1325 | LookupMulti | TextField | Multiplas FKs |

---

## 7. Agrupadores (Coluna AGRUPADOR)

A coluna AGRUPADOR e um inteiro sequencial (1-59) que determina a **categoria funcional** de cada parametro. No frontend, sao mapeados para nomes amigaveis no dicionario AGRUPADOR_NAMES em index.tsx:

| ID | Nome Amigavel |
|---|---|
| 1 | Centro de Custos |
| 2 | Movimentacoes Retroativas |
| 3 | Portadores de Cobranca |
| 4 | Contas a Receber |
| 5 | Contas a Pagar |
| 6 | Cheques Recebidos |
| 7 | Venda Balcao e PDV |
| 8 | Caixa e Tesouraria |
| 9 | Baixa e Liquidacao (Pagar) |
| 10 | Baixa e Liquidacao (Receber) |
| 11 | Conta Caixa Padrao |
| 12 | Conta Banco Padrao |
| 13 | Rateio de Classe |
| 14 | Impressao de Cheques |
| 15 | Suprimento de Caixa |
| 16 | Sangria de Caixa |
| 17 | Dominio Padrao |
| 18 | Dominio Financeiro |
| 19 | Cheques Emitidos |
| 20 | Fechamento de Caixa |
| 21 | Indices Economicos |
| 22 | Fluxo de Caixa |
| 23 | Requisicoes de Materiais |
| 24 | Devolucoes de Materiais |
| 25 | Estornos e Reversoes (Pagar) |
| 26 | Estornos e Reversoes (Receber) |
| 27 | Grade de Estoque |
| 28 | Matriz e Filiais |
| 29 | Imposto de Renda |
| 30 | Documentos Financeiros |
| 31 | Cadastros Gerais |
| 32 | Filtros de Liquidacao |
| 33 | Transferencias entre Filiais |
| 34 | Controle de Lote |
| 35 | Controle de Serie |
| 36 | Localizacao Fisica no Estoque |
| 37 | Venda Completa |
| 38 | Cliente Consumidor Padrao |
| 39 | Vencimento de Parcelas (Orcamentos) |
| 40 | Vencimento de Parcelas (Condicional) |
| 41 | Vencimento de Parcelas (Pedidos) |
| 42 | Vencimento de Parcelas (Vendas) |
| 43 | Estrutura e Composicao PAI |
| 44 | Descontos e Acrescimos Automaticos |
| 45 | Acao de Desconto (Grupos Iguais) |
| 46 | Acao de Desconto (Grupos Diferentes) |
| 47 | Calculos e Arredondamentos |
| 48 | Notas Fiscais Eletronicas (NFe/NFC-e) |
| 49 | Emissao de Boletos |
| 50 | Fiscal (Partilha de ICMS) |
| 51 | Unidades de Medida |
| 52 | Sugestao de Consumidor na Venda |
| 53 | Fiscal (Simples Nacional) |
| 54 | Fiscal (Credito Simples Nacional) |
| 55 | Organizacao de Gondolas (Base) |
| 56 | Organizacao de Gondolas (Secundaria) |
| 57 | Fiscal (Validacao de NCM) |
| 58 | Fiscal (Origem de Mercadoria) |
| 59 | Integracao com Balancas |

> [!NOTE]
> IDs sem mapeamento exibem o label generico Modulo de Configuracao {id}. Novos agrupadores devem ser adicionados ao dicionario AGRUPADOR_NAMES em index.tsx.

---

## 8. Layout da Tela

### Barra Superior
- **Botao Confirmar (Salvar)**: habilitado apenas quando ha alteracoes pendentes. Envia PUT /api/parametro/salvar-valores.
- **Botao Cancelar**: reverte todas as alteracoes locais para o estado carregado da API.

### Filtros / Seletores
- **Definido por** (Escopo): dropdown com os 5 escopos
- **Perfil**: visivel apenas quando Escopo = Perfil (161)
- **Busca por Chave**: filtro local em tempo real
- **Busca por Descricao**: filtro local em tempo real

### Lista de Parametros
- Agrupados por Agrupador em Accordion **fechados por padrao**
- Cada item exibe:
  - Checkbox Ativar (marcado)
  - **Descricao** em destaque + Chave tecnica como subtexto abaixo
  - Campo de edicao inline condicionado ao tipo do parametro

---

## 9. Regras de Negocio

| N | Regra | Severidade |
|---|---|---|
| 1 | Apenas parametros com Visivel = true sao exibidos | Auto |
| 2 | Se marcado = false, o campo de valor e desabilitado e limpo | Auto |
| 3 | Para escopo Perfil (161), a tela requer selecao do perfil antes de carregar | UI |
| 4 | O salvamento e em lote; somente os parametros modificados sao enviados | Performance |
| 5 | Botoes Novo e Excluir sao **completamente ocultados** | Bloqueado |
| 6 | A criacao ou exclusao de parametros e responsabilidade do backend/DBA, nao da UI | Fora de escopo |

---

## 10. Arquivos Relacionados

- **Tela Principal:** src/pages/AcessoGlobal/FParametro/index.tsx
- **Types:** src/pages/AcessoGlobal/FParametro/types.ts
- **Schema Zod:** src/pages/AcessoGlobal/FParametro/schema.ts
- **Config OOP:** src/pages/AcessoGlobal/FParametro/ParametroCadastroConfig.tsx
- **Controller C#:** src/Versatus.AcessoGlobal/Api/Controllers/ParametroController.cs
- **DTOs C#:** src/Versatus.AcessoGlobal/Domain/DTOs/ParametroDtos.cs
- **Servico C#:** src/Versatus.AcessoGlobal/Domain/Services/ParametroService.cs
- **Mapeamento EF:** src/Versatus.AcessoGlobal/Infrastructure/Mappings/ParametroMapping.cs

---

## 11. Licoes Aprendidas (Padrao para Futuras Migracoes)

> [!TIP]
> Essas descobertas devem orientar a migracao de outros formularios similares no sistema.

1. **Nem todo formulario e um CRUD padrao.** O FParametro e uma tela de configuracao em lote - nao herda de CadastroBasePage. Verifique o comportamento real do legado antes de assumir o padrao CRUD.

2. **AGRUPADOR e ordem de exibicao, nao uma FK.** No legado esse campo era indice de classificacao visual (colOrder do TreeList). O mapeamento amigavel deve ser mantido no frontend.

3. **O escopo determina qual tabela de valor consultar.** GloParametroValor contem as colunas IdGloFilial, IdGloEmpresa, IdGloPerfil e IdGloGrupo para filtrar o valor correto por contexto.

4. **Tipos de enumerados dinamicos precisam de lookup em GloTipoEnumerado.** Para parametros do tipo Enumerado (234), o campo Objeto contem o nome do enum C#. As opcoes devem ser buscadas dinamicamente via /api/parametro/enum-opcoes.

5. **dotnet build falha se dotnet run estiver ativo.** Sempre matar a task do servidor antes de compilar para evitar lock nos DLLs.

6. **SQL Server 2008 nao suporta OFFSET/FETCH.** Para paginacao compativel, busque para memoria com ToListAsync() antes de paginar com Skip().Take().

7. **Propriedades faltantes causam incompatibilidade de tela.** Sempre verificar todas as propriedades do legado (Agrupador, Visivel, IdRotina, TipoParametro) antes de implementar o frontend.
