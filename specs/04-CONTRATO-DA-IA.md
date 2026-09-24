# CONTRATO DE EXECUÇÃO — IA VERSATUS

> 💡 **DICA PARA O USUÁRIO:** Se a IA parecer "esquecida", use o prompt em [specs/05-ONBOARDING-IA-PROMPT.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/05-ONBOARDING-IA-PROMPT.md).

Este documento define o protocolo obrigatório para qualquer IA que atue no projeto **Versatus.Net8**. A falha em seguir este protocolo resultará em quebra de arquitetura e integridade do sistema.

---

## 1. O Ritual de Início (Obrigatório)

Antes de escrever qualquer linha de código, a IA **DEVE**:
1. Ler o Indice Geral (`specs/00-INDICE-GERAL.md`).
2. Ler as Regras Anti-Alucinação (`specs/03-REGRAS-ANTI-ALUCINACAO.md`).
3. Ler as Boas Práticas e Decisões de Coleções (`specs/decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`).
4. Ler a Decisão de ORM e Pureza de Domínio (`specs/decisoes/DEC-001-ORM.md`).
5. **Ler o Log de Progresso e Handoff (Seção 5 deste documento)** para saber o ponto exato de parada e evitar trabalho redundante.

---

## 2. Leis Fundamentais de Arquitetura

### A. Pureza de Domínio (Lei nº 1)
As classes na pasta `Domain/` (Entidades, Value Objects) devem ser **POCOs puras**.
- **PROIBIDO**: `using System.ComponentModel.DataAnnotations;`
- **PROIBIDO**: Atributos como `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]`.
- **OBRIGATÓRIO**: Todo o mapeamento do banco de dados deve ser feito via **Fluent API** na camada de `Infrastructure`.

### B. SPEC-First (Lei nº 2)
A IA não "projeta" soluções. A IA **traduz** o legado baseado na SPEC do módulo.
- Se a SPEC diz "Tarefa 1.1", implemente APENAS a Tarefa 1.1.
- Não refatore, não limpe, não melhore o legado além do que a SPEC orienta.
- Em caso de ambiguidade: **PARE E PERGUNTE**.

### C. Git-First (Lei nº 3)

> ⛔ **PROIBIDO — IA NUNCA DEVE:**
> - Fazer commit diretamente em `develop` ou `main`
> - Executar `git merge develop`, `git push origin develop` ou `git push origin main`
> - Executar `git rebase` sem permissão explícita do usuário

**Fluxo obrigatório:**

| Branch | Uso | Quem commita |
|---|---|---|
| `main` | Código estável | Apenas via merge de `release/*` aprovado |
| `develop` | Integração | Apenas via merge `--no-ff` aprovado pelo usuário |
| `feat/modulo-descricao` | Nova funcionalidade | IA trabalha aqui |
| `fix/modulo-descricao` | Correção de bug | IA trabalha aqui |
| `docs/descricao` | Atualização de spec/docs | IA trabalha aqui |

```
1. git checkout develop && git pull origin develop
2. git checkout -b feat/nome-descritivo
3. (trabalha, commita incrementalmente na feat/)
4. Deixa a branch aberta — NÃO faz merge
5. Usuário revisa e aprova o PR/MR
6. Merge com --no-ff para develop
```

- **Commits**: Use Conventional Commits — `feat(modulo):`, `fix(modulo):`, `docs(modulo):`, `test(modulo):`.
- **Merge**: Use `--no-ff` para manter o histórico de branches visível.

### D. Desacoplamento Estrito de Controladores (Lei nº 4)
Os controladores (Controllers) devem atuar puramente como adaptadores de entrega HTTP finos.
- **PROIBIDO**: Acessar o `DbContext` ou qualquer `IRepository` de dentro de qualquer classe de controlador.
- **PROIBIDO**: Instanciar ou montar agregados e entidades de domínio diretamente dentro de controladores.
- **PROIBIDO**: Aninhar classes de DTOs ou Commands dentro de classes de controlador. DTOs de request devem ser declarados como `record`s independentes na camada de Domínio/Aplicação.
- **OBRIGATÓRIO**: Depender exclusivamente de interfaces de **Serviços** (`IService`) e repassar os DTOs/records intactos para a camada de negócios.

### E. Padrão de Estrangulamento do Legado (Lei nº 5)
Qualquer código utilitário de rede, integração ou DTO criado no projeto legado para fins de estrangulamento (Strangler Fig Pattern) **DEVE** residir no projeto dedicado **`Servidor.Strangler`** e seguir uma divisão modular estrita de diretórios e namespaces por área de negócio.
- **PROIBIDO**: Criar DTOs ou helpers HTTP misturados nos arquivos originais de entidades legadas (ex: no fim do arquivo ou na mesma pasta de objeto de negócio).
- **OBRIGATÓRIO**: Organizar o projeto `Servidor.Strangler` sob a seguinte hierarquia de diretórios:
  - `Common/` (Para infraestrutura genérica de rede, como o `FinancialStranglerHelper.cs`).
  - `[NomeDoModulo]/DTOs/` (Para os DTOs de cada área, ex: `Gestao.Financeira/DTOs/`, `Gestao.Material/DTOs/`, etc.).
- **OBRIGATÓRIO**: Utilizar namespaces condizentes com a estrutura de diretórios (ex: `Projeto.Servidor.Strangler.GestaoFinanceira.DTOs`).

---

## 3. Padrões de Código (C# 12+)

- **Records**: Use `public sealed record` para DTOs, Commands, Queries e Value Objects.
- **Coleções**:
  - Exposição pública: `IReadOnlyList<T>`.
  - Interna de agregado: `List<T>` privada + `AsReadOnly()`.
  - **PROIBIDO**: Classes `*Lista` (legado).
- **Async/Await**: Todo I/O deve ser `async` e propagar o `CancellationToken`.
- **Fluxo de Erro**:
  - Use `Result<T>` e `ValidationResult` para erros de negócio esperados.
  - Exceções (`VersatusException`) somente para falhas críticas de infraestrutura ou invariantes.
  - **PROIBIDO**: Usar `try-catch` para controlar fluxo de negócio normal.

---

## 4. Como Responder ao Usuário

1. **Confirme a Branch**: Indique em qual branch você está operando.
2. **Cite a Task**: Indique qual seção da SPEC você está implementando.
3. **Prove com Código**: Mostre o arquivo criado e seu local.
4. **Resumo Git**: Informe os comandos de commit realizados (nunca merge sem aprovação).
5. **Update do Log (OBRIGATÓRIO)**: Confirme que você detalhou o progresso na Seção 5 deste documento antes de encerrar.

---

**Cumpra estas regras e seremos parceiros. Ignore-as e você quebrará o Versatus.**

---

## 5. Log de Progresso e Handoff (Atualizado: 2026-09-24)

Este log serve para que a próxima instância da IA saiba exatamente onde o trabalho parou. **Ao finalizar sua sessão, atualize esta tabela.**

| Módulo Atual | Fase / Status | Task Atual | Branch Ativa | Observação Crítica |
| :--- | :--- | :--- | :--- | :--- |
| **MOD-01** | ✅ Concluído | 9.2 (Final) | `develop` | Framework Base e Infra base finalizados. |
| **MOD-02** | ✅ Concluído | 11.0 (Validação)| `develop` | Implementadas validações estritas de CPF/CNPJ (Módulo 11), Obrigatoriedade, Razão Social e bypass de estrangeiros baseados em parâmetros do banco. |
| **MOD-05** | 🔄 Implementação SDD (etapa 6) — Fase S + E0 + E1 concluídos; E3 em andamento (T01–T04 ✅) | Próxima: `E3-T05` (serviços + repositórios E3 + RTV) | `develop` (nenhuma `feat/` aberta) | Fase S, E0 e E1 (`E1-T01`..`E1-T04`) mesclados em `develop` (PRs #3–#19). Gate `/analyze MOD-05 --epico E3` ✅. E3: `E3-T01` auditoria (PR #20), `E3-T02` entidades `Domain/Bancos/` (PR #21), `E3-T03` mappings (PR #22), `E3-T04` DbSets + `MapeamentoE3Tests` (PR #23). Build `Versatus.GestaoFinanceira` 0 erro/0 aviso; `Versatus.GestaoFinanceira.Tests` **138/138**. **Decisões E3:** `ContaBancaria` só com o núcleo (colunas `[E14]` no E14; 6 NOT NULL viram shadow properties `false`); `IndiceConversor` = serviço sem tabela; `FINSALDORATEIO` sem PK física → `HasNoKey` (DÚVIDA-E3-1). Pré-requisitos pendentes: MOD-02 migrar rateio antes de E5 (CLR-01); dump de schema de produção antes de E2/E11. |
| **SDD (Governança)** | 🔄 Fundação criada + comando `/implement` | Fluxo spec-kit + 9 skills + 7 comandos | `develop` | `specs/memory/constitution.md` v1.0 + skills `sdd-constitution/specify/clarify/plan/tasks/analyze` + `legacy-operation-audit` + `legacy-calc-parity` + comandos `/constitution /specify /clarify /plan /tasks /analyze /implement`. `.claude/commands/implement.md` criado (PR #2, `a0da9f6`) — etapa 6 do SDD, despacha skill por `tipo` de tarefa. `MANUAL-SKILLS.md` atualizado (Lei 12). |
| **MOD-08** | 🔄 Em progresso | Segurança (MOD-08) | `feat/seguranca-integracao-strangler` | Implementada autenticação via API Key com bypass de localhost na API e no legado. |
| **MOD-07** | 🔄 Em progresso | 4.3 (Fase 4) | `feat/mod-02-demo` | Concluída Fase 4 (ICMS, Substituição Tributária, Mapeamentos, Regimes e Vigências). Fase 5 (SPED) aguarda análise — ver `specs/prompts-execucao/MOD-07-FASE5-ANALISE-SPED.md`. |
| **MOD-03** | 🔄 Em progresso | Fases 1-3 prontas | `develop` | Prompts de execução criados em `specs/prompts-execucao/`. Iniciar por `MOD-03-FASES1-3-EXECUTION-PROMPT.md`. |
| **MOD-02 (Parametro)** | ✅ Concluído | Migração e Reorganização | `feat/migrate-parametro` | Migrado formulário Parametro (C# e React) e reorganizadas as páginas do frontend por módulo correspondente ao backend. |

### Histórico Recente de Decisões:
- **2026-09-24 (MOD-05 / SDD — Épico E3, T01–T04):** Mescladas em `develop` as quatro primeiras tarefas do **E3 Caixa/Banco** (sessão de 2026-09-24; `E3-T01` foi feita em 2026-09-09 e mesclada hoje): `E3-T01` (PR #20) auditoria RTV/ROT das 7 classes — `matriz-rtv.md#E3`, `matriz-rot.md#E3`, `analysis/E3-caixa-banco.md`; gate `/analyze MOD-05 --epico E3` ✅ (3 MÉDIO corrigidos, incl. nova tarefa `E3-T09` `parity`) · `E3-T02` (PR #21) POCOs `CaixaBanco` (agregado de `CaixaBancoUsuario`), `CaixaBancoUsuario`, `ContaBancaria`, `SaldoCaixaBanco`, `SaldoRateio`, `Cobrador` em `Domain/Bancos/` · `E3-T03` (PR #22) 6 mappings Fluent API em `Infrastructure/Mappings/` (DDL gerado pelo EF conferido contra `fin_columns.txt`; datas com `HasColumnType("datetime")`) · `E3-T04` (PR #23) 6 DbSets no `GestaoFinanceiraDbContext` + `tests/.../E3/MapeamentoE3Tests.cs` (19 testes; total 138/138). **Decisões do usuário:** `ContaBancaria` = **só o núcleo** (12 colunas + auditoria) — as ~40 colunas `[E14]` entram no épico E14, embora o texto original do `E3-T02` pedisse as 55; `IndiceConversor` **não** vira POCO (não há tabela) — vira `ConversorIndiceService` no E3-T05/T09. **Decisões técnicas com base no legado:** as 6 colunas `[E14]` NOT NULL de `FINCONTABANCARIA` (`GERABOLETO`, `GERAREMESSA`, `PROCESSARETORNO`, `BOLETOBENEFICIARIODIFERENTE`, `BOLETOSACADOAVALISTA`, `ENVIARSPED`) viram shadow properties `bool` com default `false` (construtor legado `ContaBancaria.cs:88-94`); **DÚVIDA-E3-1 resolvida:** `FINSALDORATEIO` não tem PRIMARY KEY no banco (só índice UNIQUE com colunas anuláveis) e é gravada por SQL direto no `SaldoRateioHelper` (acesso.global) → mapeada `HasNoKey` (só leitura). `tasks.md` e `analysis/E3-caixa-banco.md` atualizados. **Recuperação de log:** o **Épico E1** foi concluído em 2026-09-08/09 sem entrada neste log — `E1-T01` auditoria das bases (PR #16), `E1-T02` POCOs abstratas `Domain/Bases/` (PR #17), `E1-T03` serviços de comportamento `Application/Bases/` (PR #18), `E1-T04` testes RTV + golden `CALC-E1` (PR #19; 119 testes). Git: um `git push` da branch `feat/mod-05-e3-entidades` foi **bloqueado pelo hook** por estar no mesmo comando que `gh pr create --base develop` (falso positivo; refeito em comandos separados). Nenhum push/merge em `develop`/`main`.
- **2026-09-08 (MOD-05 / SDD — Fase S + Épico E0):** Concluídas e mescladas em `develop` a **Fase S** (`S-T01` `Versatus.SharedKernel` · `S-T02` `Versatus.GestaoFinanceira` + 22 pastas · `S-T03` `GestaoFinanceiraDbContext`/`...ReadDbContext` + `GestaoFinanceiraRepositorioBase<T>` · `S-T04` `tests/Versatus.GestaoFinanceira.Tests` + `AddGestaoFinanceira()`/`AddSharedKernel()` + 2 DbContexts + `AddApplicationPart` no `Program.cs`, WebAPI sobe) e o **Épico E0** (`E0-T01` `enums.md` — inventário + classificação persistido/não-persistido · `E0-T02` 43 enums em `Versatus.SharedKernel/Enums/`, + 6 movidos do AcessoGlobal = 49 · `E0-T03` `IDadosComissao` + `RateioContainer` + `ValidationRateioContainer` · `E0-T04` 62 testes verdes; `AddSharedKernel()` = no-op documentado). Criada a **DEC-007**: `Versatus.SharedKernel` deixa de ser kernel só do MOD-05 e passa a ser o kernel do **ERP** — enum usado por 2+ módulos mora lá, não é duplicado por módulo (amplia CLR-02; nota cruzada no `clarify.md`). Na mesma frente, **corrigido bug do MOD-02**: `EntidadeTipoPessoa` gravava `1/2` numa coluna cuja convenção do legado é `2/3` (`GLOENTIDADE.IDFISICAJURIDICA`), com remendo silencioso no frontend; enum renumerado, remendo removido e **dado do banco corrigido** (`UPDATE GLOENTIDADE SET IDFISICAJURIDICA=2 WHERE IDFISICAJURIDICA=1`, 2 linhas, autorizado pelo usuário; script em `specs/decisoes/dec-007-fix-entidadetipopessoa.sql`). Corrigida a imprecisão CLR-02 vs CLR-05 (só `IDadosComissao` vai ao SharedKernel; `IMovimentoPeriodo`/`IDadosPeriodoFormaPagto`/`IDadosRateioFinanceiro` vão ao épico dono em `Versatus.GestaoFinanceira.Domain`). PRs #2–#14 mesclados. `develop` builda 0 erro; `Versatus.GestaoFinanceira.Tests` 62/62; `Versatus.AcessoGlobal.Tests` 42/43 (a falha `RealDatabaseSchemaTests` — coluna `TemaModo` — **já existia antes**, alheia, confirmado via `git stash`). Pendências conhecidas: essa falha de teste preexistente e o `Lookup<T>` do `plan.md §2`, que nunca virou tarefa E0 (deferido até um consumidor definir sua forma).
- **2026-09-03 (MOD-05 / SDD — implementação):** Criado o slash command `/implement` que faltava em `.claude/commands/` (era referenciado em `04-CONTRATO-DA-IA.md`, `MANUAL-SKILLS.md` e `analyze-report.md`, mas o arquivo nunca existiu — causava `Unknown command`). É a **etapa 6 do fluxo SDD**: implementa 1 tarefa atômica de `tasks.md` (1 branch `feat/` = 1 commit), com pré-condições (constituição, gate incremental do `/analyze`, campo `Depende de:`), despacho de skill por `tipo` de tarefa (`setup`→`create-api-module`, `analysis`→`legacy-*-audit`, `service`/`operation`→`test-driven-development`, `parity`→`legacy-calc-parity`, `frontend`→`spec-generator`+`migrate-crud`, etc.) e critérios de "Pronto quando" (PR #2 → `develop`, `a0da9f6`). Executada a tarefa **`S-T01`**: `src/Versatus.SharedKernel/Versatus.SharedKernel.csproj` (Class Library `net10.0`, `ImplicitUsings`/`Nullable` enable, `ProjectReference` só para `Versatus.Framework`) + adição ao `Versatus.Core.slnx`; `dotnet build` do projeto 0 aviso/0 erro, build da solução OK (4 avisos `NU1903` pré-existentes de `SQLitePCLRaw` nos testes, alheios à mudança). Decisões: **sem pacotes NuGet** no csproj (o "pacotes 10.x" de `tasks.md` vale para quando forem adicionados; E0-T02/T03 são C# puro) e **sem `<FrameworkReference Microsoft.AspNetCore.App>`** (kernel de domínio, compila limpo sem). PR #3 → `develop` (`df9a4ee`). Branches locais apagadas; as remotas `origin/docs/sdd-implement-command`, `origin/feat/mod-05-sharedkernel-project` e `origin/docs/mod-05-sdd` seguem no GitHub (o hook de Git bloqueou `git push --delete`).
- **2026-09-03 (MOD-05 / SDD):** Executadas as etapas 1-5 do fluxo SDD para o MOD-05 Gestão Financeira, sem código (branch `docs/mod-05-sdd`). Criada a **Constituição de Engenharia** (`specs/memory/constitution.md` v1.0 — consolida AGENTS.md + 17 Regras + DEC-001..006 num gate executável, fixa `net10.0`/C# 14). Criadas 8 skills SDD (`sdd-constitution`, `sdd-specify`, `sdd-clarify`, `sdd-plan`, `sdd-tasks`, `sdd-analyze`, `legacy-operation-audit` — Matriz ROT, `legacy-calc-parity` — golden tests) + 6 slash commands + `MANUAL-SKILLS.md` atualizado. Fronteira: SDD = módulo; `spec-generator`/`migrate-crud` = tela CRUD isolada. Para o MOD-05: `specs/modulos/MOD-05/` com `spec.md` v2.3 (inventário de 180 classes, árvore de herança, 14 épicos, RN-05-001..020), `clarify.md` (13 CLR — SharedKernel mínimo, rateio como pré-tarefa do MOD-02, `ContaBancaria` E3/E14, bases legadas → POCO abstrata + serviço, `SelecaoDocumento` como serviço, React só núcleo E3/E4/E6/E9, impressão no frontend), `plan.md` (projeto `Versatus.GestaoFinanceira` + `Versatus.SharedKernel` `net10.0`, 15 passos, 11 operações com ordem de persistência, 10 riscos), `data-model.md` + `legacy-schema/` (inspeção real de `localhost\SQLEXPRESS2008` — PK composta por filial com ordem variável, `numeric(23,8)`, `text`, nulidade de auditoria não-uniforme; achado: banco de dev não tem 100% do schema), `research.md`, `contracts/`, `tasks.md` (~90 tarefas atômicas), `analyze-report.md` (V1/V2/V6/V7 PASS; V3/V4/V5 no gate `Z-T02`). **Nenhuma linha de C#/React.**
- **2026-07-20 (AcessoGlobal):** Migrado o formulário legado `Parametro` e valores `ParametroValor` para a arquitetura .NET Core + React. Implementada a regra de organização de pastas por módulo no frontend (`src/pages/[Modulo]/F[Nome]/`), movendo `FEntidade` e `FParametro` para `AcessoGlobal/` e ajustando importações. Commits adicionados na branch `feat/migrate-parametro`.
- **2026-06-12 (Seguranca):** Implementada a segurança via API Key no helper legado e middleware de validação com loopback bypass no .NET 8, conforme especificado em DEC-006 e MOD-08. Commits adicionados na branch `feat/seguranca-integracao-strangler`.
- **2026-06-12 (GestaoFinanceira):** Concluído o estrangulamento de validações e parâmetros das entidades `Documento` e `DocumentoFinanceiroBase` no projeto legado. Criado o projeto `Servidor.Strangler` sob a estrutura de pastas recomendada. Alterações mescladas na branch `develop`.
- **2026-06-11 (Governança):** Criada pasta `specs/prompts-execucao/` com 6 prompts autocontidos para execução do MOD-03 e análise do MOD-07 SPED. Cada prompt inclui regras Git obrigatórias (branch `feat/` ou `docs/`, nunca commitar em `develop`).
- **2026-06-06 (GestaoTributo):** Conclusão da Fase 4 do MOD-07. Criadas as 9 entidades de domínio em `Domain/ICMS/` (GrupoTributarioICMS, GrupoTributarioInventarioICMS, TributoIcmsSubstituicaoEstoque, DetalheUfTributacao, DetalheCidadeTributacao, RegimeTributarioVigencia, SimplesNacional, SimplesNacionalTributo e PartilhaICMSVigencia). Configurados mapeamentos Fluent API correspondentes preservando chaves estrangeiras, precisions decimais e nomes de tabelas/colunas legadas em maiúsculas. Registrados no `TributoDbContext` e validados via testes de integração passando 100% (9/9).
- **2026-06-06 (GestaoTributo):** Conclusão da Fase 3 do MOD-07. Implementados enums tributários, entidades de domínio (Tributacao, RegraTributo, RegraTributoConfiguracao, RegraTributacaoEspecial, AplicacaoProduto, AplicacaoProdutoTributo, AplicacaoNaturezaOperacao, AplicacaoEspecial e SituacaoTributaria), mapeamentos Fluent API correspondentes no EF Core 8 para base real, atualizado o TributoDbContext e adicionados testes de integração validando a persistência com 100% de sucesso.
- **2026-06-06 (AcessoGlobal):** Refatorado `EntidadeService.cs` para suportar a esteira completa de validação de negócios (Razão Social, CPF/CNPJ matemática e obrigatório, unicidade, bypass de estrangeiros com checagem de país). Adicionados métodos auxiliares para obter país via cidade/filial no `EntidadeRepository`. Adicionados 6 testes unitários no `EntidadeServiceTests.cs` cobrindo estes cenários e passando com 100% de sucesso.
- **2026-06-06 (Governança):** Adicionada a `REGRA 16 — Paridade Estrita de Validações e Parâmetros` no arquivo `specs/03-REGRAS-ANTI-ALUCINACAO.md` para evitar regressões futuras nessas validações críticas de sistema.
- **2026-06-05 (GestaoTributo):** Conclusão das Fases 1 e 2 do MOD-07. Criado o projeto Class Library `Versatus.GestaoTributo`, implementados os modelos de domínio e mappings Fluent API para NCM (TRBCLASSIFICACAOFISCAL), CFOP (TRBCFOP), CEST (TRBCEST) e dependências. Criado o projeto de testes `Versatus.GestaoTributo.Tests` com 3 testes de integração integrados e passando.
- **2026-06-05 (AcessoGlobal):** Configuração e conexão com o banco de dados SQL Server real (`localhost\SQLEXPRESS2008`) utilizando as credenciais fornecidas pelo desenvolvedor. Ajustados os mapeamentos Fluent API em `EntidadeMapping.cs` para refletir os nomes corretos da base real (`IDINDICADORCONTRIBUINTEICMS` e `IDTIPOPLATAFORMA`), e o mock do usuário para ID `1` (administrador existente).
- **2026-06-05 (AcessoGlobal):** Conclusão das Fases 7, 8, 9 e 10 do MOD-02. Adicionadas as entidades `Usuario`, `Perfil`, `Banco`, `FormaPagamento`, `SerieDocumento`, `SerieDocumentoFilial` e `Parametro`, com seus respectivos mapeamentos Fluent API, injeção de dependência e testes de integração de mapeamento.
- **2026-05-05 (Git):** Merge da branch `setup/acesso-global-project` para `develop` após aprovação do usuário.
- **2026-04-28 (Global):** Padronização total de nomenclatura para **Inglês** em todas as pastas físicas e namespaces (`Repositories`, `Context`, `Exceptions`). O `Versatus.Framework` foi totalmente refatorado.

### Próxima Ação Pendente:
- **MOD-05:** Fase S, E0, E1 e `E3-T01`..`E3-T04` concluídos (PRs #3–#23 em `develop`). Próxima: **`/implement MOD-05 E3-T05`** — `CaixaBancoService` (CRUD + usuários em lote, OP-E3-07), `ContaBancariaService` (núcleo), `CobradorService`, repositórios CQRS (`Context` escrita / `ReadContext` leitura, paginação materializada) e **1 `[Fact]` por `VAL-E3`** não-`[E14]` + **VAL-E1-27** (`ValidarCaixaBanco`, fecha no E3). VAL-E3-08/10 dependem do período (E2): mockar a porta "período aberto/fechado". Depois: `E3-T06` controllers → `E3-T07` frontend (Padrão A) → `E3-T08` migration → `E3-T09` paridade `CALC-E3`. Pré-requisitos: MOD-02 migrar `RateioMovto`/`RateioMovtoItem`/`ManutencaoRateio` antes de E5 (CLR-01); dump de schema de produção (`FINPROJECAOFLUXOCAIXA*`, `FINLOGDOMINIOPERIODO`) antes de E2/E11.
- Executar **MOD-03 Fases 1-3** usando o prompt em `specs/prompts-execucao/MOD-03-FASES1-3-EXECUTION-PROMPT.md`.
- Executar **Fase 5 SPED (análise)** usando `specs/prompts-execucao/MOD-07-FASE5-ANALISE-SPED.md`.

> **Para retomar o MOD-05:** SDD 1-5 + Fase S + E0 + E1 + `E3-T01`..`E3-T04` concluídos e em `develop`. A implementação vai pelo comando `/implement MOD-XX <TaskID>` (etapa 6) — próxima tarefa: `/implement MOD-05 E3-T05`. O gate `/analyze MOD-05 --epico E?` roda por épico assim que a tarefa `analysis` (`E?-T01`) gera as matrizes RTV/ROT — E1 e E3 já ✅; `Z-T02` é a consolidação final (V1–V7 + cross-épico) antes de fechar o módulo.
