# Diretrizes e Leis do Projeto Versatus.Net8

Este arquivo define as regras e restrições fundamentais que guiam todos os agentes de IA que atuam neste repositório. O não cumprimento destas regras constitui uma quebra de contrato.

---

## 1. Pureza de Domínio (C# Backend)
- Todas as classes na camada Domain/Entities devem ser **POCOs puras** (Plain Old CLR Objects).
- **Proibido:** Importar System.ComponentModel.DataAnnotations ou usar atributos como [Table], [Column], [Key], [ForeignKey] nas entidades do domínio.
- **Obrigatorio:** Toda a configuração e mapeamento de banco de dados deve ser configurada via **Fluent API** na camada de Infrastructure (ex: DbContext.OnModelCreating).

## 2. Abordagem Spec-First
- Nenhum código deve ser gerado ou alterado sem que a Especificação Funcional correspondente (docs/spec_f[nome].md ou na pasta specs/) esteja gerada e aprovada pelo usuário.
- A IA não deve inventar ou otimizar funcionalidades além do que está documentado na Spec e no código legado.
- Nomes de tabelas e colunas são sagrados: mantenha a nomenclatura idêntica ao legado (ex: EntCliente em vez de pluralizações como Clientes).
- **Obrigatorio:** Antes de gerar a spec, mapear **todas** as propriedades da entidade legada e verificar se cada uma tem correspondência no C# atual (entidade + DTO + mapping EF). Propriedades ignoradas causam retrabalho.

## 3. Arquitetura do Frontend (React/TypeScript)
- Toda listagem e formulário CRUD deve herdar e estender a infraestrutura orientada a objetos (BaseCadastroConfig<T>).
- **Proibido:** Inserir lógica hardcoded do backend, endpoints ou mapeamentos específicos dentro de componentes ou hooks reutilizáveis como useCrudListState.ts.
- **Obrigatorio:** Sobrescrever os métodos mapBackendToForm e mapFormToBackend no arquivo [Nome]CadastroConfig.tsx do formulário quando houver diferenças de enums ou estrutura de dados entre o legado/API e o formulário do React.
- **Obrigatorio:** Organizar as páginas do frontend em subpastas por módulo correspondente ao backend (ex.: src/pages/[Modulo]/F[Nome]/), em vez de manter uma estrutura plana diretamente em src/pages/. Isso espelha a modularização de negócio do sistema.

## 4. Estratégia Git e Proteção de Branches
- **Proibido:** A IA nunca deve fazer commits ou push diretamente nas branches estáveis main ou develop.
- **Obrigatorio:** Todo desenvolvimento deve ser isolado em uma branch de recurso (`feat/migrate-[nome]`).
- Os commits devem ser granulares por fase (Spec, Backend, Frontend, Rota) com mensagens padronizadas.
- **Obrigatorio:** Após o merge bem-sucedido de uma branch de recurso para a branch `develop`, a IA DEVE obrigatoriamente perguntar ao usuário se deseja excluir a branch de recurso (local e remota) e alternar o ambiente de trabalho para `develop`.
- O merge de develop para main é estritamente manual e de responsabilidade exclusiva do usuário.

## 5. Validação com Builds
- Após gerar código no Backend C#, o agente deve rodar obrigatoriamente dotnet build e verificar se há erros de compilação.
- Após gerar código no Frontend, o agente deve rodar 
pm run build para garantir a integridade das tipagens TypeScript e empacotamento Vite.
- **Obrigatorio:** Antes de rodar dotnet build, verificar se o servidor (dotnet run) está ativo com manage_task list e matá-lo com manage_task kill. O servidor em execução bloqueia os arquivos DLL e causa falha de build.

## 6. Padrões de Tela (Classificação Obrigatória)

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

## 7. Compatibilidade com SQL Server 2008
- O banco de dados legado é SQL Server 2008 e **não suporta** as cláusulas OFFSET/FETCH usadas pelo EF Core para paginação nativa.
- **Obrigatorio:** Para qualquer consulta paginada, buscar todos os registros do filtro com ToListAsync() primeiro e depois paginar em memória com .Skip(offset).Take(limit).
- **Proibido:** Usar .Skip().Take() diretamente sobre um IQueryable<T> que ainda não foi materializado para memória.
- Exemplo correto:
  `csharp
  var todos = await query.Where(filtros).ToListAsync();
  var total = todos.Count;
  var pagina = todos.Skip((page - 1) * limit).Take(limit).ToList();
  `
  

## 8. Tratamento de Erros e Validação de Negócio (Result Pattern)
- **Proibido:** A IA não deve lançar exceções (`throw new ...Exception(...)`) para sinalizar falhas de validação de dados ou quebra de regras de negócio esperadas.
- **Obrigatorio:** Toda validação de negócio e persistência nos serviços do domínio deve utilizar o padrão de retorno funcional `Result<T>` ou `ValidationResult` (do namespace `Versatus.Framework.Validation`) encapsulando erros estruturados em `ValidationError`. Os controladores (Controllers) devem verificar a flag `IsSuccess`, retornando `400 BadRequest` com a lista detalhada de erros caso falhe.

## 9. Sinalização Visual de Campos Obrigatórios (Frontend MUI)
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

