# CONTRATO DE EXECUÃ‡ÃƒO â€” IA VERSATUS

> ðŸ’¡ **DICA PARA O USUÃ�RIO:** Se a IA parecer "esquecida", use o prompt em [specs/05-ONBOARDING-IA-PROMPT.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/05-ONBOARDING-IA-PROMPT.md).

Este documento define o protocolo obrigatÃ³rio para qualquer IA que atue no projeto **Versatus.Net8**. A falha em seguir este protocolo resultarÃ¡ em quebra de arquitetura e integridade do sistema.

---

## 1. O Ritual de InÃ­cio (ObrigatÃ³rio)

Antes de escrever qualquer linha de cÃ³digo, a IA **DEVE**:
1. Ler o Indice Geral (`specs/00-INDICE-GERAL.md`).
2. Ler as Regras Anti-AlucinaÃ§Ã£o (`specs/03-REGRAS-ANTI-ALUCINACAO.md`).
3. Ler as Boas PrÃ¡ticas e DecisÃµes de ColeÃ§Ãµes (`specs/decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`).
4. Ler a DecisÃ£o de ORM e Pureza de DomÃ­nio (`specs/decisoes/DEC-001-ORM.md`).
5. **Ler o Log de Progresso e Handoff (SeÃ§Ã£o 5 deste documento)** para saber o ponto exato de parada e evitar trabalho redundante.

---

## 2. Leis Fundamentais de Arquitetura

### A. Pureza de DomÃ­nio (Lei nÂº 1)
As classes na pasta `Domain/` (Entidades, Value Objects) devem ser **POCOs puras**.
- **PROIBIDO**: `using System.ComponentModel.DataAnnotations;`
- **PROIBIDO**: Atributos como `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]`.
- **OBRIGATÃ“RIO**: Todo o mapeamento do banco de dados deve ser feito via **Fluent API** na camada de `Infrastructure`.

### B. SPEC-First (Lei nÂº 2)
A IA nÃ£o "projeta" soluÃ§Ãµes. A IA **traduz** o legado baseado na SPEC do mÃ³dulo.
- Se a SPEC diz "Tarefa 1.1", implemente APENAS a Tarefa 1.1.
- NÃ£o refatore, nÃ£o limpe, nÃ£o melhore o legado alÃ©m do que a SPEC orienta.
- Em caso de ambiguidade: **PARE E PERGUNTE**.

### C. Git-First (Lei nÂº 3)
O controle de versÃ£o segue o fluxo estrito:
- Branch `main`: CÃ³digo estÃ¡vel, merge apenas de tags de release.
- Branch `develop`: IntegraÃ§Ã£o de funcionalidades.
- Branches `feat/`, `fix/`, `docs/`: Trabalho isolado.
- **Merge**: Use `--no-ff` para manter o histÃ³rico de branches visÃ­vel.

---

## 3. PadrÃµes de CÃ³digo (C# 12+)

- **Records**: Use `public sealed record` para DTOs, Commands, Queries e Value Objects.
- **ColeÃ§Ãµes**:
  - ExposiÃ§Ã£o pÃºblica: `IReadOnlyList<T>`.
  - Interna de agregado: `List<T>` privada + `AsReadOnly()`.
  - **PROIBIDO**: Classes `*Lista` (legado).
- **Async/Await**: Todo I/O deve ser `async` e propagar o `CancellationToken`.
- **Fluxo de Erro**:
  - Use `Result<T>` e `ValidationResult` para erros de negÃ³cio esperados.
  - ExceÃ§Ãµes (`VersatusException`) somente para falhas crÃ­ticas de infraestrutura ou invariantes.
  - **PROIBIDO**: Usar `try-catch` para controlar fluxo de negÃ³cio normal.

---

## 4. Como Responder ao UsuÃ¡rio

1. **Confirme a Branch**: Indique em qual branch vocÃª estÃ¡ operando.
2. **Cite a Task**: Indique qual seÃ§Ã£o da SPEC vocÃª estÃ¡ implementando.
3. **Prove com CÃ³digo**: Mostre o arquivo criado e seu local.
4. **Resumo Git**: Informe os comandos de commit e merge realizados.
5. **Update do Log (OBRIGATÃ“RIO)**: Confirme que vocÃª detalhou o progresso na SeÃ§Ã£o 5 deste documento antes de encerrar.

---

**Cumpra estas regras e seremos parceiros. Ignore-as e vocÃª quebrarÃ¡ o Versatus.**

---

## 5. Log de Progresso e Handoff (Atualizado: 2026-06-05)
# CONTRATO DE EXECUÃ‡ÃƒO â€” IA VERSATUS

> ðŸ’¡ **DICA PARA O USUÃ�RIO:** Se a IA parecer "esquecida", use o prompt em [specs/05-ONBOARDING-IA-PROMPT.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/05-ONBOARDING-IA-PROMPT.md).

Este documento define o protocolo obrigatÃ³rio para qualquer IA que atue no projeto **Versatus.Net8**. A falha em seguir este protocolo resultarÃ¡ em quebra de arquitetura e integridade do sistema.

---

## 1. O Ritual de InÃ­cio (ObrigatÃ³rio)

Antes de escrever qualquer linha de cÃ³digo, a IA **DEVE**:
1. Ler o Indice Geral (`specs/00-INDICE-GERAL.md`).
2. Ler as Regras Anti-AlucinaÃ§Ã£o (`specs/03-REGRAS-ANTI-ALUCINACAO.md`).
3. Ler as Boas PrÃ¡ticas e DecisÃµes de ColeÃ§Ãµes (`specs/decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`).
4. Ler a DecisÃ£o de ORM e Pureza de DomÃ­nio (`specs/decisoes/DEC-001-ORM.md`).
5. **Ler o Log de Progresso e Handoff (SeÃ§Ã£o 5 deste documento)** para saber o ponto exato de parada e evitar trabalho redundante.

---

## 2. Leis Fundamentais de Arquitetura

### A. Pureza de DomÃ­nio (Lei nÂº 1)
As classes na pasta `Domain/` (Entidades, Value Objects) devem ser **POCOs puras**.
- **PROIBIDO**: `using System.ComponentModel.DataAnnotations;`
- **PROIBIDO**: Atributos como `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]`.
- **OBRIGATÃ“RIO**: Todo o mapeamento do banco de dados deve ser feito via **Fluent API** na camada de `Infrastructure`.

### B. SPEC-First (Lei nÂº 2)
A IA nÃ£o "projeta" soluÃ§Ãµes. A IA **traduz** o legado baseado na SPEC do mÃ³dulo.
- Se a SPEC diz "Tarefa 1.1", implemente APENAS a Tarefa 1.1.
- NÃ£o refatore, nÃ£o limpe, nÃ£o melhore o legado alÃ©m do que a SPEC orienta.
- Em caso de ambiguidade: **PARE E PERGUNTE**.

### C. Git-First (Lei nÂº 3)
O controle de versÃ£o segue o fluxo estrito:
- Branch `main`: CÃ³digo estÃ¡vel, merge apenas de tags de release.
- Branch `develop`: IntegraÃ§Ã£o de funcionalidades.
- Branches `feat/`, `fix/`, `docs/`: Trabalho isolado.
- **Merge**: Use `--no-ff` para manter o histÃ³rico de branches visÃ­vel.

---

## 3. PadrÃµes de CÃ³digo (C# 12+)

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
4. **Resumo Git**: Informe os comandos de commit e merge realizados.
5. **Update do Log (OBRIGATÓRIO)**: Confirme que você detalhou o progresso na Seção 5 deste documento antes de encerrar.

---

**Cumpra estas regras e seremos parceiros. Ignore-as e você quebrará o Versatus.**

---

## 5. Log de Progresso e Handoff (Atualizado: 2026-06-06)

Este log serve para que a próxima instância da IA saiba exatamente onde o trabalho parou. **Ao finalizar sua sessão, atualize esta tabela.**

| Módulo Atual | Fase / Status | Task Atual | Branch Ativa | Observação Crítica |
| :--- | :--- | :--- | :--- | :--- |
| **MOD-01** | ✅ Concluído | 9.2 (Final) | `develop` | Framework Base e Infra base finalizados. |
| **MOD-02** | ✅ Concluído | 11.0 (Validação)| `develop` | Implementadas validações estritas de CPF/CNPJ (Módulo 11), Obrigatoriedade, Razão Social e bypass de estrangeiros baseados em parâmetros do banco. |
| **MOD-07** | 🔄 Em progresso | 4.3 (Fase 4) | `feat/mod-02-demo` | Concluída Fase 4 (ICMS, Substituição Tributária, Mapeamentos, Regimes e Vigências). |

### Histórico Recente de Decisões:
- **2026-06-06 (GestaoTributo):** Conclusão da Fase 4 do MOD-07. Criadas as 9 entidades de domínio em `Domain/ICMS/` (GrupoTributarioICMS, GrupoTributarioInventarioICMS, TributoIcmsSubstituicaoEstoque, DetalheUfTributacao, DetalheCidadeTributacao, RegimeTributarioVigencia, SimplesNacional, SimplesNacionalTributo e PartilhaICMSVigencia). Configurados mapeamentos Fluent API correspondentes preservando chaves estrangeiras, precisions decimais e nomes de tabelas/colunas legadas em maiúsculas. Registrados no `TributoDbContext` e validados via testes de integração passando 100% (9/9).
- **2026-06-06 (GestaoTributo):** Conclusão da Fase 3 do MOD-07. Implementados enums tributários, entidades de domínio (Tributacao, RegraTributo, RegraTributoConfiguracao, RegraTributacaoEspecial, AplicacaoProduto, AplicacaoProdutoTributo, AplicacaoNaturezaOperacao, AplicacaoEspecial e SituacaoTributaria), mapeamentos Fluent API correspondentes no EF Core 8 para base real, atualizado o TributoDbContext e adicionados testes de integração validando a persistência com 100% de sucesso.
- **2026-06-06 (AcessoGlobal):** Refatorado `EntidadeService.cs` para suportar a esteira completa de validação de negócios (Razão Social, CPF/CNPJ matemática e obrigatório, unicidade, bypass de estrangeiros com checagem de país). Adicionados métodos auxiliares para obter país via cidade/filial no `EntidadeRepository`. Adicionados 6 testes unitários no `EntidadeServiceTests.cs` cobrindo estes cenários e passando com 100% de sucesso.
- **2026-06-06 (Governança):** Adicionada a `REGRA 16 — Paridade Estrita de Validações e Parâmetros` no arquivo `specs/03-REGRAS-ANTI-ALUCINACAO.md` para evitar regressões futuras nessas validações críticas de sistema.
- **2026-06-05 (GestaoTributo):** Conclusão das Fases 1 e 2 do MOD-07. Criado o projeto Class Library `Versatus.GestaoTributo`, implementados os modelos de domínio e mappings Fluent API para NCM (TRBCLASSIFICACAOFISCAL), CFOP (TRBCFOP), CEST (TRBCEST) e dependências. Criado o projeto de testes `Versatus.GestaoTributo.Tests` com 3 testes de integração integrados e passando.
- **2026-06-05 (AcessoGlobal):** Configuração e conexão com o banco de dados SQL Server real (`localhost\SQLEXPRESS2008`) utilizando as credenciais fornecidas pelo desenvolvedor. Ajustados os mapeamentos Fluent API em `EntidadeMapping.cs` para refletir os nomes corretos da base real (`IDINDICADORCONTRIBUINTEICMS` e `IDTIPOPLATAFORMA`), e o mock do usuário para ID `1` (administrador existente).
- **2026-06-05 (AcessoGlobal):** Conclusão das Fases 7, 8, 9 e 10 do MOD-02. Adicionadas as entidades `Usuario`, `Perfil`, `Banco`, `FormaPagamento`, `SerieDocumento`, `SerieDocumentoFilial` e `Parametro`, com seus respectivos mapeamentos Fluent API, injeção de dependência e testes de integração de mapeamento.
- **2026-06-05 (AcessoGlobal):** Configuração do `IDesignTimeDbContextFactory` no projeto de inicialização (`Versatus.AcessoGlobal.Demo`) para suportar a criação de EF Core Migrations do SQL Server. Gerada a Migration inicial `InitialCreate`.
- **2026-06-05 (AcessoGlobal):** Correção do formato do CPF de teste de 14 para 11 dígitos numéricos limpos no `Program.cs` da Demo para conformidade com a validação da tabela `GloEntidadeFisica` do banco de dados real.
- **2026-05-05 (Git):** Merge da branch `setup/acesso-global-project` para `develop` após aprovação do usuário.
- **2026-05-05 (AcessoGlobal):** Implementação de testes de integração com SQLite In-Memory para validar mapeamentos Fluent API e relacionamentos 1:1.
- **2026-05-05 (AcessoGlobal):** Criação do projeto de testes `Versatus.AcessoGlobal.Tests` utilizando xUnit, Moq e FluentAssertions.
- **2026-05-05 (AcessoGlobal):** Implementação de repositórios especializados (`Cliente`, `Fornecedor`, etc.) e serviços de validação de endereços.
- **2026-05-05 (AcessoGlobal):** Implementação do `EntidadeService` com lógica de geração de sequencial (via `IGeradorSequencial`) e validação de unicidade de CPF/CNPJ.
- **2026-05-05 (AcessoGlobal):** Finalização das especializações de papéis (Cliente, Fornecedor, Funcionario, Transportadora) com seus respectivos mappings Fluent API and enums originais.
- **2026-05-05 (AcessoGlobal):** Migração do `Cliente` (tabela `GloCliente`) com suporte a enums específicos (`SituacaoClienteSPC`, `TipoImovel`) e relacionamento 1:1 com `Entidade`.
- **2026-05-05 (AcessoGlobal):** Implementação da `Entidade` base preservando o padrão de papéis (roles) via booleano do legado para garantir compatibilidade com a tabela `GloEntidade`.
- **2026-05-05 (AcessoGlobal):** Adição explícita dos pacotes `Microsoft.EntityFrameworkCore` e `Microsoft.EntityFrameworkCore.Relational` ao projeto `Versatus.AcessoGlobal` para suportar mapeamentos Fluent API independentes.
- **2026-04-28 (Global):** Padronização total de nomenclatura para **Inglês** em todas as pastas físicas e namespaces (`Repositories`, `Context`, `Exceptions`). O `Versatus.Framework` foi totalmente refatorado.

### Próxima Ação Pendente:
- Iniciar **Fase 5: SPED e Escrituração Fiscal** do **MOD-07 (Gestão de Tributos)**.
