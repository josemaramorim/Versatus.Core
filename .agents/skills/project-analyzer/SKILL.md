---
name: project-analyzer
description: Use quando precisar fazer uma análise profunda e estruturada de qualquer artefato do projeto — código C#, componente React, arquitetura, documento, processo de negócio, fluxo de dados, regra fiscal, performance, segurança ou qualquer outro tópico — gerando um relatório de diagnóstico com descobertas, riscos e recomendações acionáveis.
---

# Skill: project-analyzer

Esta skill entrega análises estruturadas, imparciais e contextualizadas sobre qualquer aspecto do projeto Versatus ERP. Ela é **genérica por design**: o alvo da análise é definido pelo usuário no momento do disparo.

---

## 1. Como Esta Skill Funciona

O fluxo da análise sempre segue 5 etapas, independentemente do alvo:

```
ETAPA 1: DEFINIR ESCOPO
   ↳ Identificar exatamente o que será analisado e qual é o objetivo da análise

ETAPA 2: COLETAR EVIDÊNCIAS
   ↳ Ler todos os arquivos, specs, regras e contexto relevantes

ETAPA 3: DIAGNOSTICAR
   ↳ Identificar pontos fortes, riscos, lacunas e inconsistências

ETAPA 4: RECOMENDAR
   ↳ Propor melhorias concretas, priorizadas por impacto

ETAPA 5: DOCUMENTAR
   ↳ Produzir o Relatório de Análise no formato padrão
```

> [!IMPORTANT]
> **Nenhum arquivo de código ou configuração deve ser alterado durante a análise.**  
> Esta skill é puramente investigativa e descritiva. Mudanças só ocorrem se o usuário solicitar explicitamente após receber o relatório.

---

## 2. Tipos de Análise Suportados

| Categoria | Exemplos de Pedido |
|---|---|
| **Código C#** | Serviço, Entidade, Repositório, Controller, DbContext |
| **Componente React** | `index.tsx`, `schema.ts`, `CadastroConfig.tsx`, hooks |
| **Arquitetura** | Camadas, dependências entre projetos, CQRS, DI |
| **Processo de Negócio** | Fluxo de liquidação, processo de rateio, conciliação bancária |
| **Regras de Negócio / Spec** | Spec Funcional, Matriz RTV, regras fiscais |
| **Segurança** | Exposição de endpoints, validação de input, autenticação |
| **Performance** | Queries lentas, paginação, carregamento de dados |
| **Cobertura de Testes** | Cobertura Backend xUnit, Frontend Vitest/Zod |
| **Documentação** | Specs, AGENTS.md, Manual de Skills, READMEs |
| **Inovação / UX** | Melhorias de experiência, automação, aplicação de IA |

---

## 3. Protocolo de Coleta de Evidências

Antes de diagnosticar, a IA DEVE coletar o contexto completo do alvo:

### 3.1 Análise de Código (C# ou React)
1. Ler o arquivo principal do alvo.
2. Ler todas as dependências diretas (interfaces, DTOs, schemas Zod, specs referenciadas).
3. Verificar a cobertura de testes existente (`.Tests/` para C# e `schema.test.ts` para React).
4. Consultar `AGENTS.md` para verificar conformidade com as regras do projeto.

### 3.2 Análise de Processo / Regra de Negócio
1. Ler a Spec Funcional (`docs/spec_f[nome].md`) do processo.
2. Verificar o código C# do serviço de domínio correspondente.
3. Verificar se o schema Zod e a tela React refletem fielmente a regra.
4. Cruzar com arquivos legados se mencionados.

### 3.3 Análise de Arquitetura / Módulo
1. Inspecionar a estrutura de pastas do módulo (`list_dir`).
2. Verificar separação de camadas (Domain, Infrastructure, API).
3. Verificar configuração de DbContexts (Read/Write CQRS).
4. Verificar injeção de dependências (`DependencyInjection/`).

### 3.4 Análise de Inovação / UX
1. Entender o fluxo de trabalho atual do usuário ou gestor.
2. Identificar gargalos, fricções e erros mais comuns.
3. Propor melhorias de UX, automação e aplicação de IA alinhadas ao contexto do ERP.
4. Priorizar por facilidade de implementação vs. impacto no negócio.

---

## 4. Formato do Relatório de Análise

A skill SEMPRE produz um relatório no seguinte formato padrão:

```markdown
# Relatório de Análise: [Alvo da Análise]

**Data:** [Data da análise]
**Tipo de Análise:** [Código / Processo / Arquitetura / UX / etc.]
**Escopo:** [Arquivo(s), módulo(s) ou tópico(s) analisados]

---

## 📊 Resumo Executivo

[2 a 4 linhas resumindo o estado geral e os achados mais críticos]

**Status Geral:** [SAUDÁVEL ✅ / ATENÇÃO ⚠️ / CRÍTICO ❌]

---

## ✅ Pontos Fortes (O que está bem)

- [Item 1]
- [Item 2]

---

## ⚠️ Riscos e Lacunas (O que precisa de atenção)

| # | Severidade | Descrição | Localização |
|---|---|---|---|
| 1 | 🔴 Alta | [Descrição do risco ou lacuna] | [Arquivo/Módulo] |
| 2 | 🟡 Média | [Descrição] | [Arquivo/Módulo] |
| 3 | 🟢 Baixa | [Descrição] | [Arquivo/Módulo] |

---

## 💡 Recomendações Priorizadas

### Prioridade 1 — Alta (Fazer Agora)
1. **[Nome da Ação]:** [Descrição concreta do que fazer e por quê]

### Prioridade 2 — Média (Fazer no Próximo Ciclo)
1. **[Nome da Ação]:** [Descrição]

### Prioridade 3 — Baixa (Melhoria Contínua)
1. **[Nome da Ação]:** [Descrição]

---

## 📋 Próximos Passos

[Lista de ações para que o usuário decida o que executar após o relatório]
```

---

## 5. Regras de Comportamento

1. **Nunca alterar código durante a análise.** Se detectar um problema crítico, registrar no relatório e aguardar aprovação do usuário para criar uma branch de correção.
2. **Referenciar sempre os arquivos lidos** com links clicáveis `[arquivo.cs](file:///...)`.
3. **Severidade baseada no impacto:** Alta = afeta produção, segurança ou dados. Média = afeta qualidade ou experiência. Baixa = melhoria desejável.
4. **Não inventar problemas.** Toda descoberta deve ter evidência no código ou documento lido.
5. **Salvar o relatório em `docs/`** se o usuário solicitar ou se o relatório for extenso (>50 linhas).

---

## 6. Exemplos de Disparo

```
Usar a skill project-analyzer para analisar o EntidadeService.cs e identificar riscos de performance e violações de AGENTS.md.
```

```
Usar a skill project-analyzer para analisar o processo de liquidação de Contas a Pagar e sugerir melhorias de UX para o operador financeiro.
```

```
Usar a skill project-analyzer para fazer uma análise de arquitetura do módulo Versatus.AcessoGlobal.
```

```
Usar a skill project-analyzer para analisar a cobertura de testes do FCondicaoPagamento e apontar cenários não cobertos.
```

```
Usar a skill project-analyzer para analisar o schema.ts da FEntidade e verificar se todas as regras da spec estão refletidas no Zod.
```
