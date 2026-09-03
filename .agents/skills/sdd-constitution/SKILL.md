---
name: sdd-constitution
description: Gera ou atualiza a Constituição de Engenharia (specs/memory/constitution.md) consolidando as 13 Leis do AGENTS.md, as 17 Regras Anti-Alucinação e as decisões DEC-001..006 num gate executável. Use ao iniciar o fluxo SDD num repositório ou quando AGENTS.md / regras / DECs mudarem.
---

# Skill: sdd-constitution

Primeira etapa do fluxo **SDD (Spec-Driven Development)** do Versatus. Produz e mantém
`specs/memory/constitution.md` — a **fonte única de verificação** que todas as demais
skills `sdd-*` usam como gate.

> A constituição **NÃO cria regra nova**. Ela consolida regras já ratificadas. Se você
> sentir necessidade de uma regra inexistente, isso é um `DÚVIDA:` para o usuário — não
> invente cláusula.

---

## 1. Quando disparar

- Repositório ainda não tem `specs/memory/constitution.md`.
- `.agents/AGENTS.md`, `specs/03-REGRAS-ANTI-ALUCINACAO.md` ou qualquer `specs/decisoes/DEC-*`
  foi alterado.
- Mudança de plataforma (runtime, ORM, convenção de nomes de projeto).
- Comando `/constitution`.

---

## 2. Entradas obrigatórias (leia na íntegra, sem confiar em memória)

1. `.agents/AGENTS.md` — as 13 Leis.
2. `specs/03-REGRAS-ANTI-ALUCINACAO.md` — as 17 Regras + glossário.
3. `specs/decisoes/DEC-001-ORM.md` … `DEC-006-INTEGRACAO-SEGURA.md`.
4. `specs/04-CONTRATO-DA-IA.md` — Leis de arquitetura + fluxo Git.
5. Estado **real** do repositório para a Seção 0 (Plataforma):
   - `grep -rn "TargetFramework" --include=*.csproj src tests`
   - versões de pacote em `Versatus.GestaoTributo.csproj`
   - convenção de nome de projeto/namespace já em uso.

---

## 3. Estrutura obrigatória do `constitution.md`

| Seção | Conteúdo |
| :--- | :--- |
| **0. Plataforma** | Tabela: runtime, linguagem, ORM, banco alvo, testes, nome de projeto. Valores tirados do repo real, não de docs antigos. Marcada como imutável até nova versão. |
| **Artigos I–XI** | Um artigo por eixo: SPEC-First · Fidelidade ao Legado · Pureza de Domínio · Clean Architecture/Controllers · Coleções e C# Moderno · Fluxo de Erro · Transações/CQRS · Cross-Module/Estrangulamento · Testes como Trava · Git/Handoff · Manutenção de Skills. Cada cláusula cita a Lei/Regra/DEC de origem. |
| **12. Gate de Conformidade** | Checklist `PASS/FAIL` por subárea (plataforma, spec-first/fidelidade, arquitetura, transação/CQRS/cross-module, testes, governança). É isto que `/analyze` executa. |
| **13. Glossário** | Mapa legado → novo. |

Cada cláusula deve ser **verificável** (frase no imperativo com critério objetivo), não
aspiracional. Ruim: "o código deve ser limpo". Bom: "Controllers não injetam `DbContext`
nem `IRepository`".

---

## 4. Regras de escrita

1. **Rastreie a origem** de cada cláusula: `(Lei 4 · Regra 17)`, `(DEC-003)`, etc.
2. **Não contradiga** os documentos ratificados. Se detectar contradição entre eles,
   registre em `## Conflitos detectados` no fim do arquivo e reporte ao usuário — não
   resolva sozinho.
3. **Versione**: cabeçalho com `Versão`, `Criada`/`Atualizada`, lista de documentos que
   ratifica. Bump de versão a cada alteração da Seção 0.
4. **Português** no texto; termos técnicos e nomes de arquivo/classe como no repo.
5. O gate da Seção 12 deve ter itens 1:1 com as cláusulas dos artigos — nada no gate sem
   artigo correspondente, nenhum artigo sem item de gate.

---

## 5. Saída e Git

- Escreva/atualize `specs/memory/constitution.md`.
- Branch `docs/constitution` (ou a branch de docs SDD ativa). Nunca `develop`/`main`.
- Commit: `docs(sdd): cria/atualiza Constituição de Engenharia vX.Y`.
- Atualize `specs/00-INDICE-GERAL.md` se ele listar documentos de `specs/`.
- **PARE e peça revisão do usuário.** A constituição só entra em vigor após "Aprovado".
