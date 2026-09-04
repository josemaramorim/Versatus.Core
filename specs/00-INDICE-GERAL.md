# VERSATUS — Índice Geral de SPECs
## Migração .NET Framework → .NET Core/8 — Método do Estrangulamento

> **Versão:** 1.0  
> **Data:** 2026-04-27  
> **Estratégia:** Strangler Fig Pattern (Padrão de Estrangulamento)  
> **Fonte:** `projeto_tag_1906` (snapshot de referência congelado)

---

## O que é este conjunto de documentos?

Este diretório contém os **documentos de SPEC** (Especificação Técnica) que guiam a migração
do sistema Versatus de .NET Framework para .NET Core 8. Cada SPEC é um contrato claro e
imutável que define o que deve ser feito **antes de qualquer linha de código ser escrita**.

### Princípios fundamentais

| Princípio | Descrição |
|---|---|
| **SPEC-FIRST** | Nenhum código é escrito sem uma SPEC aprovada |
| **Estrangulamento gradual** | O sistema legado continua funcionando enquanto o novo cresce ao redor |
| **Módulo por vez** | Um módulo completo antes de avançar para o próximo |
| **Sem invenção** | A IA documenta o que existe; não inventa novas arquiteturas |
| **Rastreabilidade** | Cada classe nova mapeia exatamente uma classe legada |
| **Git-first** | Use Git para versionar SPECs e código, com `develop`, `release/*` e `main` como branches de integração controlados |

---

## Estrutura dos Documentos

```
specs/
├── 00-INDICE-GERAL.md                  ← Este arquivo
├── 01-VISAO-GERAL-ARQUITETURA.md       ← Arquitetura legada + nova + regras de migração
├── 02-GUIA-SPEC.md                     ← Como ler e usar as SPECs
├── 03-REGRAS-ANTI-ALUCINACAO.md        ← Regras para evitar que a IA invente coisas
│
├── modulos/
│   ├── MOD-01-FRAMEWORK.md             ← Framework base (ObjetoNegocio, ListBase, Transacao)
│   ├── MOD-02-ACESSO-GLOBAL.md         ← Entidades, clientes, fornecedores, usuários
│   ├── MOD-03-GESTAO-MATERIAL.md       ← Produtos, estoques, movimento de estoque
│   ├── MOD-04-FATURAMENTO.md           ← Documentos de venda, NF-e, pedidos
│   ├── MOD-05-GESTAO-FINANCEIRA.md     ← Documentos financeiros, liquidação, caixa (rascunho v1 — ver MOD-05/)
│   ├── MOD-05/                          ← SDD: spec.md, dependency-graph.md, plan.md, tasks.md, matrizes
│   ├── MOD-06-GESTAO-COMPRA.md         ← Cotações, recebimento, requisições
│   ├── MOD-07-GESTAO-TRIBUTO.md        ← Tributação, CFOP, NCM, SPED
│   ├── MOD-08-NFe.md                   ← Nota Fiscal Eletrônica
│   ├── MOD-09-GESTAO-RH.md             ← Folha de pagamento, funcionários
│   ├── MOD-10-GESTAO-CONTRATO.md       ← Contratos, reajustes, prorrogações
│   ├── MOD-11-GESTAO-OS.md             ← Ordem de Serviço
│   ├── MOD-12-GESTAO-PRODUCAO.md       ← Produção, ordens de produção
│   └── MOD-13-MODULOS-SECUNDARIOS.md  ← Armazém, Frota, Locação, Obra, Transporte
│
├── prompts-execucao/                   ← ⭐ PROMPTS PRONTOS PARA OUTRA IA EXECUTAR
│   ├── 00-GUIA-HANDOFF.md             ← Leia este PRIMEIRO — passo a passo de handoff
│   ├── MOD-03-FASES1-3-EXECUTION-PROMPT.md  ← Criar projeto + Unidades + Classificação
│   ├── MOD-03-FASE4-ANALISE-PRODUTO.md      ← Analisar Produto.cs (189 KB)
│   ├── MOD-03-FASES5-8-EXECUTION-PROMPT.md  ← Grades, Composição, Lote, Série
│   ├── MOD-03-FASES9-10-ANALISE-ESTOQUE.md  ← Analisar Estoque + MovimentoEstoque
│   ├── MOD-03-FASES11-12-EXECUTION-PROMPT.md← Auxiliares + Repos + Migration
│   └── MOD-07-FASE5-ANALISE-SPED.md         ← Analisar EFD + EFDPisCofins + SPED
│
└── decisoes/
    ├── DEC-001-ORM.md                  ← Substituição do Gentle.NET
    ├── DEC-002-REMOTING.md             ← Substituição do .NET Remoting
    ├── DEC-003-TRANSACAO.md            ← Padrão de transações no novo sistema
    ├── DEC-004-ORDEM-MIGRACAO.md       ← Ordem de prioridade dos módulos
    ├── DEC-005-COLECOES-E-BOAS-PRATICAS.md ← Coleções nativas .NET, sem ListBase
    ├── DEC-006-INTEGRACAO-SEGURA.md    ← API Key + bypass localhost (legado ↔ API nova)
    └── DEC-007-SHAREDKERNEL-ESCOPO.md  ← Versatus.SharedKernel vira kernel do ERP
```

---

## Fases da Migração

### Fase 0 — Fundação (pré-requisito tudo)
> SPECs: `MOD-01-FRAMEWORK`, `DEC-001`, `DEC-002`, `DEC-003`

Criar a infraestrutura base do novo sistema sem tocar no legado.

### Fase 1 — Núcleo (módulos sem dependências de negócio)
> SPECs: `MOD-02`, `MOD-07`

Entidades globais e tributação são a fundação de todos os outros módulos.

### Fase 2 — Material e Compras
> SPECs: `MOD-03`, `MOD-06`

Produtos e compras dependem do núcleo mas não de faturamento.

### Fase 3 — Faturamento e Fiscal
> SPECs: `MOD-04`, `MOD-08`

O coração do negócio. Requer Fase 1 e 2 completos.

### Fase 4 — Financeiro
> SPECs: `MOD-05`

Depende de faturamento para gerar documentos financeiros.

### Fase 5 — Módulos de Gestão
> SPECs: `MOD-09`, `MOD-10`, `MOD-11`, `MOD-12`, `MOD-13`

Módulos mais isolados, podem ser atacados em paralelo após Fase 1.

---

## Status dos Módulos

| SPEC | Módulo | Fase | Status | Observações |
|---|---|---|---|---|
| MOD-01 | Framework Base | 0 | ✔️ Concluído | |
| MOD-02 | Acesso Global | 1 | ✔️ Concluído | Fases 1-10 completas, 32 testes passando |
| MOD-03 | Gestão Material | 2 | 🔄 Em progresso | Fases 1-3 prontas — ver `prompts-execucao/` |
| MOD-04 | Faturamento | 3 | 📝 Rascunho | Aguarda MOD-03 |
| MOD-05 | Gestão Financeira | 4 | 🔄 SDD completo (pré-implementação) | `specs/modulos/MOD-05/` — spec v2.3, clarify (13 CLR), plan, data-model, research, contracts, tasks (~90), analyze parcial. Branch `docs/mod-05-sdd`. |
| MOD-06 | Gestão Compra | 2 | 📝 Rascunho | |
| MOD-07 | Gestão Tributo | 1 | 🔄 Em progresso | Fases 1-4 OK; Fase 5 SPED — ver `prompts-execucao/` |
| MOD-08 | NFe | 3 | 📝 Rascunho | |
| MOD-09 | Gestão RH | 5 | 📝 Rascunho | |
| MOD-10 | Gestão Contrato | 5 | 📝 Rascunho | |
| MOD-11 | Gestão OS | 5 | 📝 Rascunho | |
| MOD-12 | Gestão Produção | 5 | 📝 Rascunho | |
| MOD-13 | Módulos Secundários | 5 | 📝 Rascunho | |


**Legenda:** 📝 Rascunho | ✅ Aprovado | 🔄 Em progresso | ✔️ Concluído

---

## Como trabalhar com estes documentos

1. **Leia primeiro:** `02-GUIA-SPEC.md` e `03-REGRAS-ANTI-ALUCINACAO.md`
2. **Escolha um módulo** pela ordem das fases acima
3. **Leia a SPEC do módulo** correspondente completamente
4. **Somente então** peça à IA para gerar código
5. **Nunca pule SPECs** — o estrangulamento exige ordem

---

*Gerado em: 2026-04-27 — Congelado com base em `projeto_tag_1906`*
