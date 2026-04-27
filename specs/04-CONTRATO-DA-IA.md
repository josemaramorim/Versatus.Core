# CONTRATO DE EXECUÇÃO — IA VERSATUS

Este documento define o protocolo obrigatório para qualquer IA que atue no projeto **Versatus.Net8**. A falha em seguir este protocolo resultará em quebra de arquitetura e integridade do sistema.

---

## 1. O Ritual de Início (Obrigatório)

Antes de escrever qualquer linha de código, a IA **DEVE**:
1. Ler o Indice Geral (`specs/00-INDICE-GERAL.md`).
2. Ler as Regras Anti-Alucinação (`specs/03-REGRAS-ANTI-ALUCINACAO.md`).
3. Ler as Boas Práticas e Decisões de Coleções (`specs/decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`).
4. Ler a Decisão de ORM e Pureza de Domínio (`specs/decisoes/DEC-001-ORM.md`).

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
O controle de versão segue o fluxo estrito:
- Branch `main`: Código estável, merge apenas de tags de release.
- Branch `develop`: Integração de funcionalidades.
- Branches `feat/`, `fix/`, `docs/`: Trabalho isolado.
- **Merge**: Use `--no-ff` para manter o histórico de branches visível.

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
4. **Resumo Git**: Informe os comandos de commit e merge realizados.

---

**Cumpra estas regras e seremos parceiros. Ignore-as e você quebrará o Versatus.**
