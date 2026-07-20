# Spec Funcional: Cadastro de Parâmetro (FParametro)

> **Tipo:** Spec Funcional  
> **Versão:** 1.0  
> **Módulo:** AcessoGlobal  
> **Baseado em:** [spec_fentidade.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/docs/spec_fentidade.md)

---

## 1. Objetivo

Gerenciar as configurações e parâmetros globais do sistema. Cada parâmetro possui uma chave identificadora única (Nome/Chave), uma descrição funcional, e um valor configurado que determina comportamentos de regras de negócio em diversos módulos do ERP (como obrigatoriedade de CPF/CNPJ, tolerância de duplicidade, etc.).

---

## 2. Endpoints da API

| Método | Rota | Função |
|---|---|---|
| `GET` | `/api/parametro/paginado?page&limit&sortBy&sortOrder&search` | Listagem paginada de parâmetros com filtros por chave/descrição |
| `GET` | `/api/parametro/{id}` | Obter parâmetro simples por ID |
| `GET` | `/api/parametro/completo/{id}` | Obter parâmetro completo (com o valor configurado) |
| `POST` | `/api/parametro` | Criar novo parâmetro (gerando `IdGloParametro` e `IdGloParametroValor`) |
| `PUT` | `/api/parametro/{id}` | Atualizar parâmetro (permite alterar apenas `Valor` e `Descricao`) |
| `DELETE` | `/api/parametro/{id}` | Remover parâmetro do sistema |

---

## 3. Conversão de Dados (Legado vs. Frontend)

> [!IMPORTANT]
> A chave primária da tabela `GloParametro` é `IdGloParametro`. No frontend, usaremos a propriedade genérica `id` (mapeada a partir de `IdParam` no DTO) para compatibilidade com o hook base `useCrudListState.ts`.
> Os valores de parâmetros de tipo lógico (`Smallint = 156`) devem ser mapeados de/para booleanos no formulário React.

### Tipagem do Valor (IdTipoValor)
Os valores de configuração dos tipos suportados pelo legado são mapeados conforme abaixo:

| IdTipoValor (Banco) | Tipo (Frontend) | Descrição |
|---|---|---|
| `153` | `Int` | Inteiro |
| `154` | `Numeric` | Decimal / Numérico |
| `155` | `String` | Texto livre |
| `156` | `Smallint` | Booleano (Sim/Não) |
| `157` | `DateTime` | Data |
| `233` | `Lookup` | Busca externa (Chave estrangeira) |
| `234` | `Enumerado` | Tipo Enumerado do sistema |
| `374` | `Automatico` | Inferência automática pelo sistema |
| `1325` | `LookupMulti` | Múltiplas chaves estrangeiras |

---

## 4. Colunas da Listagem (Grid)

| Coluna | Campo interno | Observação |
|---|---|---|
| Chave | `chave` | Mapeia para o nome/chave única do parâmetro (ex.: `CPFCNPJOBRIGATORIO`) |
| Descrição | `descricao` | Descrição funcional resumida do parâmetro |
| Valor | `valor` | Exibe o valor configurado atualmente (se houver) |

---

## 5. Campos do Formulário

| Campo | Tipo | Obrigatório | Regra |
|---|---|---|---|
| Chave | Texto | ✅ | Chave única (Nome). **Bloqueado para edição** se o modo for Edição (`mode === 'edit'`). |
| Descrição | Texto (Multiline) | ❌ | Detalhamento sobre a utilidade do parâmetro. |
| Valor | Texto / Select / Switch | ❌ | Entrada do valor. Pode ser adaptado dinamicamente com base no `tipo` se desejado, ou exibido como campo de texto livre padrão. |
| Tipo | Select | ✅ | Define o comportamento do valor (padrão: `155 = String`). |

---

## 6. Regras de Negócio (Back-end)

| Nº | Regra | Severidade |
|---|---|---|
| 1 | **Chave Única:** Não é permitido duplicar o nome da chave (`Chave` / `Nome`) no banco de dados. | ❌ Bloqueia |
| 2 | **Edição Bloqueada de Chave:** Em operações de atualização (`PUT`), a chave do parâmetro não pode ser alterada. | ❌ Bloqueia |
| 3 | **Geração de Sequenciais:** A criação de `Parametro` deve gerar `IdParam` usando o gerador sequencial `"Parametro"`. A criação/atualização de valor deve gerar `IdParametroValor` usando `"ParametroValor"`. | Auto |
| 4 | **Sincronização com Cache de Servidor:** Após persistir ou excluir o parâmetro, deve ser acionado o serviço de cache global para atualizar o parâmetro em memória. | Auto |

---

## 7. Filtros disponíveis na listagem

| Filtro | Tipo | Campo Interno | Observação |
|---|---|---|---|
| Chave | Texto | `codigo` | Mapeado no `useCrudListState` como campo `search` |
| Descrição | Texto | `razaoSocial` | Mapeado no `useCrudListState` como campo `search` |

> [!NOTE]
> Como o hook `useCrudListState.ts` consolida os termos no parâmetro genérico `search`, o backend irá realizar uma busca case-insensitive por correspondência parcial contida tanto em `Chave` (Nome) quanto em `Descricao`.

---

## 8. Arquivos Relacionados

- **Types e defaultValues:** [types.ts](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FParametro/types.ts)
- **Validação Zod:** [schema.ts](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FParametro/schema.ts)
- **Config OOP:** [ParametroCadastroConfig.tsx](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FParametro/ParametroCadastroConfig.tsx)
- **Formulário / View:** [index.tsx](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FParametro/index.tsx)
- **Controller C#:** [ParametroController.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Api/Controllers/ParametroController.cs)
- **DTOs C#:** [ParametroDtos.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/DTOs/ParametroDtos.cs)
- **Serviço C#:** [ParametroService.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/Services/ParametroService.cs)
- **Interface Serviço:** [IParametroService.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/Services/IParametroService.cs)
- **Repositório:** [SpecializedRepositories.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Infrastructure/Repositories/SpecializedRepositories.cs)

---

## 9. Dúvidas / Divergências

> [!NOTE]
> Não há pontos incertos mapeados até o momento.
