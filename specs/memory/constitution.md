# Constituição de Engenharia — Versatus.Net8

> **Versão:** 1.0 | **Criada:** 2026-09-03 | **Ratifica:** `.agents/AGENTS.md`,
> `specs/03-REGRAS-ANTI-ALUCINACAO.md`, `specs/decisoes/DEC-001..DEC-006`
>
> Este documento é a **fonte única de verificação** ("constitution gate") do fluxo SDD.
> Ele **não cria regra nova** — consolida as regras já ratificadas em um checklist
> executável. Em caso de divergência textual, os documentos ratificados acima têm
> precedência e este arquivo deve ser corrigido.
>
> Toda skill `sdd-*` **DEVE** rodar o *Gate de Conformidade* (Seção 9) contra o artefato
> que produz. `/analyze` reprova o módulo se qualquer item do gate falhar.

---

## 0. Plataforma (imutável nesta versão)

| Item | Valor | Origem |
| :--- | :--- | :--- |
| Runtime | **`net10.0`** em todos os `.csproj` (`src/` e `tests/`) | Estado real do repo (2026-09-03) |
| Linguagem | **C# 14** (`LangVersion` default do SDK) | Decorre do runtime |
| ORM | **EF Core 10.x** (Fluent API), pacotes alinhados ao que `Versatus.GestaoTributo` usa | DEC-001 |
| Banco alvo | **SQL Server 2008** (legado) — sem `OFFSET/FETCH`, sem `Skip().Take()` sobre `IQueryable` não materializado | Lei 8 |
| Testes | `xunit` + `Microsoft.EntityFrameworkCore.InMemory` para mapeamento; SQL real para paridade | Convenção `*.Tests` |
| Nome de projeto de módulo | `Versatus.<NomeDoModulo>` (ex.: `Versatus.GestaoFinanceira`) | Convenção `Versatus.GestaoTributo` |

> Qualquer mudança nesta seção exige nova versão da constituição e menção explícita no Log
> de Progresso (`specs/04-CONTRATO-DA-IA.md` §5).

---

## 1. Artigo I — SPEC-First (Lei 2 e 3 · Regras 7, 7.1, 7.2, 8)

1. Nenhum código é gerado ou alterado sem SPEC aprovada pelo usuário
   (`specs/modulos/MOD-XX/spec.md` para módulo, `docs/spec_f[nome].md` para tela isolada).
2. A IA **traduz** o legado; não projeta, não melhora, não "aplica SOLID" por conta
   própria sobre regra de negócio.
3. Se a SPEC não cobre algo: **PARE e escreva `DÚVIDA:`**. Não invente, não assuma.
4. Mudança de arquitetura/escopo identificada pela IA é **sugerida**, nunca implementada,
   até a SPEC ser atualizada pelo usuário e commitada **antes** do código.
5. Um pedido = um item. Nunca "migre tudo isso".

## 2. Artigo II — Fidelidade ao Legado (Regras 1–6, 9, 16)

1. **Nomes de tabela e coluna são sagrados.** Preservados idênticos ao legado via
   `.ToTable(...)` / `.HasColumnName(...)`. Dúvida de nome → `DÚVIDA:`, nunca chute.
2. **Nenhum campo adicionado** sem SPEC (proibido `CreatedAt`, `IsDeleted`, etc.).
3. **Nenhum campo removido** — mesmo que pareça obsoleto. Marcar `// LEGADO: verificar uso`.
4. **Nenhuma regra de negócio refatorada.** Lógica de 200 linhas migra com 200 linhas de
   comportamento equivalente. Exceção única: API removida do .NET (ex.: `Thread.Abort`) —
   adaptar o mínimo e documentar na SPEC.
5. **IDs por `GeradorSequencialService`** — nunca `IDENTITY`, nunca `GUID`. Todo mapping de
   PK usa `.ValueGeneratedNever()`.
6. **Rastreabilidade de origem** em todo arquivo gerado:
   ```csharp
   // Origem: servidor/objeto de negócio/gestao.financeira/Documento.cs (legado)
   // Tabela: FinDocumento
   ```
7. **Paridade estrita de validações e parâmetros** (Regra 16): toda regra que depende de
   `GloParametro` (ex.: `CpfCnpjObrigatorio`, `AceitaCnpjCpfInvalido`) ou de país/localização
   é crítica. Proibido ignorar o parâmetro ou bypassar unicidade/formato sem replicar o
   comportamento condicional herdado. Dígitos verificadores e limites com rigor matemático.

## 3. Artigo III — Pureza de Domínio (Lei 2 · DEC-001)

1. `Domain/` contém **POCOs puras**. Proibido `using System.ComponentModel.DataAnnotations`
   e atributos `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]`, `[Required]`.
2. Todo mapeamento é **Fluent API** em `Infrastructure/Mappings/`, um arquivo por entidade
   (`<Entidade>Mapping.cs : IEntityTypeConfiguration<Entidade>`).
3. Domínio não conhece `DbContext`, `IDbTransaction`, HTTP nem `IAmbiente`.
4. `string` → `= string.Empty`; navegação → anulável (`Cidade?`); auditoria
   (`IdUsuarioInclusao`, `DataInclusao`, `HoraInclusao`, …) → anulável (`int?`, `DateTime?`).
5. `bool` no domínio é `bool` simples (`= true`/`= false`); a conversão `bool → short`
   (smallint legado) é feita **uma vez** em `ConfigureConventions` do DbContext.

## 4. Artigo IV — Clean Architecture e Controllers Finos (Lei 1, 4 · Regra 17)

1. Camadas: `Domain` (POCO, DTOs `record`, `IService`, `IRepository`) · `Infrastructure`
   (Mappings, DbContext, repositórios) · `Api` (Controllers finos) · `Frontend` (React OOP).
2. Controllers são **adaptadores HTTP**: proibido injetar/acessar `DbContext` ou
   `IRepository`; proibido montar agregados de domínio; proibido DTO/Command aninhado na
   classe do controller. Dependem só de `IService` e repassam `record`s intactos.
3. DI por construtor; dependência sobre abstração.

## 5. Artigo V — Coleções e C# Moderno (DEC-005 · Regras 11, 12, 13)

1. **Proibido criar classes `*Lista`.** Sem equivalente no novo sistema.
   - Exposição pública imutável: `IReadOnlyList<T>`.
   - Coleção interna de agregado: `List<T>` privada + `IReadOnlyList<T>` público + método
     controlado de mutação.
   - Parâmetro que só itera: `IEnumerable<T>`.
   - Query paginada: `record PagedResult<T>(IReadOnlyList<T> Itens, int Total)`.
   - Proibido `IList` não-genérico, `ArrayList`, `ListBase`.
2. DTOs, Commands, Queries e Value Objects são `sealed record`.
3. Todo I/O é `async` e propaga `CancellationToken`. Proibido `.Result` / `.Wait()`.
4. Guard clauses no início (`ArgumentNullException.ThrowIfNull(x)`).
5. Enums com **valores inteiros preservados do banco**; nunca constantes soltas.
6. `ILogger<T>` com log estruturado — sem concatenação de string.
7. Expressões modernas (collection expressions, primary constructors, switch expressions).
8. `Nullable` habilitado (`<Nullable>enable</Nullable>`) e nulos tratados explicitamente.

## 6. Artigo VI — Fluxo de Erro (Lei 9 · Regra 13)

1. **Proibido `throw` para validação de negócio esperada** ou registro inexistente.
2. Serviços de domínio retornam `Result<T>` / `ValidationResult`
   (`Versatus.Framework.Validation`), com erros em `ValidationError`.
3. Controller checa `IsSuccess` → `400 BadRequest` com a lista de erros; sucesso →
   `200 OK` / `201 Created`.
4. Exceções só para falha real de infraestrutura ou invariante violado, e sempre da
   hierarquia `VersatusException` — nunca `Exception` genérica.

## 7. Artigo VII — Transações e CQRS (DEC-003 · Lei 11)

1. **Uma transação por Handler/UseCase**, portada pelo `DbContext`
   (`BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`). Nunca compartilhar
   transação entre Handlers.
2. **Só o Handler chama `SaveChangesAsync`.** Entidades e serviços de domínio nunca.
3. Operação cross-módulo: DbContexts participantes compartilham a **mesma conexão**
   (`UseTransaction`), ou `TransactionScope`. Proibido MSDTC / transação distribuída.
4. **CQRS leve:** mutações (`POST/PUT/DELETE`) na `WriteConnection` (`<Modulo>DbContext`);
   consultas/paginações/lookups (`GET`) na `ReadConnection` (`<Modulo>ReadDbContext`,
   `NoTracking`). Repositórios base usam `Context`/`DbSet` para escrita e
   `ReadContext`/`ReadDbSet` para leitura.
5. **Paginação SQL Server 2008:** `await query.Where(...).ToListAsync()` **primeiro**, depois
   `.Skip(offset).Take(limit)` em memória. Nunca `Skip().Take()` sobre `IQueryable`.

## 8. Artigo VIII — Cross-Module e Estrangulamento (DEC-002 · Lei 5)

1. Referência a entidade de outro módulo é **`int` lógico** (ex.: `IdEntidade`,
   `IdFilial`). **Proibido** `HasOne`/`Include`/navegação EF cross-projeto.
2. `MarshalByRefObject`, `.NET Remoting`, `IAmbiente.Assimilar`, `Transacao` viajante,
   `ObjectBase`/`ListBase` — **removidos**, sem equivalente.
3. Utilitário de rede/DTO criado no legado para estrangulamento reside em
   `Servidor.Strangler/<Modulo>/DTOs/` com namespace espelhando a pasta. Proibido misturar
   DTO de estrangulamento no arquivo da entidade legada.
4. Ao migrar um módulo já estrangulado, o `plan.md` **reconcilia** os DTOs de
   `Servidor.Strangler.<Modulo>` com o domínio novo e aponta divergências de contrato.

## 9. Artigo IX — Testes como Trava (Lei 6, 13 · Regra 10)

1. **Backend:** nenhum `Domain/Services/<Nome>Service.cs` é entregue sem
   `tests/<Modulo>.Tests/<Nome>ServiceTests.cs` cobrindo **100% da Matriz RTV** (1 `[Fact]`
   por linha `VAL-xx`).
2. **Operações transacionais:** 1 teste de integração + 1 golden test de paridade por linha
   `OP-xx` da **Matriz ROT**.
3. **Cálculos financeiros/fiscais:** golden tests com valores capturados do legado/banco
   real (skill `legacy-calc-parity`).
4. **Frontend:** todo formulário React tem `src/pages/<Modulo>/F<Nome>/schema.test.ts`
   (Vitest) cobrindo cada regra de validação.
5. Antes de considerar qualquer entrega concluída: `dotnet build` **0 erro / 0 aviso**,
   `dotnet test` e (se houver frontend) `npm run build` + `npm test` **100% verde**.
   Matar `dotnet run` ativo antes do build (lock de DLL).
6. **Paridade antes de desligar o legado** (Regra 10): cenários críticos comparados
   new-vs-legado em 100% antes de qualquer desligamento.

## 10. Artigo X — Git e Handoff (Lei 5 · Regras 7.3, 14, 15)

1. **Nunca** commit/push/merge/rebase em `develop` ou `main` (hook
   `.claude/hooks/block-protected-git.js` bloqueia de fato).
2. Uma tarefa = uma branch (`feat/`, `fix/`, `docs/`) = commits atômicos por fase.
   Conventional Commits: `feat(modulo):`, `fix(modulo):`, `docs(modulo):`, `test(modulo):`.
3. Ao concluir uma branch, **perguntar** ao usuário antes de sugerir merge; nunca mesclar
   sozinho. Após merge aprovado, perguntar sobre exclusão da branch.
4. Toda sessão relevante encerra atualizando o **Log de Progresso**
   (`specs/04-CONTRATO-DA-IA.md` §5) — via `/handoff`.
5. SPEC atualizada é commitada **antes** do código que a segue.

## 11. Artigo XI — Manutenção de Skills (Lei 12)

1. Fonte única de cada skill: `.agents/skills/<nome>/SKILL.md` (+ `references/`).
2. Toda criação/alteração de frontmatter/exclusão espelha em `.claude/skills/<nome>/SKILL.md`
   (stub curto, mesmo frontmatter, corpo apenas manda ler a fonte).
3. Toda mudança de skill reflete em `docs/MANUAL-SKILLS.md` (índice + propósito + quando
   usar + exemplo).

---

## 12. Gate de Conformidade (checklist executável do `/analyze`)

Marque cada item **PASS/FAIL** contra o artefato. Qualquer `FAIL` reprova.

### 12.1 Plataforma
- [ ] Todo `.csproj` novo declara `net10.0`.
- [ ] Versões de pacote alinhadas a `Versatus.GestaoTributo` (EF Core 10.x).
- [ ] Projeto de módulo nomeado `Versatus.<Modulo>`.

### 12.2 SPEC-First e Fidelidade
- [ ] Existe SPEC aprovada cobrindo 100% do escopo do artefato.
- [ ] Toda tabela/coluna citada bate com o legado (ou está marcada `DÚVIDA:`).
- [ ] Nenhum campo adicionado/removido sem menção explícita na SPEC.
- [ ] Nenhuma regra de negócio "simplificada" sem autorização na SPEC.
- [ ] PK com `.ValueGeneratedNever()`; nenhum `IDENTITY`/`GUID`.
- [ ] Comentário de origem (`// Origem:` + `// Tabela:`) em cada classe gerada.
- [ ] Regras dependentes de `GloParametro`/país replicadas condicionalmente (Regra 16).

### 12.3 Arquitetura
- [ ] `Domain/` sem DataAnnotations; mapeamento 100% Fluent API (1 arquivo/entidade).
- [ ] Controllers sem `DbContext`/`IRepository`; sem DTO aninhado; dependem só de `IService`.
- [ ] Nenhuma classe `*Lista`; coleções conforme Artigo V.
- [ ] DTOs/Commands/Queries/VOs são `sealed record`.
- [ ] I/O `async` com `CancellationToken`; sem `.Result`/`.Wait()`.
- [ ] Enums com valores inteiros do banco preservados.
- [ ] Validação esperada via `Result<T>`/`ValidationResult`; `throw` só infra/invariante
      (`VersatusException`).

### 12.4 Transação / CQRS / Cross-module
- [ ] Uma transação por Handler; `SaveChangesAsync` só no Handler.
- [ ] `GET` na `ReadConnection` (`NoTracking`); mutação na `WriteConnection`.
- [ ] Paginação materializa com `ToListAsync()` antes de `Skip/Take`.
- [ ] Referência cross-módulo é `int` lógico, sem navegação EF.
- [ ] Se módulo estrangulado: DTOs de `Servidor.Strangler` reconciliados no `plan.md`.

### 12.5 Testes
- [ ] Matriz RTV: 1 `[Fact]` planejado por linha `VAL-xx`.
- [ ] Matriz ROT: 1 integração + 1 golden por linha `OP-xx`.
- [ ] Cálculos com golden tests de paridade (valores do legado).
- [ ] Frontend (se houver): `schema.test.ts` por formulário.
- [ ] Plano de build/test verde documentado (`dotnet build/test`, `npm build/test`).

### 12.6 Governança
- [ ] Trabalho em branch `feat/`/`fix/`/`docs/`; nada em `develop`/`main`.
- [ ] Skill nova/alterada: fonte + stub + `MANUAL-SKILLS.md` sincronizados.
- [ ] Log de Progresso (§5 do Contrato) previsto para atualização no fim da sessão.

---

## 13. Glossário rápido (legado → novo)

| Legado | Novo |
| :--- | :--- |
| `ObjectBase` / `ObjectGenerator` / `MarshalByRefObject` | Removidos (eram Remoting) |
| `[Entidade]Lista` / `ListBase` / `ArrayList` | `IReadOnlyList<T>` / `List<T>` |
| `[TableName]` (Gentle) | `.ToTable(...)` (Fluent API) |
| `AutoSequencial` | `GeradorSequencialService` + `.ValueGeneratedNever()` |
| `Transacao` viajante / `amb.ComTransacao` | `DbContext` porta a transação; DI |
| `IAmbiente` (usuário/filial/transação) | `IContexto` injetado |
| `Situacao` (opera estado do documento) | Handler / UseCase |
| `Lookup` (busca cacheada) | `IMemoryCache` / repositório com cache |
| `Gentle.Framework` | EF Core |
| `DataPatch` | Migration EF Core |

---

*Ratificada com base no estado do repositório em 2026-09-03. Corrigir esta versão sempre
que AGENTS.md, 03-REGRAS-ANTI-ALUCINACAO.md ou DEC-001..006 mudarem.*
