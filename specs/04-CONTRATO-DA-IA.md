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

## 5. Log de Progresso e Handoff (Atualizado: 2026-06-11)

Este log serve para que a próxima instância da IA saiba exatamente onde o trabalho parou. **Ao finalizar sua sessão, atualize esta tabela.**

| Módulo Atual | Fase / Status | Task Atual | Branch Ativa | Observação Crítica |
| :--- | :--- | :--- | :--- | :--- |
| **MOD-01** | ✅ Concluído | 9.2 (Final) | `develop` | Framework Base e Infra base finalizados. |
| **MOD-02** | ✅ Concluído | 11.0 (Validação)| `develop` | Implementadas validações estritas de CPF/CNPJ (Módulo 11), Obrigatoriedade, Razão Social e bypass de estrangeiros baseados em parâmetros do banco. |
| **MOD-07** | 🔄 Em progresso | 4.3 (Fase 4) | `feat/mod-02-demo` | Concluída Fase 4 (ICMS, Substituição Tributária, Mapeamentos, Regimes e Vigências). Fase 5 (SPED) aguarda análise — ver `specs/prompts-execucao/MOD-07-FASE5-ANALISE-SPED.md`. |
| **MOD-03** | 🔄 Em progresso | Fases 1-3 prontas | `develop` | Prompts de execução criados em `specs/prompts-execucao/`. Iniciar por `MOD-03-FASES1-3-EXECUTION-PROMPT.md`. |

### Histórico Recente de Decisões:
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
- Executar **MOD-03 Fases 1-3** usando o prompt em `specs/prompts-execucao/MOD-03-FASES1-3-EXECUTION-PROMPT.md`.
- Executar **Fase 5 SPED (análise)** usando `specs/prompts-execucao/MOD-07-FASE5-ANALISE-SPED.md`.
