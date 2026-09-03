---
name: sdd-specify
description: Gera a especificação de MÓDULO (specs/modulos/MOD-XX/spec.md) a partir do código legado — inventário de classes, árvore de herança, mapa de dependências cross-módulo, regras de negócio numeradas e escopo. É a etapa 1 do fluxo SDD. Para uma tela CRUD isolada use spec-generator.
---

# Skill: sdd-specify

Etapa 1 do fluxo SDD. Transforma um módulo legado inteiro numa **especificação funcional
de módulo**, sem nenhuma decisão técnica de implementação (isso é do `/plan`).

> **Fronteira:** `sdd-specify` = escopo de **módulo** (dezenas/centenas de classes,
> épicos, ordem de dependência). `spec-generator` = **uma tela** CRUD isolada dentro de um
> módulo já planejado. Não confundir.

---

## 1. Entrada

Apenas o identificador do módulo (ex.: `MOD-05`). A skill descobre os caminhos por
convenção:

```
projeto_tag_1906/servidor/objeto de negócio/<modulo>/            (objetos de negócio)
projeto_tag_1906/cliente/cliente.aplicativo/aplicativo.<modulo>/ (formulários)
projeto_tag_1906/Geral/ , projeto_tag_1906/servidor/framework/   (kernel compartilhado)
```

Se o mapeamento `MOD-XX → pasta` não for óbvio, confirme com o usuário antes de varrer.

---

## 2. Ritual de início (obrigatório)

1. Ler `specs/memory/constitution.md` inteira.
2. Ler `specs/00-INDICE-GERAL.md` e o `specs/modulos/MOD-XX-*.md` existente (se houver — o
   novo `spec.md` **substitui** o arquivo único, preservando o histórico de alterações).
3. Ler `specs/04-CONTRATO-DA-IA.md` §5 (Log de Progresso) e
   `specs/decisoes/ALUCINACOES-DETECTADAS.md`.
4. Rodar `git branch --show-current` — trabalhar em `docs/mod-XX-sdd`.
5. Listar todos os arquivos do módulo legado e o `.csproj` legado (para extrair
   `ProjectReference` = dependências reais).

---

## 3. Varredura obrigatória

### 3.1 Inventário de classes
Tabela com **toda** classe `.cs` do módulo: arquivo, linhas, classe-pai, interfaces,
tabela legada (`[TableName]`/`[AutoSequencial]` se houver), tipo
(Entidade / Base / Operação / Relacionamento / Consulta / Lista / View / UserControl) e
1 linha de propósito.

### 3.2 Árvore de herança
Para cada entidade/operação, subir a hierarquia até `object` (ou classe do framework),
listando cada nível e o arquivo. Classes `*Base` e do projeto `Geral` entram no inventário
mesmo fora da pasta do módulo.

### 3.3 Mapa de dependências
- **Intra-módulo:** grafo de agregação (quem contém quem).
- **Cross-módulo:** cada `using Projeto.Servidor.Interface.<Outro>` / `ProjectReference` →
  tabela `Classe → Módulo dependido → o que consome`. Marca o que vira `int` lógico.
- **Kernel `Geral`:** subconjunto de tipos compartilhados efetivamente usados
  (`Lookup`, `RateioMovto`, `TransacaoBase`, `ValidationResult`, enums…).
- **Estrangulamento:** se existe `Servidor.Strangler.<Modulo>`, listar os DTOs já expostos.

### 3.4 Enums
Todo enum usado pelo módulo, com **valores inteiros** e origem. Referência:
`docs/analise_integracao_enumerados.md` e `references/enum_mapping_guide.md` se existirem.

### 3.5 Regras de negócio (macro)
Numerar `RN-XX-NNN` (`RN-05-001`, …) as regras de negócio de nível de módulo com o arquivo
de origem. A extração linha-a-linha de validações (Matriz RTV) e operações (Matriz ROT) é
feita depois pelas skills `legacy-validation-audit` e `legacy-operation-audit` — aqui só o
nível macro.

---

## 4. Estrutura de `specs/modulos/MOD-XX/spec.md`

```markdown
# SPEC — MOD-XX: <Nome do Módulo>

> Versão · Data · Fase · Status · Constituição ratificada (link + versão)

## 1. Visão Geral e Fronteiras
- O que o módulo faz, quem o alimenta, quem consome.
- **Em escopo** / **Fora de escopo** (nesta conversão) — explícito.
- Alcance do frontend nesta rodada (ex.: backend-first, React só no núcleo).

## 2. Inventário de Classes (tabela completa — §3.1)

## 3. Árvore de Herança (§3.2)

## 4. Mapa de Dependências (§3.3) + arquivo dependency-graph.md

## 5. Enums (§3.4)

## 6. Épicos (agrupamento por dependência)
Tabela: Épico · classes-núcleo · depende de · tem frontend nesta rodada?
(ordem topológica; bases primeiro, operações depois, integrações externas por último)

## 7. Regras de Negócio Macro (RN-XX-NNN — §3.5)

## 8. Artefatos SDD subsequentes (checklist)
- [ ] clarify.md   - [ ] plan.md / research.md / data-model.md / contracts/
- [ ] tasks.md     - [ ] matriz-rtv.md / matriz-rot.md   - [ ] analyze-report.md

## 9. Pendências e DÚVIDAS
Lista `DÚVIDA:` numerada — insumo direto do /clarify.

## Histórico de Alterações
```

---

## 5. Regras de conteúdo

1. **Zero decisão técnica.** Sem nome de tabela nova, sem DbContext, sem endpoint, sem
   estrutura de pasta do projeto novo. Isso é `/plan`.
2. **Sem inventar.** Toda linha do inventário vem de um arquivo real. Nome de tabela
   incerto → coluna "Tabela" recebe `DÚVIDA`.
3. **Não decidir escopo sozinho.** Se não estiver claro se algo entra nesta conversão,
   vira `DÚVIDA:` na Seção 9.
4. `*Lista` aparecem no inventário como tipo "Lista" e são marcadas "eliminada (Artigo V)".
5. Épicos terminam com a integração de terceiros (arquivos bancários, SPED, e-mail) como
   fase dedicada final.

---

## 6. Saída e gate

- Escreva `specs/modulos/MOD-XX/spec.md` + `specs/modulos/MOD-XX/dependency-graph.md`.
- Rode o Gate de Conformidade §12.2 da constituição (a parte aplicável a spec).
- Commit: `docs(mod-XX): spec de módulo (SDD etapa 1)`.
- Atualize `specs/00-INDICE-GERAL.md` (status do módulo → 🔄).
- **PARE e peça aprovação do usuário.** `/clarify` só roda após "Aprovado".
