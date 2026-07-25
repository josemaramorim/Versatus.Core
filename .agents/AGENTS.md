# Diretrizes e Leis do Projeto Versatus.Net8

Este arquivo define as regras e restrições fundamentais que guiam todos os agentes de IA que atuam neste repositório. O não cumprimento destas regras constitui uma quebra de contrato.

---

## 1. Princípios Arquiteturais e Boas Práticas (.NET 10, SOLID, Clean Architecture, Clean Code)

Todo código gerado ou alterado no repositório DEVE seguir rigorosamente as premissas arquiteturais modernas:

- **Clean Architecture (Arquitetura em Camadas):**
  - **Domain:** Entidades POCO puras, DTOs, Interfaces de Serviço/Repositório e Lógica de Negócio independente de infraestrutura.
  - **Infrastructure:** Mapeamentos EF Core via Fluent API, DbContext e Implementações de Repositório.
  - **Api / Controllers:** Controladores finos e limpos que delegam a execução para os serviços de domínio via DI e retornam retornos HTTP padronizados (`200 OK`, `201 Created`, `400 BadRequest`).
  - **Frontend:** Camada desacoplada usando React OOP (`BaseCadastroConfig<T>`), Zod Schemas e componentes limpos.

- **Princípios S.O.L.I.D:**
  - **S (Single Responsibility):** Cada classe, serviço, controlador ou componente React deve ter uma única responsabilidade clara.
  - **O (Open/Closed):** Extensibilidade via Fluent API no EF Core e herança em `BaseCadastroConfig<T>` sem alterar contratos base.
  - **L (Liskov Substitution):** Subclasses e especificações devem respeitar rigorosamente os contratos da classe base.
  - **I (Interface Segregation):** Interfaces de serviços e repositórios coesas e focadas no seu escopo de domínio.
  - **D (Dependency Inversion):** Controladores e serviços dependem de abstrações/interfaces (`IEntidadeService`, `IRepositorio<T>`), injetadas via DI (.NET IServiceCollection).

- **Clean Code & Sintaxe Moderna do .NET 10:**
  - Nomes expressivos e alinhados ao domínio de negócio.
  - **File-scoped namespaces:** (ex: `namespace Versatus.AcessoGlobal.Domain.Services;`).
  - **Nullable Reference Types habilitados:** Tratar nulos de forma explícita (`int?`, `string?`).
  - Ausência de acoplamento direto, código morto ou exceções de fluxo de controle.

---

## 2. Pureza de Domínio (C# Backend)
- Todas as classes na camada Domain/Entities devem ser **POCOs puras** (Plain Old CLR Objects).
- **Proibido:** Importar System.ComponentModel.DataAnnotations ou usar atributos como [Table], [Column], [Key], [ForeignKey] nas entidades do domínio.
- **Obrigatorio:** Toda a configuração e mapeamento de banco de dados deve ser configurada via **Fluent API** na camada de Infrastructure (ex: DbContext.OnModelCreating).

## 3. Abordagem Spec-First
- Nenhum código deve ser gerado ou alterado sem que a Especificação Funcional correspondente (docs/spec_f[nome].md ou na pasta specs/) esteja gerada e aprovada pelo usuário.
- A IA não deve inventar ou otimizar funcionalidades além do que está documentado na Spec e no código legado.
- Nomes de tabelas e colunas são sagrados: mantenha a nomenclatura idêntica ao legado (ex: EntCliente em vez de pluralizações como Clientes).
- **Obrigatorio:** Antes de gerar a spec, mapear **todas** as propriedades da entidade legada e verificar se cada uma tem correspondência no C# atual (entidade + DTO + mapping EF). Propriedades ignoradas causam retrabalho.
- **Obrigatorio (Auditoria de Propriedades vs UI):** É proibido deixar propriedades editáveis da entidade (como `Ativo`/`Situacao`, flags, observações) de fora da interface gráfica (JSX/Form). Toda propriedade do DTO deve obrigatoriamente ter seu componente de entrada no formulário React, salvo campos puramente internos de auditoria/sistema.

## 4. Arquitetura do Frontend (React/TypeScript)
- Toda listagem e formulário CRUD deve herdar e estender a infraestrutura orientada a objetos (BaseCadastroConfig<T>).
- **Proibido:** Inserir lógica hardcoded do backend, endpoints ou mapeamentos específicos dentro de componentes ou hooks reutilizáveis como useCrudListState.ts.
- **Obrigatorio:** Sobrescrever os métodos mapBackendToForm e mapFormToBackend no arquivo [Nome]CadastroConfig.tsx do formulário quando houver diferenças de enums ou estrutura de dados entre o legado/API e o formulário do React.
- **Obrigatorio:** Organizar as páginas do frontend em subpastas por módulo correspondente ao backend (ex.: src/pages/[Modulo]/F[Nome]/), em vez de manter uma estrutura plana diretamente em src/pages/. Isso espelha a modularização de negócio do sistema.

## 5. Estratégia Git e Proteção de Branches
- **Proibido:** A IA nunca deve fazer commits ou push diretamente nas branches estáveis main ou develop.
- **Obrigatorio:** Todo desenvolvimento deve ser isolado em uma branch de recurso (`feat/migrate-[nome]`).
- Os commits devem ser granulares por fase (Spec, Backend, Frontend, Rota) com mensagens padronizadas.
- **Obrigatorio:** Após o merge bem-sucedido de uma branch de recurso para a branch `develop`, a IA DEVE obrigatoriamente perguntar ao usuário se deseja excluir a branch de recurso (local e remota) e alternar o ambiente de trabalho para `develop`.
- O merge de develop para main é estritamente manual e de responsabilidade exclusiva do usuário.

## 6. Validação com Builds
- Após gerar código no Backend C#, o agente deve rodar obrigatoriamente dotnet build e verificar se há erros de compilação.
- Após gerar código no Frontend, o agente deve rodar npm run build para garantir a integridade das tipagens TypeScript e empacotamento Vite.
- **Obrigatorio:** Antes de rodar dotnet build, verificar se o servidor (dotnet run) está ativo com manage_task list e matá-lo com manage_task kill. O servidor em execução bloqueia os arquivos DLL e causa falha de build.

## 7. Padrões de Tela (Classificação Obrigatória)

Todo formulário migrado deve ser classificado em um dos dois padrões antes de qualquer geração de código:

### Padrão A: CRUD Padrão
- **Exemplos:** FEntidade, FCondicaoPagamento, FProduto
- **Características:** Grid paginado, botões Novo/Editar/Excluir, formulário modal ou lateral
- **Endpoints:** GET paginado, GET/{id}, POST, PUT/{id}, DELETE/{id}
- **Frontend:** Herda BaseCadastroConfig<T> com grid e formulário padrão

### Padrão B: Configuração em Lote
- **Exemplos:** FParametro, FPermissao
- **Características:** Accordion ou TreeList agrupando itens existentes, edição inline, salvamento em lote
- **Endpoints:** GET /escopo (ou equivalente contextual) + PUT /salvar-valores
- **Frontend:** Accordions fechados por padrão; Descrição como label primário, Chave técnica como subtexto
- **Proibido:** Exibir os botões Novo e Excluir em qualquer parte da UI nesse padrão

## 8. Compatibilidade com SQL Server 2008
- O banco de dados legado é SQL Server 2008 e **não suporta** as cláusulas OFFSET/FETCH usadas pelo EF Core para paginação nativa.
- **Obrigatorio:** Para qualquer consulta paginada, buscar todos os registros do filtro com ToListAsync() primeiro e depois paginar em memória com .Skip(offset).Take(limit).
- **Proibido:** Usar .Skip().Take() diretamente sobre um IQueryable<T> que ainda não foi materializado para memória.
- Exemplo correto:
  ```csharp
  var todos = await query.Where(filtros).ToListAsync();
  var total = todos.Count;
  var pagina = todos.Skip((page - 1) * limit).Take(limit).ToList();
  ```

## 9. Tratamento de Erros e Validação de Negócio (Result Pattern & Clean Code)
- **Proibido:** A IA não deve lançar exceções (`throw new ...Exception(...)`) para sinalizar falhas de validação de dados ou quebra de regras de negócio esperadas.
- **Obrigatorio:** Toda validação de negócio e persistência nos serviços do domínio deve utilizar o padrão de retorno funcional `Result<T>` ou `ValidationResult` (do namespace `Versatus.Framework.Validation`) encapsulando erros estruturados em `ValidationError`. Os controladores (Controllers) devem verificar a flag `IsSuccess`, retornando `400 BadRequest` com a lista detalhada de erros caso falhe.

## 10. Sinalização Visual de Campos Obrigatórios (Frontend MUI)
- **Obrigatorio:** Todo campo marcado como obrigatório na Spec Funcional DEVE receber a prop `required` no componente MUI correspondente (`TextField`, `FormControl`, `Select`).
- O MUI exibe automaticamente o asterisco vermelho (`*`) no label quando a prop `required` está presente, sinalizando ao usuário que o campo é obrigatório antes mesmo de tentar salvar.
- **Proibido:** Omitir a prop `required` nos campos obrigatórios, deixando o usuário descobrir a obrigatoriedade apenas após falha de validação no submit.
- A prop `required` do MUI é puramente cosmética e não substitui a validação Zod/React Hook Form — ambas devem coexistir.
- Exemplo correto para `TextField`:
  ```tsx
  <TextField required label="Descrição" error={!!error} helperText={error?.message} />
  ```
- Exemplo correto para `FormControl` (Select):
  ```tsx
  <FormControl fullWidth required error={!!errors.idTipoCondicaoPagto}>
    <InputLabel>Tipo Condição</InputLabel>
  ```

## 11. Arquitetura de Banco de Dados CQRS (Leitura vs. Escrita)
- **Obrigatorio:** O projeto adota a segregação de conexões de banco de dados (CQRS Leve):
  - **Mutações / Alteração de Estado (`POST`, `PUT`, `DELETE`):** Executadas na conexão principal de escrita (`WriteConnection` / `AcessoGlobalDbContext` / `TributoDbContext`).
  - **Consultas / Paginações / Lookups (`GET`):** Executadas na conexão de leitura desabilitada de tracking (`ReadConnection` / `AcessoGlobalReadDbContext` / `TributoReadDbContext`).
- Os repositórios herdados de `AcessoGlobalRepositorioBase<TEntity>` utilizam automaticamente `ReadContext` / `ReadDbSet` para leitura e `Context` / `DbSet` para escrita.
- Configuração de conexão via variáveis `WriteConnection` e `ReadConnection` no `appsettings.json` / ICP.
