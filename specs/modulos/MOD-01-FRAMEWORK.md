# SPEC — MOD-01: Framework Base
## Módulo: servidor/framework/servidor.framework

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 0 (pré-requisito de tudo)  
> **Status:** ✅ Concluído - Framework Base e Infraestrutura implementados (2026-04-28)  
> **Prioridade:** MÁXIMA — nenhum outro módulo pode começar sem este

---

## 1. Visão Geral do Módulo

O **Framework Base** é a fundação de toda a aplicação servidora legada.
Ele define as classes abstratas e contratos que todos os objetos de negócio implementam.
Na migração, a maioria das classes deste módulo será **eliminada** (pois existiam apenas
por causa do .NET Remoting), mas seus **conceitos** devem ser preservados em novas formas.

### Dependências externas legadas (a eliminar)
- `Gentle.Framework` — ORM legado → substituir por EF Core
- `System.Runtime.Remoting` — comunicação legada → remover completamente
- `MarshalByRefObject` — serialização Remoting → remover completamente

### Localização no legado
```
servidor/framework/servidor.framework/
```

---

## 2. Inventário de Classes Legadas

| Arquivo | Classe Principal | Tipo | Destino na Migração |
|---|---|---|---|
| `ObjetoNegocio.cs` | `ObjectBase` | Base | ⚠️ Eliminar herança, preservar conceito |
| `AmbienteServidorGlobal.cs` | `AmbienteServidorGlobal` | Singleton | 🔄 Substituir por DI / IContexto |
| `ListaBase.cs` | `ListBase` | Base de coleção | ❌ **ELIMINAR** — usar `IReadOnlyList<T>` e `List<T>` nativos (ver DEC-005) |
| `Transacao.cs` | `Transacao`, `TransactionEx` | Transação | 🔄 Substituir por `IDbContextTransaction` |
| `ClienteServidor.cs` | `ClienteServidor` | Sessão | 🔄 Substituir por JWT Claims + Session |
| `ConfiguracaoServidor.cs` | `ConfiguracaoServidor` | Config | 🔄 Substituir por `IOptions<T>` / appsettings |
| `GeradorSequencial.cs` | `GeradorSequencial` | Gerador de ID | ✅ MANTER — replicar fielmente |
| `Licenca.cs` | `Licenca` | Licenciamento | ✅ Manter — replicar lógica |
| `Liberacao.cs` | `Liberacao` | Permissões | ✅ Manter — replicar lógica |
| `Factory.cs` | `Factory` | Criação de objetos | 🔄 Substituir por DI Container |
| `BizServerManager.cs` | `BizServerManager` | Gerenciador do servidor | 🔄 Adaptar para IHostedService |
| `CanalServidor.cs` | `CanalServidor` | Canal Remoting | ❌ Eliminar completamente |
| `ObjetoBaseSAO.cs` | `ObjetoBaseSAO` | Base SAO | ❌ Eliminar — era wrapper de Remoting |
| `ListaBasePersistencia.cs` | `ListaBasePersistencia` | Lista persistível | 🔄 Substituir por repositório |
| `ObjetoNegocioAtributo.cs` | atributos | Atributos ORM | 🔄 Substituir por atributos do EF Core |
| `ObjetoNegocioCache.cs` | cache | Cache | 🔄 Substituir por `IMemoryCache` |
| `ObjetoNegocioConsulta.cs` | consultas | Consultas | 🔄 Substituir por queries do repositório |
| `GeradorSequencial.cs` | `GeradorSequencial` | Sequencial | ✅ CRÍTICO — não alterar comportamento |
| `SponsorManager.cs` | `SponsorManager` | Lifetime Remoting | ❌ Eliminar |
| `ServidorEstatisticas.cs` | estatísticas | Monitoramento | 🔄 Substituir por Health Checks |

---

## 3. Análise das Classes Críticas

### 3.1 `ObjectBase` — origem de tudo

**Localização legada:** `ObjetoNegocio.cs`  
**Herança legada:** `MarshalByRefObject` → `ObjectBase` → objeto de negócio

**O que faz:**
- Mantém referência ao `IAmbiente` (contexto de execução)
- Gerencia ciclo de vida via `InitializeLifetimeService` (remoting)
- Ponto central de acesso ao ambiente pelo objeto

**O que fazer na migração:**
- **Eliminar** a herança de `MarshalByRefObject`
- **Eliminar** `InitializeLifetimeService`
- **Preservar** o conceito de `IAmbiente` via injeção de dependência
- Classes de domínio NÃO devem herdar de nenhuma classe base de infraestrutura

### 3.2 `IAmbiente` — contexto de execução

**O que contém (lido do código):**
- Referência à `IFilial` corrente
- Referência ao `IUsuario` corrente
- Referência à `ITransacao` corrente (pode ser null)
- Referência à `IEmpresa` corrente
- Método `Assimilar(IAmbiente)` — propaga contexto de pai para filho

**O que fazer na migração:**
```
Criar: IContextoExecucao
Conteúdo:
  - int IdFilial
  - int IdUsuario
  - int IdEmpresa
  - string[] Perfis (roles do usuário)
Fonte: Claims do JWT Token
Registro: Singleton por request (IHttpContextAccessor)
```

### 3.3 `GeradorSequencial` — CRÍTICO

**Localização legada:** `GeradorSequencial.cs`  
**Comportamento:** Gera IDs sequenciais por filial (não usa IDENTITY do SQL Server)

> ⚠️ **ATENÇÃO MÁXIMA:** Este gerador é a espinha dorsal da integridade do banco.
> Alterar seu comportamento pode causar colisão de IDs e corrupção de dados.

**O que fazer na migração:**
- Ler e documentar completamente o algoritmo do `GeradorSequencial.cs` legado
- Criar `GeradorSequencialService` no novo projeto com comportamento **idêntico**
- Adicionar testes unitários que comparam a saída dos dois
- Registrar como `IScopedService` para garantir thread-safety

**Tarefas para a IA (ordem obrigatória):**
1. Ler o `GeradorSequencial.cs` e documentar o algoritmo aqui (seção 3.3.1)
2. Implementar `IGeradorSequencial` interface
3. Implementar `GeradorSequencialService : IGeradorSequencial`
4. Criar testes unitários de paridade

#### 3.3.1 Algoritmo do GeradorSequencial

O gerador legado utiliza as tabelas `GloSequencial` (Pai) e `GloSequencialItem` (Valores por contexto) para evitar o `IDENTITY` nativo e permitir reinício de numeração por Filial/Empresa.

**Fluxo Lógico:**
1. **Identificação:** Busca o `IdGloSequencial` na tabela `GloSequencial` baseado no nome da classe (`SequencialNome()`) e no tipo (`SequencialTipo`).
2. **Atualização (Update-first):**
   - Executa `UPDATE GloSequencialItem SET Numero = Numero + 1 WHERE IdGloSequencial = @id AND ...filtros contextuais`.
   - Filtros contextuais: Se `Tipo == Filial`, filtra por `IdGloFilial`. Se `Tipo == Empresa`, filtra por `IdGloEmpresa`. Se `Geral`, não filtra contexto.
3. **Leitura Pós-Update:** Se o `UPDATE` afetou 1 linha, faz um `SELECT Numero` para obter o novo valor incrementado.
4. **Criação (Insert-fallback):** Se o `UPDATE` afetou 0 linhas (sequencial novo para aquele contexto):
   - Faz um `INSERT` em `GloSequencialItem` com valor inicial `1` (ou `2` se a entidade for o próprio 'Sequencial').
   - Retorna `1`.
5. **Transacionalidade:** Se não houver uma transação externa ativa, abre uma transação local com nível `ReadCommitted`.

**Crítico para Migração:**
- Manter a distinção entre `IdGloSequencial` (GUID/ID da regra) e `IdGloSequencialItem` (Valor atual).
- Respeitar a ordem: Update → Select (Se rows > 0) OR Insert (Se rows == 0).

### 3.4 `Transacao` — gerenciamento de transações

> Ver também: `decisoes/DEC-003-TRANSACAO.md`

**Localização legada:** `Transacao.cs`  
**Herança:** `TransacaoBase` → `Transacao`  
**Dependência:** `Gentle.Framework.Transaction` (a eliminar)

**O que fazer na migração:**
- Remover dependência do Gentle.NET
- Usar `IDbContextTransaction` do EF Core
- O padrão de uso deve permanecer via `using` (disposable)

**Padrão alvo:**
```csharp
// Novo padrão — dentro do Handler/UseCase
await using var transacao = await dbContext.Database.BeginTransactionAsync();
try {
    // operações
    await transacao.CommitAsync();
} catch {
    await transacao.RollbackAsync();
    throw;
}
```

---

## 4. Estrutura do Projeto Novo (Versatus.Framework)

```
Versatus.Framework/
├── Contexto/
│   ├── IContextoExecucao.cs        ← Substitui IAmbiente
│   ├── ContextoExecucao.cs
│   └── ContextoExecucaoExtensions.cs
├── Sequencial/
│   ├── IGeradorSequencial.cs       ← Interface do gerador de IDs
│   └── GeradorSequencialService.cs ← Implementação fiel ao legado
├── Excecoes/
│   ├── VersatusException.cs        ← Exceção base (herança obrigatória)
│   ├── RegraDeNegocioException.cs  ← Violação de regra de negócio
│   └── EntidadeNaoEncontradaException.cs
├── Validation/
│   ├── ValidationError.cs          ← Erro de validação esperado
│   ├── ValidationResult.cs         ← Resultado de validação estruturado
│   └── Result.cs                   ← Padrão genérico Result<T> para handlers
├── Paginacao/
│   └── PagedResult.cs              ← record PagedResult<T> para queries paginadas
├── Repositorio/
│   ├── IRepositorio.cs             ← Interface genérica base
│   └── RepositorioBase.cs          ← Implementação base com EF Core
└── Configuracao/
    └── VersatusOptions.cs          ← Substitui ConfiguracaoServidor
```

> ⚠️ **Não existe** `ListaBase.cs` nem nenhuma classe equivalente neste projeto.
> Coleções são sempre tipos nativos do .NET (`List<T>`, `IReadOnlyList<T>`).

---

## 5. Dependências do Este Módulo

| Depende de | Tipo |
|---|---|
| Entity Framework Core 8 | NuGet |
| Microsoft.Extensions.DependencyInjection | NuGet |
| Microsoft.Extensions.Caching.Memory | NuGet |
| Microsoft.Extensions.Options | NuGet |

---

## 6. Regras de Negócio Críticas

1. O `GeradorSequencial` deve produzir IDs idênticos ao legado para o mesmo estado inicial
2. O `IContextoExecucao` deve ser preenchido a partir de claims do JWT (não de parâmetros)
3. Toda exceção de negócio deve herdar de `VersatusException` — nunca lançar `Exception` genérica
4. Não usar exceções como controle de fluxo em validações esperadas; preferir `Result<T>`/`ValidationResult` ou padrão Notification para erros de entrada e regras de negócio comuns
5. Transações devem ser sempre assíncronas no novo sistema
6. **Proibido** criar qualquer classe `*Lista` — usar `IReadOnlyList<T>` e `List<T>` nativos
7. Todo método de I/O deve ser `async` com `CancellationToken`
8. `PagedResult<T>` deve ser um `record` imutável com `IReadOnlyList<T> Items` e `int Total`

---

## 8. Roteiro de Tarefas (Ordem de Execução)

Siga esta sequência de tarefas para implementar MOD-01. Cada tarefa deve resultar em um commit Git, um PR para revisão, e marca de concluído na SPEC.

### Fase 1: Setup e Infraestrutura Básica

**Tarefa 1.1 — Criar projeto Versatus.Framework**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.Framework`
- Adicione os pacotes NuGet listados na seção 5
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/framework-project` [x]
- Commit: `setup: Create Versatus.Framework project with EF Core 8` [x]

**Tarefa 1.2 — Criar estrutura de pastas e arquivos base**
- Crie as pastas conforme seção 4 do arquivo: `Contexto/`, `Sequencial/`, `Excecoes/`, `Validation/`, `Paginacao/`, `Repositorio/`, `Configuracao/`
- Crie arquivos vazios para cada classe listada na seção 4
- Branch: `setup/framework-structure` [x]
- Commit: `setup: Create folder structure for Versatus.Framework` [x]

### Fase 2: Exceções Base (Prioridade Alta)

**Tarefa 2.1 — Implementar VersatusException**
- Crie `Excecoes/VersatusException.cs` (classe base para todas as exceções do sistema)
- Deve herdar de `Exception`
- Adicione construtores para mensagem simples, com parâmetros formatados
- Adicione propriedades para rastreamento (ex: `ErrorCode`, `Timestamp`)
- Branch: `feat/versatus-exception` [x]
- Commit: `feat: Implement VersatusException base class` [x]
- Marque no checklist: ✅ `VersatusException` criada

**Tarefa 2.2 — Implementar RegraDeNegocioException**
- Crie `Excecoes/RegraDeNegocioException.cs`
- Deve herdar de `VersatusException`
- Usar para violações de regras de negócio esperadas
- Branch: `feat/regra-negocio-exception` [x]
- Commit: `feat: Implement RegraDeNegocioException` [x]
- Marque no checklist: ✅ `RegraDeNegocioException` criada

**Tarefa 2.3 — Implementar EntidadeNaoEncontradaException**
- Crie `Excecoes/EntidadeNaoEncontradaException.cs`
- Deve herdar de `VersatusException`
- Use para quando uma entidade do banco não existe
- Construtor recebe `Type` da entidade e chave (id)
- Branch: `feat/entidade-nao-encontrada-exception` [x]
- Commit: `feat: Implement EntidadeNaoEncontradaException` [x]
- Marque no checklist: ✅ `EntidadeNaoEncontradaException` criada

### Fase 3: Validação e Resultados

**Tarefa 3.1 — Implementar ValidationError**
- Crie `Validation/ValidationError.cs`
- `record ValidationError(string Campo, string Mensagem)`
- Branch: `feat/validation-error` [x]
- Commit: `feat: Implement ValidationError record` [x]
- Marque no checklist: ✅ `ValidationError` criado

**Tarefa 3.2 — Implementar ValidationResult**
- Crie `Validation/ValidationResult.cs`
- `record ValidationResult(bool IsValid, IReadOnlyList<ValidationError> Errors)`
- Inclua métodos estáticos `Ok()` e `Fail(params ValidationError[])`
- Branch: `feat/validation-result` [x]
- Commit: `feat: Implement ValidationResult record` [x]
- Marque no checklist: ✅ `ValidationResult` criada

**Tarefa 3.3 — Implementar Result<T> genérico**
- Crie `Validation/Result.cs`
- `sealed record Result<T>(bool IsSuccess, T? Value, IReadOnlyList<ValidationError> Errors)`
- Inclua métodos estáticos `Ok(T value)` e `Fail(params ValidationError[])`
- Branch: `feat/result-generic` [x]
- Commit: `feat: Implement Result<T> generic record` [x]
- Marque no checklist: ✅ `Result<T>` criada

### Fase 4: Paginação

**Tarefa 4.1 — Implementar PagedResult<T>**
- Crie `Paginacao/PagedResult.cs`
- `record PagedResult<T>(IReadOnlyList<T> Items, int Total)`
- `Items` é a lista de itens paginada (imutável)
- `Total` é o total de registros disponíveis (sem filtro de paginação)
- Branch: `feat/paged-result` [x]
- Commit: `feat: Implement PagedResult<T> record` [x]
- Marque no checklist: ✅ `PagedResult<T>` criada

### Fase 5: Contexto de Execução

**Tarefa 5.1 — Implementar IContextoExecucao**
- Crie `Contexto/IContextoExecucao.cs`
- Propriedades: `int IdFilial`, `int IdUsuario`, `int IdEmpresa`, `string[] Perfis`
- Branch: `feat/contexto-execucao-interface` [x]
- Commit: `feat: Implement IContextoExecucao interface` [x]

**Tarefa 5.2 — Implementar ContextoExecucao**
- Crie `Contexto/ContextoExecucao.cs` (class simples, implementa IContextoExecucao)
- Can ser registrado como `Scoped` no DI
- Branch: `feat/contexto-execucao` [x]
- Commit: `feat: Implement ContextoExecucao class` [x]
- Marque no checklist: ✅ `IContextoExecucao` implementado

**Tarefa 5.3 — Implementar ContextoExecucaoExtensions**
- Crie `Contexto/ContextoExecucaoExtensions.cs`
- Método para inicializar contexto a partir de `ClaimsPrincipal` do JWT
- Branch: `feat/contexto-execucao-extensions`
- Branch: `feat/contexto-execucao-extensions` [x]
- Commit: `feat: Implement ContextoExecucaoExtensions` [x]
- Marque no checklist: ✅ `ContextoExecucaoExtensions` implementado

### Fase 6: Gerador Sequencial (CRÍTICO — Prioridade Máxima)

**Tarefa 6.1 — Analisar e Documentar GeradorSequencial**
- ⚠️ **ANTES de qualquer implementação**, leia o arquivo `servidor/framework/servidor.framework/GeradorSequencial.cs` do legado
- Documente o algoritmo completo na seção 3.3.1 desta SPEC
- Lista propriedades, métodos, lógica de geração, sincronização de threads
- Branch: `analysis/gerador-sequencial`
- Commit: `docs: Analyze and document GeradorSequencial algorithm`
- Marque no checklist: ✅ `GeradorSequencial` analisado

**Tarefa 6.2 — Implementar IGeradorSequencial**
- Crie `Sequencial/IGeradorSequencial.cs`
- Defina métodos que o serviço deve implementar (baseado na análise 6.1)
- Branch: `feat/gerador-sequencial-interface` [x]
- Commit: `feat: Implement IGeradorSequencial interface` [x]

**Tarefa 6.3 — Implementar GeradorSequencialService**
- Crie `Sequencial/GeradorSequencialService.cs`
- Implemente behavier **idêntico** ao `GeradorSequencial.cs` legado
- Use banco de dados para sincronização de threads (não em-memória)
- Branch: `feat/gerador-sequencial-service` [x]
- Commit: `feat: Implement GeradorSequencialService with legacy parity` [x]
- Marque no checklist: ✅ `GeradorSequencialService` implementado

**Tarefa 6.4 — Testes de Paridade GeradorSequencial**
- Crie testes unitários comparando saída do novo vs. legado
- Teste com múltiplas filiais
- Teste com múltiplas threads
- Branch: `test/gerador-sequencial-parity`
- Commit: `test: Add parity tests for GeradorSequencialService`
- Marque no checklist: ✅ Testes de paridade feitos

### Fase 7: Repositório Base

**Tarefa 7.1 — Implementar IRepositorio<T>**
- Crie `Repositorio/IRepositorio.cs`
- Defina contrato para operações CRUD assíncronas com `CancellationToken`
- Métodos: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `SaveChangesAsync`
- Branch: `feat/repositorio-interface` [x]
- Commit: `feat: Implement IRepositorio<T> generic interface` [x]

**Tarefa 7.2 — Implementar RepositorioBase<T>**
- Crie `Repositorio/RepositorioBase.cs`
- Implemente `IRepositorio<T>` com EF Core
- Use `DbContext` injetado
- Todos os métodos devem ser `async` com `CancellationToken`
- Branch: `feat/repositorio-base` [x]
- Commit: `feat: Implement RepositorioBase<T> with EF Core` [x]
- Marque no checklist: ✅ `IRepositorio<T>` e `RepositorioBase<T>` criados

### Fase 8: Configurações

**Tarefa 8.1 — Implementar VersatusOptions**
- Crie `Configuracao/VersatusOptions.cs`
- Propriedades para configurações do servidor (ex: `TimeoutTransacao`, `MaximoTentativas`)
- Use padrão `IOptions<VersatusOptions>` para injeção
- Branch: `feat/versatus-options` [x]
- Commit: `feat: Implement VersatusOptions configuration` [x]
- Marque no checklist: ✅ `VersatusOptions` criada

### Fase 9: Verificação Final

**Tarefa 9.1 — Confirmar ausência de Classes Legadas**
- Valide que não há `MarshalByRefObject`, `ListBase`, `ArrayList`, `ObjectBase` herdado
- Rode AntiPattern Analyzer se disponível
- Branch: `verify/legacy-patterns-absent`
- Commit: `verify: Confirm legacy patterns are absent`
- Marque no checklist: ✅ `MarshalByRefObject` e `System.Runtime.Remoting` ausentes

**Tarefa 9.2 — Configurar Roslyn Analyzers**
- Adicione analyzers para reforçar as regras (ex: evitar `Exception` genérica)
- Branch: `setup/roslyn-analyzers`
- Commit: `setup: Configure Roslyn analyzers for rule enforcement`
- Marque no checklist: ✅ Análise estática configurada

### Resumo de Commits Esperados

```
1. setup: Create Versatus.Framework project with EF Core 8
2. setup: Create folder structure for Versatus.Framework
3. feat: Implement VersatusException base class
4. feat: Implement RegraDeNegocioException
5. feat: Implement EntidadeNaoEncontradaException
6. feat: Implement ValidationError record
7. feat: Implement ValidationResult record
8. feat: Implement Result<T> generic record
9. feat: Implement PagedResult<T> record
10. feat: Implement IContextoExecucao interface
11. feat: Implement ContextoExecucao class
12. feat: Implement ContextoExecucaoExtensions
13. docs: Analyze and document GeradorSequencial algorithm
14. feat: Implement IGeradorSequencial interface
15. feat: Implement GeradorSequencialService with legacy parity
16. test: Add parity tests for GeradorSequencialService
17. feat: Implement IRepositorio<T> generic interface
18. feat: Implement RepositorioBase<T> with EF Core
19. feat: Implement VersatusOptions configuration
20. verify: Confirm legacy patterns are absent
21. setup: Configure Roslyn analyzers for rule enforcement
```

---

## 9. Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise do diretório `servidor/framework/servidor.framework/` do `projeto_tag_1906`*
