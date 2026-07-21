# Guia de Escalabilidade: Migração de Telas CRUD com IA

Este documento serve como referência rápida para planejar e guiar a IA (como Antigravity) na migração dos mais de 100 formulários do sistema legado para a nova arquitetura do projeto.

---

## 📌 Sumário
1. [O Padrão de Ouro (Golden Pattern)](#-o-padrão-de-ouro-golden-pattern)
2. [Configuração das Regras do Agente (AGENTS.md)](#-configuração-das-regras-do-agente-agentsmd)
3. [Criação do Skill Customizado (migrate-crud)](#-criação-do-skill-customizado-migrate-crud)
4. [Instrução de Execução (Modelo de Prompt)](#-instrução-de-execução-modelo-de-prompt)
5. [Checklist de Validação](#-checklist-de-validação)

---

## 🏆 O Padrão de Ouro (Golden Pattern)

Para cada nova tela de cadastro (CRUD), a estrutura de arquivos e responsabilidades **deve seguir exatamente o modelo implementado no cadastro de Entidade**:

### Estrutura do Frontend (React + TS + Vite)
* **`src/pages/[Modulo]/F[Nome]/index.tsx`**: Contém apenas a camada de apresentação visual (Layout, Tabs, Componentes Material UI).
* **`src/pages/[Modulo]/F[Nome]/types.ts`**: Define o schema de dados da interface do formulário e os valores padrões (`defaultValues`).
* **`src/pages/[Modulo]/F[Nome]/schema.ts`**: Contém o schema do **Zod** para validação em tempo real dos campos.
* **`src/pages/[Modulo]/F[Nome]/[Nome]CadastroConfig.tsx`**: Estende `BaseCadastroConfig<T>`. É a única classe que define o título, a URL da API, as colunas da listagem, os filtros e, principalmente, as funções de tradução de dados:
  * `mapBackendToForm(backend: any)`: Traduz enums e dados aninhados da API para o formulário.
  * `mapFormToBackend(form: T)`: Traduz o formulário plano de volta no formato esperado pela API.

### Estrutura do Back-end (.NET 10 WebAPI)
* **DTOs (`Salvar[Nome]Dto.cs`)**: Estrutura plana que recebe os dados do Frontend.
* **Service (`[Nome]Service.cs`)**: Implementa a validação das regras de negócio e a persistência no banco.
* **Controller (`[Nome]Controller.cs`)**: Expõe os endpoints seguindo o padrão REST:
  * `[HttpGet("paginado")]`: Para a listagem paginada da grid.
  * `[HttpGet("completo/{id}")]`: Para carregar detalhes do registro.
  * `[HttpPost]`: Para criar.
  * `[HttpPut("{id}")]`: Para atualizar.
  * `[HttpDelete("{id}")]`: Para remover.

---

## 🤖 Configuração das Regras do Agente (AGENTS.md)

Crie um arquivo chamado **`.agents/AGENTS.md`** na raiz do projeto. Cole o conteúdo abaixo nele para orientar qualquer agente de IA que trabalhar no seu repositório:

```markdown
# Diretrizes de Desenvolvimento do Projeto

## 1. Padrão Arquitetural de Telas CRUD
Todas as novas telas de cadastro devem herdar o padrão OOP implementado em `FEntidade`:
* Proibido injetar lógicas de mapeamento de dados (como formatação de enums ou aninhamentos) diretamente em hooks como `useCrudListState.ts` ou componentes React.
* Todo o mapeamento deve ser feito através da sobrescrita dos métodos `mapBackendToForm` e `mapFormToBackend` na classe correspondente herdada de `BaseCadastroConfig`.
* O hook `useCrudListState.ts` deve permanecer genérico.

## 2. Padrão de Comunicação de API
* Toda requisição fetch do frontend deve passar os cabeçalhos retornados por `getApiHeaders()` localizado em `src/config/api.ts` para garantir a injeção da chave de autenticação `X-Api-Key`.
* URLs da API devem ser montadas usando a função `buildApiEndpoint()` do mesmo arquivo.
```

---

## 🛠️ Criação do Skill Customizado (migrate-crud)

Crie um arquivo chamado **`.agents/skills/migrate-crud/SKILL.md`** para registrar a habilidade. Ela ensinará a IA como proceder passo a passo:

```markdown
---
name: "migrate-crud"
description: "Gera e migra telas CRUD completas do legado para .NET 10 + React OOP"
---

# Fluxo de Migração Sistemática de CRUD

Ao receber uma solicitação de migração de tela, execute as fases abaixo de forma ordenada:

1. **Fase de Análise:** Solicite o script SQL (DDL) da tabela correspondente ou o código do back-end legado.
2. **Fase de DTOs e Back-end:** Crie os DTOs de salvamento e obtenção, o serviço de negócio com regras de validação e a controller C# seguindo o padrão REST de `EntidadeController.cs`.
3. **Fase de Mapeamento (Front):** Crie o arquivo `types.ts` e o validador Zod em `schema.ts`.
4. **Fase de Configuração (Front):** Crie a classe herdada de `BaseCadastroConfig` e implemente os conversores bidirecionais `mapBackendToForm` e `mapFormToBackend` caso existam enums legados incompatíveis.
5. **Fase de UI (Front):** Crie os arquivos de abas e layout em `pages/F<Nome>/index.tsx`.
6. **Fase de Validação:** Rodar `dotnet build` e `npm run build` para garantir que nada quebrou.
```

---

## 📋 Instrução de Execução (Modelo de Prompt)

Toda vez que você for pedir à IA para fazer uma nova tela, copie, altere os dados entre colchetes `[ ]` e envie o seguinte prompt no chat:

> "Quero migrar o cadastro de **[Nome da Entidade]** utilizando o skill de migração do projeto.
>
> **1. Dados do Banco de Dados Legado:**
> [Cole o script DDL da tabela ou a classe de persistência antiga aqui]
>
> **2. Regras de Negócio Importantes:**
> * [Ex: 'O campo CNPJ é obrigatório se for Pessoa Jurídica']
> * [Ex: 'O campo parcelas deve validar se o valor é maior que zero']
>
> Siga o Golden Pattern da `FEntidade` para a divisão de arquivos e mapeamentos OOP no frontend."

---

## 🔍 Checklist de Validação

Antes de considerar a migração de uma tela concluída, garanta que:
* [ ] Os tipos de dados do banco de dados (ex: enums como `1, 2, 3`) estão convertidos corretamente para os selects e listagens na tela.
* [ ] O formulário possui validação em tempo real utilizando o Zod Schema.
* [ ] Os botões de salvar e editar chamam a URL correta com o `id` da rota no PUT (`/api/entidade/{id}`).
* [ ] O código do frontend compilou 100% sem erros de TypeScript (`npm run build`).
* [ ] O back-end compilou e restaurou as dependências (`dotnet build`).
