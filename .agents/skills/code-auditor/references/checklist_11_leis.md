# Checklist Interativo das 11 Leis do AGENTS.md

Use este checklist para inspecionar os arquivos antes de aprovar qualquer PR ou merge:

1. **[ ] SOLID, Clean Architecture & .NET 10:** Estrutura desacoplada em Domain, Infrastructure, Api.
2. **[ ] Pureza de Domínio (POCOs):** Sem atributos `[Table]`, `[Column]` ou DataAnnotations em `Domain/Entities/`.
3. **[ ] Abordagem Spec-First:** Implementação 100% fiel a `docs/spec_f[nome].md`.
4. **[ ] Cobertura de UI:** Nenhuma propriedade editável do DTO ficou de fora do formulário React.
5. **[ ] React OOP:** Componentes herdando `BaseCadastroConfig<T>`, organizados por módulo (`src/pages/[Modulo]/F[Nome]/`).
6. **[ ] Estratégia Git:** Desenvolvimento em `feat/migrate-[nome]`, pergunta de limpeza após merge na `develop`.
7. **[ ] Builds Limpos:** `dotnet build` e `npm run build` executando com 0 erros.
8. **[ ] Classificação de Tela:** Respeita Padrão A (CRUD) ou Padrão B (Lote com botões ocultados).
9. **[ ] Paginação SQL Server 2008:** Paginação em memória com `ToListAsync()` antes de `.Skip().Take()`.
10. **[ ] Result Pattern:** Sem `throw Exception()`, retornos funcionais `Result<T>` com `ValidationError`.
11. **[ ] MUI required & CQRS Split:** Asterisco vermelho nos campos obrigatórios e repositório com `ReadContext` / `WriteContext`.
