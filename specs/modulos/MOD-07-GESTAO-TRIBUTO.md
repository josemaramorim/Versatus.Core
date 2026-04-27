# SPEC — MOD-07: Gestão de Tributos
## Módulo: servidor/objeto de negócio/gestao.tributo

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 1  
> **Status:** 📝 Rascunho  
> **Prioridade:** ALTA — base fiscal de todos os documentos

---

## 1. Visão Geral do Módulo

O módulo de **Gestão de Tributos** contém toda a legislação fiscal parametrizada:
NCM, CFOP, ICMS, IPI, PIS, COFINS, SPED, Inventário, e configurações de regimes tributários.
É a base que alimenta o cálculo de impostos em faturamento e compras.

### Localização no legado
```
servidor/objeto de negócio/gestao.tributo/
```

### Dependências
- **Requer:** MOD-01, MOD-02
- **É usado por:** MOD-03, MOD-04, MOD-06, MOD-08 (NFe)

---

## 2. Inventário de Classes por Grupo

### 2.1 Classificação Fiscal (NCM/CFOP)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `ClassificacaoFiscal.cs` | 22 KB | **Entidade Central** | NCM (nomenclatura comum do Mercosul) |
| `ClassificacaoFiscalTributo.cs` | 11 KB | Relacionamento | Tributos por NCM |
| `ClassificacaoFiscalTributoFilial.cs` | 6 KB | Relacionamento | Tributos por NCM e filial |
| `ClassificacaoFiscalTributoLista.cs` | 1 KB | Lista | |
| `Cfop.cs` | 15 KB | Entidade | CFOP (código fiscal de operações) |
| `Cest.cs` | 15 KB | Entidade | CEST (substituição tributária) |
| `CestNCM.cs` | 9 KB | Relacionamento | CEST × NCM |
| `CestSegmento.cs` | 4 KB | Entidade | Segmento do CEST |
| `UnidadeFiscal.cs` | 12 KB | Entidade | Unidade fiscal |
| `OrigemMercadoria.cs` | 9 KB | Entidade | Origem da mercadoria |
| `SituacaoTributaria.cs` | 22 KB | Entidade | CST/CSOSN |
| `SituacaoTributariaFiscal.cs` | 7 KB | Relacionamento | Situação tributária fiscal |

### 2.2 Aplicação de Tributos (Regras)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `AplicacaoTributo.cs` | **36 KB** | Entidade | Motor de aplicação de tributos |
| `AplicacaoTributoLista.cs` | 2 KB | Lista | |
| `AplicacaoProduto.cs` | 22 KB | Entidade | Aplicação por produto |
| `AplicacaoProdutoTributo.cs` | 11 KB | Relacionamento | Tributação do produto |
| `AplicacaoNaturezaOperacao.cs` | 10 KB | Entidade | Aplicação por natureza de operação |
| `AplicacaoEspecial.cs` | 12 KB | Entidade | Regras especiais de tributação |
| `RegraTributo.cs` | 14 KB | Entidade | Regra de tributação |
| `RegraTributoConfiguracao.cs` | **61 KB** | Configuração | Config das regras tributárias (GRANDE) |
| `RegraTributoConfiguracaoLista.cs` | 6 KB | Lista | |
| `RegraTributacaoEspecial.cs` | 11 KB | Entidade | Regras especiais |

### 2.3 ICMS e Substituição Tributária

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `GrupoTributarioICMS.cs` | 10 KB | Entidade | Grupo tributário de ICMS |
| `GrupoTributarioInventarioICMS.cs` | 8 KB | Entidade | Grupo para inventário |
| `TributoIcmsSubstituicaoEstoque.cs` | 24 KB | Entidade | ST por produto (MVA) |
| `DetalheUfTributacao.cs` | 9 KB | Entidade | Detalhe por UF |
| `DetalheCidadeTributacao.cs` | 9 KB | Entidade | Detalhe por cidade (ISS) |
| `PartilhaICMSVigencia.cs` | 4 KB | Entidade | Partilha ICMS DIFAL |
| `RegimeTributarioVigencia.cs` | 12 KB | Entidade | Vigência do regime tributário |
| `SimplesNacional.cs` | 11 KB | Entidade | Tabelas do Simples Nacional |
| `SimplesNacionalTributo.cs` | 10 KB | Relacionamento | Tributos do Simples |
| `RestituicaoICMSST.cs` | 3 KB | Entidade | Restituição de ICMS-ST |

### 2.4 SPED Fiscal

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `EFD.cs` | **28 KB** | Entidade | EFD-ICMS/IPI |
| `EFDPisCofins.cs` | 24 KB | Entidade | EFD-PIS/COFINS |
| `EFDConsulta.cs` | 5 KB | Consulta | Consulta de EFD |
| `EFDMotivoSemEscriturar.cs` | 3 KB | Entidade | Motivo sem escrituração |
| `EFDSpedVigenciaVersao.cs` | 4 KB | Entidade | Versão vigente do SPED |
| `ECF.cs` | 17 KB | Entidade | ECF (antiga impressora fiscal) |
| `TributoAjuste.cs` | 18 KB | Entidade | Ajustes de apuração |
| `TributoAjusteAvulso.cs` | 10 KB | Entidade | Ajustes avulsos |

### 2.5 Nota Fiscal e Sintegra

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `NotaFiscal.cs` | Entidade | NF (modelo 1/1A) (25 KB) |
| `NaturezaOperacao.cs` | Entidade | Natureza de operação (18 KB) |
| `NaturezaOperacaoFilial.cs` | Relacionamento | Natureza por filial |
| `LancamentoFiscalDocumento.cs` | Entidade | Lançamento fiscal (30 KB) |
| `Sintegra.cs` | Entidade | Arquivo Sintegra (40 KB) |
| `ObservacaoFiscal.cs` | Entidade | Observação fiscal (16 KB) |
| `DocumentoArrecadacao.cs` | Entidade | Documento de arrecadação (43 KB) |

### 2.6 Inventário

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Inventario.cs` | Entidade | Inventário fiscal (44 KB) |
| `InventarioItem.cs` | Relacionamento | Item do inventário (27 KB) |
| `InventarioItemComplemento.cs` | Relacionamento | Complemento do item |
| `InventarioVigencia.cs` | Entidade | Vigência do inventário (14 KB) |

### 2.7 Fórmulas e Benefícios

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `TributoFormula.cs` | Entidade | Fórmula de tributação (22 KB) |
| `TributoFormulaCondicao.cs` | Relacionamento | Condição da fórmula |
| `BeneficioFiscal.cs` | Entidade | Benefício fiscal (15 KB) |
| `BeneficioFiscalDetalhe.cs` | Relacionamento | Detalhe do benefício |

---

## 3. Regras de Negócio Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-07-001 — Tributação multicamada
A tributação é calculada em cascata: NCM define base → Regra Tributária aplica alíquotas
→ Situação Tributária define CST → Benefícios fiscais podem reduzir.
**Nunca simplificar — a ordem importa**.

### RN-07-002 — Vigências
Tabelas fiscais têm vigência (data início / data fim). Sempre consultar pela data do documento.

### RN-07-003 — Regime tributário muda as regras
Simples Nacional, Lucro Presumido e Lucro Real têm tributações completamente diferentes
para o mesmo produto. `RegimeTributarioVigencia` determina qual regra usar.

### RN-07-004 — SPED é crítico
O SPED é a escrituração oficial entregue ao governo. Erros geram multas.
`EFD.cs` e `EFDPisCofins.cs` devem ser migrados com paridade absoluta.

### RN-07-005 — NCM-CEST para ST
A junção NCM + UF de destino determina se há Substituição Tributária.
`TributoIcmsSubstituicaoEstoque` contém as MVAs por produto/UF.

---

## 4. Checklist de Conclusão do Módulo

- [ ] ClassificacaoFiscal (NCM) implementada
- [ ] CFOP implementado
- [ ] SituacaoTributaria (CST/CSOSN) implementada
- [ ] NaturezaOperacao implementada
- [ ] AplicacaoTributo analisada e implementada
- [ ] RegraTributoConfiguracao analisada (61 KB)
- [ ] ICMS-ST implementado
- [ ] Simples Nacional implementado
- [ ] EFD-ICMS/IPI implementado com paridade
- [ ] EFD-PIS/COFINS implementado com paridade
- [ ] Inventário implementado
- [ ] Benefícios fiscais implementados

---

## 5. Roteiro de Tarefas (Ordem de Execução)

Este roteiro descreve a ordem de implementação de MOD-07. Cada fase deve terminar com um commit Git e uma revisão de SPEC. O módulo é base de todos os cálculos fiscais do sistema, por isso as regras de vigência e de regime tributário devem ser tratadas com prioridade.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.GestaoTributo**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.GestaoTributo`
- Adicione referências a `Versatus.Framework` e `Versatus.AcessoGlobal`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/gestao-tributo-project`
- Commit: `setup: Create Versatus.GestaoTributo project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Classificacao/`, `Aplicacao/`, `ICMS/`, `SPED/`, `NotaFiscal/`, `Inventario/`, `Beneficios/`
- Branch: `setup/gestao-tributo-structure`
- Commit: `setup: Create GestaoTributo folder structure`

**Tarefa 1.3 — Criar DbContext base**
- Crie `Infrastructure/TributoDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/gestao-tributo-dbcontext-base`
- Commit: `setup: Create TributoDbContext base`

### Fase 2: Classificação Fiscal e CFOP

**Tarefa 2.1 — Implementar ClassificacaoFiscal e NCM**
- Crie `Domain/Classificacao/ClassificacaoFiscal.cs`, `ClassificacaoFiscalTributo.cs`, `ClassificacaoFiscalTributoFilial.cs`
- Garanta suporte a NCM e tributos por filial
- Branch: `feat/ncm-entity`
- Commit: `feat: Implement NCM and fiscal classification entities`

**Tarefa 2.2 — Implementar CFOP e CEST**
- Crie `Domain/Classificacao/Cfop.cs`, `Cest.cs`, `CestNCM.cs`, `CestSegmento.cs`
- Branch: `feat/cfop-cest-entities`
- Commit: `feat: Implement CFOP and CEST entities`

**Tarefa 2.3 — Configurar classificação no DbContext**
- Atualize `TributoDbContext` com os DbSet de classificação fiscal e CFOP
- Branch: `feat/classificacao-dbcontext`
- Commit: `feat: Configure fiscal classification in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Aplicação de Tributos e Regras

**Tarefa 3.1 — Implementar AplicacaoTributo e regras de produto**
- Crie `Domain/Aplicacao/AplicacaoTributo.cs`, `AplicacaoProduto.cs`, `AplicacaoProdutoTributo.cs`, `AplicacaoNaturezaOperacao.cs`, `AplicacaoEspecial.cs`
- Branch: `feat/aplicacao-tributo-entities`
- Commit: `feat: Implement tax application entities`

**Tarefa 3.2 — Implementar RegraTributo e configurações**
- Crie `Domain/Aplicacao/RegraTributo.cs`, `RegraTributoConfiguracao.cs`, `RegraTributoConfiguracaoLista.cs`, `RegraTributacaoEspecial.cs`
- Analise o arquivo legado `RegraTributoConfiguracao.cs` (61 KB) com atenção à paridade
- Branch: `feat/regra-tributo-entities`
- Commit: `feat: Implement tax rule configuration entities`

**Tarefa 3.3 — Configurar aplicação de tributos no DbContext**
- Atualize `TributoDbContext` com os DbSet de aplicação de tributos e regras
- Branch: `feat/aplicacao-dbcontext`
- Commit: `feat: Configure tax application entities in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: ICMS e Substituição Tributária

**Tarefa 4.1 — Implementar grupos de ICMS**
- Crie `Domain/ICMS/GrupoTributarioICMS.cs`, `GrupoTributarioInventarioICMS.cs`, `TributoIcmsSubstituicaoEstoque.cs`, `DetalheUfTributacao.cs`, `DetalheCidadeTributacao.cs`
- Branch: `feat/icms-entity`
- Commit: `feat: Implement ICMS entities`

**Tarefa 4.2 — Implementar regimes e vigências**
- Crie `Domain/ICMS/RegimeTributarioVigencia.cs`, `SimplesNacional.cs`, `SimplesNacionalTributo.cs`, `PartilhaICMSVigencia.cs`
- Branch: `feat/regime-tributario-entities`
- Commit: `feat: Implement tax regime and validity entities`

**Tarefa 4.3 — Configurar ICMS no DbContext**
- Atualize `TributoDbContext` com os DbSet de ICMS e regimes
- Branch: `feat/icms-dbcontext`
- Commit: `feat: Configure ICMS entities in DbContext`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: SPED e Escrituração Fiscal

**Tarefa 5.1 — Implementar EFD-ICMS/IPI e PIS/COFINS**
- Crie `Domain/SPED/EFD.cs`, `EFDPisCofins.cs`, `EFDConsulta.cs`, `EFDMotivoSemEscriturar.cs`, `EFDSpedVigenciaVersao.cs`, `TributoAjuste.cs`, `TributoAjusteAvulso.cs`
- Branch: `feat/sped-entities`
- Commit: `feat: Implement SPED entities`

**Tarefa 5.2 — Testar paridade do SPED**
- Crie testes que comparem as saídas com o legado para EFD-ICMS/IPI e EFD-PIS/COFINS
- Branch: `test/sped-parity`
- Commit: `test: Add SPED parity tests`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Nota Fiscal, Sintegra e Natureza de Operação

**Tarefa 6.1 — Implementar Nota Fiscal e Natureza de Operação**
- Crie `Domain/NotaFiscal/NotaFiscal.cs`, `NaturezaOperacao.cs`, `NaturezaOperacaoFilial.cs`, `LancamentoFiscalDocumento.cs`, `DocumentoArrecadacao.cs`
- Branch: `feat/nota-fiscal-entities`
- Commit: `feat: Implement fiscal document entities`

**Tarefa 6.2 — Implementar Sintegra e Observações Fiscais**
- Crie `Domain/NotaFiscal/Sintegra.cs`, `ObservacaoFiscal.cs`
- Branch: `feat/sintegra-entities`
- Commit: `feat: Implement Sintegra entity`

**Tarefa 6.3 — Configurar nota fiscal no DbContext**
- Atualize `TributoDbContext` com os DbSet de nota fiscal e natureza de operação
- Branch: `feat/nota-fiscal-dbcontext`
- Commit: `feat: Configure fiscal document entities in DbContext`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Inventário Fiscal

**Tarefa 7.1 — Implementar Inventário**
- Crie `Domain/Inventario/Inventario.cs`, `InventarioItem.cs`, `InventarioItemComplemento.cs`, `InventarioVigencia.cs`
- Branch: `feat/inventario-entities`
- Commit: `feat: Implement fiscal inventory entities`

**Tarefa 7.2 — Configurar inventário no DbContext**
- Atualize `TributoDbContext` com os DbSet do inventário
- Branch: `feat/inventario-dbcontext`
- Commit: `feat: Configure inventory entities in DbContext`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Fórmulas e Benefícios Fiscais

**Tarefa 8.1 — Implementar Fórmulas de Tributação**
- Crie `Domain/Beneficios/TributoFormula.cs`, `TributoFormulaCondicao.cs`
- Branch: `feat/tributo-formula-entities`
- Commit: `feat: Implement tax formula entities`

**Tarefa 8.2 — Implementar Benefícios Fiscais**
- Crie `Domain/Beneficios/BeneficioFiscal.cs`, `BeneficioFiscalDetalhe.cs`
- Branch: `feat/beneficio-fiscal-entities`
- Commit: `feat: Implement fiscal benefit entities`

**Tarefa 8.3 — Configurar benefícios no DbContext**
- Atualize `TributoDbContext` com os DbSet de benefícios
- Branch: `feat/beneficios-dbcontext`
- Commit: `feat: Configure fiscal benefits in DbContext`
- Marque no checklist: ✅ Fase 8 completa

### Fase 9: Repositórios e Migrations

**Tarefa 9.1 — Criar repositórios chave**
- Crie `Infrastructure/Repositorios/IClassificacaoFiscalRepository.cs`, `IAplicacaoTributoRepository.cs`, `ITributacaoRepository.cs`
- Branch: `feat/tributo-repositories`
- Commit: `feat: Implement tributary repository interfaces`

**Tarefa 9.2 — Criar migrations EF Core**
- Crie a primeira migration para `TributoDbContext`
- Valide todas as tabelas e relacionamentos
- Branch: `setup/gestao-tributo-migrations`
- Commit: `setup: Create initial EF Core migrations for GestaoTributo`

**Tarefa 9.3 — Registrar DI**
- Crie método de extensão para registrar DbContext e repositórios
- Branch: `setup/gestao-tributo-di`
- Commit: `setup: Configure dependency injection for GestaoTributo`
- Marque no checklist: ✅ Fase 9 completa

### Fase 10: Testes de Paridade e Integração

**Tarefa 10.1 — Testar paridade fiscal**
- Crie testes para verificar NCM, ICMS-ST, SPED e regime tributário contra o legado
- Branch: `test/tributo-parity`
- Commit: `test: Add tax parity tests`

**Tarefa 10.2 — Testar integração com faturamento e compras**
- Adicione testes para garantir que MOD-04 e MOD-06 consumam corretamente as regras tributárias
- Branch: `test/tributo-integration`
- Commit: `test: Add tax integration tests`
- Marque no checklist: ✅ Fase 10 completa

---

## 6. Checklist de Conclusão do Módulo

- [ ] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: Classificação Fiscal e CFOP — Completa
- [ ] Fase 3: Aplicação de Tributos e Regras — Completa
- [ ] Fase 4: ICMS e Substituição Tributária — Completa
- [ ] Fase 5: SPED e Escrituração Fiscal — Completa
- [ ] Fase 6: Nota Fiscal, Sintegra e Natureza de Operação — Completa
- [ ] Fase 7: Inventário Fiscal — Completa
- [ ] Fase 8: Fórmulas e Benefícios Fiscais — Completa
- [ ] Fase 9: Repositórios e Migrations — Completa
- [ ] Fase 10: Testes de Paridade e Integração — Completa

---

## 7. Checklist de Conclusão do Módulo

- [ ] ClassificacaoFiscal (NCM) implementada
- [ ] CFOP implementado
- [ ] SituacaoTributaria (CST/CSOSN) implementada
- [ ] NaturezaOperacao implementada
- [ ] AplicacaoTributo analisada e implementada
- [ ] RegraTributoConfiguracao analisada (61 KB)
- [ ] ICMS-ST implementado
- [ ] Simples Nacional implementado
- [ ] EFD-ICMS/IPI implementado com paridade
- [ ] EFD-PIS/COFINS implementado com paridade
- [ ] Inventário implementado
- [ ] Benefícios fiscais implementados

---

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/gestao.tributo/` — 107 arquivos — `projeto_tag_1906`*
