# Decisão Técnica — DEC-004: Ordem de Migração dos Módulos
## Documento: decisoes/DEC-004-ORDEM-MIGRACAO.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** ✅ Decisão tomada — revisar conforme progresso

---

## Critérios de Priorização

A ordem de migração foi definida com base em:

1. **Dependência técnica** — módulos base devem vir antes dos que dependem deles
2. **Risco** — módulos mais críticos para o negócio têm prioridade
3. **Independência** — módulos mais isolados podem ser atacados mais cedo
4. **Retorno** — módulos que geram valor visível ao usuário têm prioridade

---

## Mapa de Dependências

```
MOD-01 (Framework Base)
  └─► MOD-02 (Acesso Global) ─── base de todos
        ├─► MOD-07 (Tributos)
        │     ├─► MOD-03 (Material)
        │     │     ├─► MOD-04 (Faturamento) ──► MOD-08 (NFe)
        │     │     │     └─► MOD-05 (Financeiro)
        │     │     └─► MOD-06 (Compras) ──────► MOD-05 (Financeiro)
        │     └─► MOD-09 (RH) ─────────────────► MOD-05 (Financeiro)
        ├─► MOD-10 (Contratos) ─────────────────► MOD-05 (Financeiro)
        ├─► MOD-11 (OS) ────────────────────────► MOD-04 (Faturamento)
        ├─► MOD-12 (Produção) ──────────────────► MOD-03 (Material)
        └─► MOD-13 (Secundários) — variados
```

---

## Ordem de Execução Definitiva

### FASE 0 — Fundação (sem atividade de negócio)

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 0.1 | Framework Base | MOD-01 | 1–2 semanas | — |

**O que entrega:** infraestrutura de projeto, DI, contexto de execução, gerador de sequencial,
padrão de exceções, repositório base.

---

### FASE 1 — Núcleo de Entidades

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 1.1 | Acesso Global (parcial: geográfico + empresa/filial/grupo) | MOD-02 §2.1 + §2.7 | 2 semanas | MOD-01 |
| 1.2 | Acesso Global (usuário + segurança) | MOD-02 §2.6 | 1 semana | 1.1 |
| 1.3 | Acesso Global (cliente + fornecedor) | MOD-02 §2.3 + §2.4 | 2 semanas | 1.1 |
| 1.4 | Acesso Global (pagamentos + finanças globais) | MOD-02 §2.8 + §2.10 | 1–2 semanas | 1.1 |
| 1.5 | Gestão Tributos (base: NCM, CFOP, Natureza, situação tributária) | MOD-07 §2.1 | 1–2 semanas | 1.1 |

> **Marco da Fase 1:** Login funcionando com JWT, CRUD de clientes/fornecedores,
> consulta de NCM/CFOP via nova API.

---

### FASE 2 — Material e Compras

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 2.1 | Gestão Material (classificação: grupo, tipo, unidade) | MOD-03 §2.8 | 1 semana | FASE 1 |
| 2.2 | Gestão Material (produto básico) | MOD-03 §2.1 | 2–3 semanas | 2.1 |
| 2.3 | Gestão Material (estoque e saldo) | MOD-03 §2.4 | 1–2 semanas | 2.2 |
| 2.4 | Gestão Material (lote, série, localização) | MOD-03 §2.5–2.7 | 1–2 semanas | 2.3 |
| 2.5 | Gestão Material (movimentação) | MOD-03 §2.6 | 1–2 semanas | 2.4 |
| 2.6 | Gestão Tributos (aplicação de tributos) | MOD-07 §2.2 | 2 semanas | 1.5 + 2.2 |
| 2.7 | Gestão Compras (cotação e recebimento) | MOD-06 | 2–3 semanas | 2.5 + 2.6 |

> **Marco da Fase 2:** Cadastro de produtos completo, movimentação de estoque
> via API, nota fiscal de entrada processada.

---

### FASE 3 — Faturamento e NF-e

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 3.1 | Faturamento (tipos de documento + config) | MOD-04 §2.3 + §2.4 | 1–2 semanas | FASE 2 |
| 3.2 | Faturamento (tabela de preço) | MOD-04 §2.6 | 1 semana | 3.1 |
| 3.3 | Faturamento (documento de venda — cabeçalho) | MOD-04 §2.1 (parcial) | 2 semanas | 3.1 |
| 3.4 | Faturamento (item de venda) | MOD-04 §2.2 | 2–3 semanas | 3.3 |
| 3.5 | Faturamento (situação — incluir/confirmar/cancelar) | MOD-04 §3.3 | 2 semanas | 3.4 |
| 3.6 | NFe (geração de XML) | MOD-08 | 3–4 semanas | 3.5 |
| 3.7 | NFe (comunicação SEFAZ) | MOD-08 | 2 semanas | 3.6 |

> **Marco da Fase 3:** Emissão completa de NF-e de venda via nova API,
> com estoque baixado e financeiro gerado.

---

### FASE 4 — Financeiro

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 4.1 | Gestão Financeira (caixa/banco + domínio) | MOD-05 §2.4 + §2.5 | 1–2 semanas | FASE 3 |
| 4.2 | Gestão Financeira (documento financeiro + parcela) | MOD-05 §2.1 | 2–3 semanas | 4.1 |
| 4.3 | Gestão Financeira (liquidação + formas de pagamento) | MOD-05 §2.2 + §2.8 | 2 semanas | 4.2 |
| 4.4 | Gestão Financeira (cheques) | MOD-05 §2.7 | 1–2 semanas | 4.2 |
| 4.5 | Gestão Financeira (reversão + estorno) | MOD-05 §2.9 | 1–2 semanas | 4.3 |
| 4.6 | Gestão Financeira (DRE + projeções) | MOD-05 §2.10 | 1–2 semanas | 4.5 |

> **Marco da Fase 4:** Ciclo financeiro completo funcionando: geração, liquidação,
> estorno de títulos via nova API.

---

### FASE 5 — Módulos de Gestão (podem ser em paralelo)

| Ordem | Módulo | SPEC | Duração estimada | Bloqueio |
|---|---|---|---|---|
| 5.1 | Gestão Tributária completa (SPED) | MOD-07 §2.4 | 3–4 semanas | FASE 4 |
| 5.2 | Gestão RH | MOD-09 | 3–4 semanas | FASE 4 |
| 5.3 | Gestão Contratos | MOD-10 | 2–3 semanas | FASE 4 |
| 5.4 | Gestão OS | MOD-11 | 2–3 semanas | FASE 3 |
| 5.5 | Gestão Produção | MOD-12 | 2–3 semanas | FASE 2 |
| 5.6+ | Módulos Secundários | MOD-13 | variado | variado |

---

## Regras de Avanço de Fase

Antes de iniciar uma nova fase, verificar:

- [ ] **100% dos checklists** da fase anterior concluídos
- [ ] **Testes de integração** entre módulos da fase passando
- [ ] **Testes de paridade** com o legado executados para os módulos críticos
- [ ] **Documentação de rollback** atualizada (como voltar ao legado se necessário)
- [ ] **Review de SPEC** — a SPEC da próxima fase está completa e aprovada?

---

## Riscos e Contingências

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| `DocumentoVenda.cs` (584 KB) muito complexo | ALTA | ALTO | Dividir em sessões menores; analisar antes de implementar |
| `NFEGen.cs` (253 KB) depende de schemas SEFAZ | ALTA | ALTO | Usar biblioteca externa especializada em NF-e |
| GeradorSequencial produz IDs errados | BAIXA | CRÍTICO | Testes de paridade obrigatórios antes de ir para produção |
| Regras de tributo mudaram desde o legado | MÉDIA | ALTO | Revisar legislação vigente vs. código legado |
| Banco de dados com dados inconsistentes | MÉDIA | MÉDIO | Executar scripts de diagnóstico antes da migração |

---

## Duração Total Estimada

| Fase | Mínimo | Máximo |
|---|---|---|
| Fase 0 | 1 semana | 2 semanas |
| Fase 1 | 6 semanas | 9 semanas |
| Fase 2 | 8 semanas | 13 semanas |
| Fase 3 | 10 semanas | 15 semanas |
| Fase 4 | 7 semanas | 11 semanas |
| Fase 5 | 12 semanas | 20 semanas |
| **TOTAL** | **~44 semanas** | **~70 semanas** |

> ⚠️ Estimativas com equipe de 1-2 desenvolvedores.
> Escalar a equipe reduz o tempo linear em algumas fases mas não em todas
> (há dependências sequenciais que não paralelizam).

---

*Decisão: 2026-04-27*
