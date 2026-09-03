---
name: sdd-clarify
description: Conduz uma rodada estruturada de perguntas sobre as ambiguidades da spec de módulo, registra perguntas e respostas em specs/modulos/MOD-XX/clarify.md e reincorpora as decisões na spec. Etapa 2 do fluxo SDD, entre /specify e /plan.
---

# Skill: sdd-clarify

Etapa 2 do fluxo SDD. Fecha as lacunas da `spec.md` **antes** de planejar, para o `/plan`
não ter que adivinhar. Materializa a Regra 7 ("em dúvida, PARE e PERGUNTE") como um passo
formal e rastreável.

---

## 1. Pré-condição

`specs/modulos/MOD-XX/spec.md` existe e foi aprovada pelo usuário.

---

## 2. Como levantar as perguntas

Varra, em ordem:

1. **Seção 9 (DÚVIDAS)** da `spec.md` — cada `DÚVIDA:` vira ≥1 pergunta.
2. **Inventário** — classes cujo tipo/propósito ficou "?", tabela `DÚVIDA`, ou entidade
   sem tabela identificável.
3. **Mapa de dependências** — cross-módulo onde não está claro se vira `int` lógico,
   chamada de serviço ou está fora de escopo nesta rodada.
4. **Épicos** — fronteira de escopo (entra/não entra nesta conversão), quais têm frontend.
5. **Enums** — valores/di­vergências não resolvidos.
6. **Estrangulamento** — divergência entre DTO de `Servidor.Strangler` e o que a spec
   descreve.
7. **Kernel `Geral`** — destino do código compartilhado (projeto novo vs. `Versatus.Framework`).
8. **Infra de dados** — disponibilidade do banco legado para inspeção `INFORMATION_SCHEMA`
   no `/plan`.

Agrupe perguntas equivalentes. Priorize as que **mudam o plano** (escopo, arquitetura,
ordem de épicos) sobre as cosméticas.

---

## 3. Formato da rodada

- Faça as perguntas em blocos pequenos (recomendado ≤ 5 por vez), cada uma com:
  - contexto de 1 linha (arquivo/seção de origem),
  - 2–4 opções objetivas quando possível + opção livre,
  - impacto ("se A → o plano faz X; se B → Y").
- Use o mecanismo de pergunta do cliente (ex.: `AskUserQuestion`) quando disponível.
- **Não presuma.** Sem resposta = pergunta continua aberta; não escolha "a mais razoável".

---

## 4. Arquivo `specs/modulos/MOD-XX/clarify.md`

```markdown
# Clarificações — MOD-XX

> Rodada iniciada em <data>. Cada item alimenta uma alteração rastreável na spec.

| ID | Origem (seção/arquivo) | Pergunta | Opções | Resposta do usuário | Efeito na spec |
|----|------------------------|----------|--------|---------------------|----------------|
| CLR-01 | spec §9 DÚVIDA 1 | ... | A / B / C | B (2026-09-03) | §6 Épico E14 movido para fase final |
| CLR-02 | inventário linha 44 | ... | ... | pendente | — |
```

Regras:
- Uma linha por pergunta. `pendente` enquanto não respondida.
- Registre **data** da resposta.
- Toda resposta com efeito na spec gera edição correspondente em `spec.md` **no mesmo
  commit**, e a coluna "Efeito na spec" aponta a seção alterada.

---

## 5. Reincorporação

Depois da rodada:
1. Edite `spec.md` aplicando cada decisão (Seções 1, 6, 9 tipicamente). Remova da Seção 9
   as `DÚVIDA:` resolvidas; deixe as pendentes.
2. Se restarem pendências que **bloqueiam** o plano, o `/plan` não pode começar — avise o
   usuário quais são.
3. Atualize o "Histórico de Alterações" da spec.

---

## 6. Saída e gate

- `specs/modulos/MOD-XX/clarify.md` + edições em `spec.md`.
- Commit: `docs(mod-XX): rodada de clarificação (SDD etapa 2)`.
- **PARE.** `/plan` só roda quando não há pendência bloqueante e o usuário confirma.
