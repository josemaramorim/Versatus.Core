---
name: code-auditor
description: Use ao concluir a migração de um formulário para auditar se o código C# e React cumpre as 12 leis do AGENTS.md.
---

# Skill: code-auditor

Esta skill realiza uma auditoria completa de conformidade arquitetural e de qualidade sobre qualquer formulário ou módulo migrado no projeto Versatus.Net8.

---

## 1. Escopo da Auditoria

Dado um formulário `F[Nome]` ou Módulo `[Modulo]`, o auditor inspeciona os seguintes arquivos:
1. **Backend C#:** Entidade POCO, DTOs, Fluent API Mapping, Service (`I[Nome]Service`), Repository (`I[Nome]Repository` / `[Nome]Repository`) e Controller (`[Nome]Controller`).
2. **Frontend React:** `types.ts`, `schema.ts`, `[Nome]CadastroConfig.tsx` e `index.tsx`.
3. **Spec Funcional:** `docs/spec_f[nome].md`.

---

## 2. Checklist das 11 Leis do AGENTS.md

 Execute a verificação item por item e classifique como **[CONFORME ✅]**, **[AVISO ⚠️]** ou **[VIOLAÇÃO ❌]**:

### 1. SOLID, Clean Architecture & .NET 10
- [ ] **Camadas Isoladas:** Domain POCO puras, Infrastructure EF Core Mappings, Services via DI, Controllers limpos.
- [ ] **Dependency Inversion (DIP):** Injeção de dependência via interfaces (`I[Nome]Service`, `I[Nome]Repository`) em todos os componentes.
- [ ] **C# Moderno:** File-scoped namespaces e Nullable Reference Types habilitados.

### 2. Pureza de Domínio (POCOs Puras)
- [ ] **Sem DataAnnotations:** Nenhuma classe em `Domain/Entities/` importa `System.ComponentModel.DataAnnotations` nem usa atributos `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]`.
- [ ] **Fluent API Exclusivo:** Todo o mapeamento de tabelas e colunas está configurado em `Infrastructure/Mappings/`.

### 3. Abordagem Spec-First & Fidelidade de Nomes
- [ ] **Spec Aprovada:** Código C# e React condizem 100% com `docs/spec_f[nome].md`.
- [ ] **Nomenclatura Sagrada:** Nomes de tabelas e colunas fiéis ao legado (ex: `GloEntidade`).

### 4. Auditoria de Propriedades vs. UI
- [ ] **Cobertura 100% da UI:** Todas as propriedades editáveis da entidade possuem componentes de entrada (TextField, Switch, Select, Checkbox) na tela JSX (`index.tsx`).

### 5. Arquitetura Frontend (React OOP)
- [ ] **BaseCadastroConfig<T>:** O arquivo `[Nome]CadastroConfig.tsx` herda `BaseCadastroConfig<T>`.
- [ ] **Mapeamentos mapBackendToForm / mapFormToBackend:** Sobrescritos quando há conversões de enums ou dados.
- [ ] **Sem Hardcode em Hooks Globais:** Hooks reutilizáveis (como `useCrudListState.ts`) não contêm URLs ou enums hardcoded específicos da tela.
- [ ] **Organização por Módulo:** Pasta criada em `src/pages/[Modulo]/F[Nome]/`.

### 6. Proteção de Branches e Git
- [ ] **Isolamento de Branch:** Código desenvolvido em `feat/migrate-[nome]`.
- [ ] **Limpeza de Branch:** Pergunta de exclusão realizada após merge na `develop`.

### 7. Validação com Builds
- [ ] `dotnet build` executa sem erros de compilação.
- [ ] `npm run build` executa sem erros de tipagem TypeScript ou Vite.

### 8. Padrão de Tela (CRUD vs Lote)
- [ ] **Padrão A (CRUD):** Grid paginado, botões Novo/Editar/Excluir visíveis.
- [ ] **Padrão B (Lote):** Accordions agrupados, botões Novo e Excluir **100% ocultados**.

### 9. Paginação SQL Server 2008
- [ ] **Paginação em Memória:** Uso obrigatório de `await query.ToListAsync()` primeiro e `.Skip(offset).Take(limit)` em memória. Proibido `.Skip().Take()` direto no `IQueryable`.

### 10. Result Pattern & Clean Code
- [ ] **Sem Throw exception:** Proibido `throw new ...Exception(...)` para erros de negócio.
- [ ] **Retorno Funcional:** Uso de `Result<T>` retornando `400 BadRequest` com `ValidationError`.

### 11. Sinalização Visual MUI (`required`), Floating Labels e CQRS DB Split
- [ ] **MUI required:** Prop `required` nos componentes MUI para todos os campos obrigatórios.
- [ ] **MUI Floating Label (variant="outlined"):** Todos os campos usam `variant="outlined"` — label flutua sobre a borda superior do campo. Proibido label externo (acima/fora do campo) ou usar apenas `placeholder`. Verificar estados: cinza (normal), azul (focado), vermelho + helperText (erro).
- [ ] **CQRS DB Split:** Repositório usa `ReadContext` (`ReadConnection` com `NoTracking`) para consultas e `Context` (`WriteConnection`) para gravações.

### 12. Confirmação de Alterações Não Salvas (Descarte e Fechamento)
- [ ] **Banner Inline de Cancelamento:** Botão Cancelar ou Voltar exibe banner inline amarelo ("Alterações não salvas" / "Você possui alterações não salvas no formulário. Deseja realmente cancelar e descartar as alterações?") com botões `[Descartar e Sair]` e `[Continuar Editando]`.
- [ ] **Fechamento de Aba [X]:** Fechamento de aba suja exibe a mensagem padronizada ("Você possui alterações não salvas na aba \"[Nome da Aba]\". Deseja realmente fechar e descartar as alterações?").

---

## 3. Relatório de Auditoria Gerado

A skill deve produzir um relatório final resumido no formato:

```markdown
# Relatório de Auditoria de Código — F[Nome]

- **Data da Auditoria:** [Data]
- **Status Geral:** [APROVADO ✅ / AJUSTES NECESSÁRIOS ⚠️]

### 📊 Placar de Conformidade:
- **Itens Conformes:** X / 11
- **Avisos / Melhorias:** Y
- **Violações Críticas:** Z

### 📋 Detalhamento das Desconformidades (se houver):
1. **[Regra X - Nome da Regra]:** Detalhe da não conformidade encontrada no arquivo `file:///c:/...` e como corrigir.

### 💡 Ação Recomendada:
[Aprovar para merge / Corrigir itens apontados antes de prosseguir]
```
