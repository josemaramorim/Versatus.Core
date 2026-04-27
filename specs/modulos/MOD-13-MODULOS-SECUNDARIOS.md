# MOD-13 — Módulos Secundários

> **Fase:** 5 | **Prioridade:** BAIXA

## Módulos incluídos

| Módulo | Localização | Arquivos | Descrição |
|---|---|---|---|
| Gestão Armazém | `Gestao.Armazem/` | — | Gestão de armazém/WMS |
| Gestão Contábil | `Gestao.Contabil/` | — | Integração contábil |
| Gestão Frota | `Gestao.Frota/` | — | Controle de frota |
| Gestão Garagem | `Gestao.Garagem/` | — | Controle de garagem |
| Gestão Locação | `Gestao.Locacao/` | — | Locação de equipamentos |
| Gestão Obra | `Gestao.Obra/` | — | Gestão de obras |
| Gestão Transporte | `Gestao.Transporte/` | — | Romaneios e transportes |
| Gestão Mobile | `Gestao.Mobile/` | — | App mobile de vendas |
| Gestão Pdv | `Gestao.Pdv/` | — | PDV/Frente de caixa |
| Gestão Pesagem | `Gestao.Pesagem/` | — | Pesagem integrada |
| NFSe | `NFSe/` | — | NFS-e (nota de serviço) |
| MDFe | `MDFe/` | — | MDF-e (manifesto de carga) |
| SPED Fiscal | `SPED.Fiscal/` | — | SPED Fiscal |
| SPED PisCofins | `SPED.PisCofins/` | — | SPED PIS/COFINS |
| Integração Ecommerce | `Gestao.Integracao/` | — | Integração e-commerce |

> **Nota:** Cada um destes submódulos precisará de sua própria SPEC detalhada quando chegar o momento de migrá-los. Criar `MOD-13a`, `MOD-13b`, etc.

## Roteiro de Tarefas — MOD-13 (Módulos Secundários)

Este roteiro descreve a abordagem de alto nível para MOD-13. Como são módulos de prioridade baixa, o foco é primeiro mapear cada submódulo e só então elaborar SPECs específicas.

### Fase 1: Classificação e priorização

**Tarefa 1.1 — Mapear e validar o escopo de cada submódulo**
- Revise o legado para identificar os arquivos, funcionalidades e integrações de cada submódulo listado
- Documente dependências comuns (por exemplo, `Gestao.Contabil` com `Gestao.Faturamento` e `Gestao.Transporte` com `Gestao.Estoque`)
- Branch: `analysis/mod-13-scope`
- Commit: `docs: Map MOD-13 secondary modules scope`

**Tarefa 1.2 — Priorizar submódulos por valor e dependência**
- Defina uma ordem de migração baseada em valor de negócio, risco e dependências técnicas
- Identifique submódulos que podem ser adiados ou integrados após a fase 5
- Branch: `analysis/mod-13-prioritization`
- Commit: `docs: Prioritize MOD-13 secondary modules`

### Fase 2: Preparar SPECs individuais

**Tarefa 2.1 — Criar SPECs separadas por submódulo**
- Para cada submódulo relevante, crie arquivos como `MOD-13a-GESTAO-ARMAZEM.md`, `MOD-13b-GESTAO-CONTABIL.md`, etc.
- Inclua visão geral, inventário, regras críticas, checklist e roteiro de tarefas
- Branch: `spec/mod-13-submodules`
- Commit: `docs: Create MOD-13 submodule SPEC templates`

**Tarefa 2.2 — Definir arquitetura comum de suporte**
- Desenhe padrões de projeto para submódulos secundários: pastas `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Estabeleça regras gerais de convenções de nomeação e uso de `Result<T>`/`ValidationResult`
- Branch: `spec/mod-13-architecture`
- Commit: `docs: Define architecture guidelines for secondary modules`

### Fase 3: Implementação gradual de submódulos

**Tarefa 3.1 — Migrar submódulos de maior prioridade**
- Comece pelos submódulos com maior dependência e impacto: por exemplo, `Gestao.Armazem`, `Gestao.Contabil`, `Gestao.Transporte`
- Implemente um submódulo de cada vez, criando um branch por submódulo
- Branch: `feat/mod-13-first-submodule`
- Commit: `feat: Migrate first MOD-13 secondary module`

**Tarefa 3.2 — Validar integrações e infraestrutura**
- Teste que cada submódulo se comporta como esperado com os módulos principais existentes (estoque, faturamento, transporte)
- Valide integrações transversais e evite duplicação de regras de negócio
- Branch: `feat/mod-13-integration-tests`
- Commit: `test: Add integration tests for MOD-13 submodules`

### Fase 4: Revisão de riscos e adiamento

**Tarefa 4.1 — Revisar prioridade ao final da fase 5**
- Reavalie os submódulos secundários após concluir as migrações de maior prioridade
- Determine quais módulos devem avançar, quais devem ser adiados e quais podem ser descontinuados
- Branch: `chore/mod-13-review`
- Commit: `chore: Review MOD-13 secondary modules priority`

**Tarefa 4.2 — Ajustar roadmap conforme evolução do produto**
- Atualize a SPEC de MOD-13 com base em novas informações de dependência, risco e valor
- Crie branches de entrega específicos para cada submódulo aprovado
- Branch: `chore/mod-13-roadmap-update`
- Commit: `chore: Update MOD-13 roadmap`

### Conclusão
- MOD-13 deve ser tratado como um conjunto de projetos à parte: mapear, priorizar e documentar antes de desenvolver.
- Branch final de entrega: `release/mod-13-modulos-secundarios`
- Commit final: `chore: Complete MOD-13 Módulos Secundários roadmap`
