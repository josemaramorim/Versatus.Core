---
name: writing-skills
description: Use ao criar novas skills, editar skills existentes ou verificar o funcionamento de skills antes de colocá-las em produção.
---

# Criação de Skills (Writing Skills)

## Visão Geral

**Criar skills É o Desenvolvimento Guiado por Testes (TDD) aplicado à documentação de processos da IA.**

As **Skills do projeto** ficam localizadas na pasta `.agents/skills/` no repositório (ou em `C:\Users\WIN10\.gemini\config\skills\` no escopo global do usuário).

No ciclo de criação de uma skill, você cria cenários de teste/pressão com a IA ou subagentes, observa o comportamento falhar (comportamento baseline sem a skill), escreve o documento da skill (`SKILL.md`), observa o teste passar (a IA segue a skill) e refatora para fechar brechas ou ambiguidades.

**Princípio Fundamental:** Se você não viu a IA falhar sem a skill, você não sabe se a skill está ensinando a coisa certa.

> [!IMPORTANT]
> **REQUISITO OBRIGATÓRIO:** Você DEVE compreender a skill `test-driven-development` antes de utilizar esta skill. O ciclo RED-GREEN-REFACTOR do TDD é a base para a criação de skills eficazes.
> Para consultar as diretrizes oficiais da Anthropic sobre criação de skills, leia [references/boas-praticas-anthropic.md](references/boas-praticas-anthropic.md).

---

## O que é uma Skill?

Uma **skill** é um guia de referência prático para técnicas, padrões ou ferramentas comprovadas. As skills ajudam a IA a descobrir e aplicar abordagens eficientes em tarefas complexas.

- **Skills SÃO:** Técnicas reutilizáveis, padrões de arquitetura, ferramentas, guias de referência.
- **Skills NÃO SÃO:** Narrativas de como você resolveu um problema específico uma única vez.

---

## Mapeamento TDD para Criação de Skills

| Conceito TDD | Criação de Skill |
|---|---|
| **Caso de Teste** | Cenário de pressão / instrução dada à IA sem a skill |
| **Código de Produção** | Documento da skill (`SKILL.md`) |
| **Teste Falhou (RED)** | A IA viola a regra ou alucina sem a skill (baseline) |
| **Teste Passou (GREEN)** | A IA cumpre 100% dos requisitos quando a skill está presente |
| **Refatoração (REFACTOR)** | Fechar brechas de interpretação mantendo o texto sucinto |
| **Escrever o teste primeiro** | Executar o cenário ANTES de escrever o `SKILL.md` |
| **Assistir à falha** | Documentar as desculpas e racionalizações exatas da IA |
| **Código Mínimo** | Escrever a skill focando estritamente em sanar as falhas observadas |

---

## Quando Criar uma Skill

**Crie uma skill quando:**
- A técnica não é intuitivamente óbvia para a IA.
- Você precisará dessa referência repetidamente entre múltiplos projetos ou módulos.
- O padrão se aplica de forma ampla no repositório.
- A equipe/projeto se beneficiará da padronização.

**NÃO crie uma skill para:**
- Soluções pontuais de uma única vez.
- Convenções específicas de um único arquivo (coloque no `AGENTS.md` ou na Spec).
- Regras puramente mecânicas que podem ser validadas via script ou regex.

---

## Estrutura de Pastas de uma Skill

```
.agents/skills/
  nome-da-skill/
    SKILL.md                    # Instrução principal (Obrigatório)
    references/                 # Documentos de referência extensos (>100 linhas)
    examples/                   # Exemplos práticos
    templates/                  # Modelos e ferramentas reutilizáveis
```

---

## Estrutura do Arquivo `SKILL.md`

### Frontmatter (YAML):
- Dois campos obrigatórios: `name` e `description`.
- `name`: Apenas letras, números e hífens em minúsculas (ex: `spec-generator`, `code-auditor`).
- `description`: Escreva sempre em 3ª pessoa e comece com `"Use quando..."` ou `"Use ao..."` focando apenas nas **condições de disparo** (sintomas e momentos), **NUNCA resumindo o fluxo da skill**.

```yaml
---
name: nome-da-skill
description: Use quando [condições exatas de disparo e sintomas]
---
```

---

## Otimização para Descoberta de Skills (SDO — Skill Discovery Optimization)

### 1. Campo Description Focado nas Condições de Disparo
A IA lê o campo `description` para decidir se deve carregar o arquivo `SKILL.md`. 

> [!CAUTION]
> **CRÍTICO: O campo `description` NUNCA deve resumir o passo a passo da skill.**
> Se a descrição resumir o fluxo, a IA pode tentar seguir a descrição curta em vez de ler o `SKILL.md` completo.

- **❌ RUIM (Resume o fluxo):** `description: Use para TDD - escreva o teste primeiro, veja falhar, crie o código e refatore.`
- **✅ BOM (Foca apenas nas condições):** `description: Use ao implementar qualquer funcionalidade ou correção de bug, antes de escrever código de produção.`

### 2. Eficiência de Tokens (Conciso e Direto)
- Mantenha o corpo do `SKILL.md` abaixo de 500 linhas.
- Mova documentações extensas (>100 linhas) ou esquemas grandes para a subpasta `references/`.
- Assuma que a IA é inteligente e forneça apenas as regras específicas do projeto.
