# Guia e Prompt Padrão: Auditoria de Cobertura de Código 100% (Full-Stack)

Este documento define o **Prompt Padrão de Disparo** e o procedimento automatizado para auditar e elevar para **100% a cobertura de testes unitários** em qualquer momento do projeto.

---

## 📋 Prompt Padrão de Disparo (Copie e Cole no Chat)

Sempre que desejar realizar uma auditoria completa de cobertura de testes ou preencher lacunas de testes no projeto, basta enviar o prompt abaixo para a IA:

```text
Executar a auditoria completa de cobertura de código no projeto (Backend C# + Frontend React).
Analise o relatório de cobertura de todas as classes de serviço (.NET 10) e schemas Zod (React), identifique qualquer linha ou condição sem teste e crie os testes unitários faltantes até atingir 100% de cobertura de código em ambas as pontas.
```

---

## 🛠️ Procedimento Automatizado que a IA Executará

Ao receber o prompt acima, a IA seguirá rigorosamente este fluxo de 4 passos:

### Passo 1: Executar Medição no Backend C#
1. Encerra qualquer processo em segundo plano que esteja travando DLLs.
2. Executa `dotnet test --collect:"XPlat Code Coverage"`.
3. Inspeciona a lista de métodos e linhas não cobertas em `tests/[Modulo].Tests/`.

### Passo 2: Executar Medição no Frontend React
1. Executa `npm test -- --coverage` dentro de `src/Versatus.Frontend/`.
2. Inspeciona o relatório do Vitest identificando arquivos `schema.ts` com cobertura `< 100%`.

### Passo 3: Geração Automatizada dos Testes Faltantes
1. **No Backend:** Para cada `if / else / switch` não exercitado em `Domain/Services/[Nome]Service.cs`, gera o método `[Fact]` correspondente em `[Nome]ServiceTests.cs`.
2. **No Frontend:** Para cada regra ou refinamento Zod não exercitado em `schema.ts`, gera o teste `it(...)` em `schema.test.ts`.

### Passo 4: Validação e Confirmação de 100%
1. Re-executa `dotnet test` e `npm test` até confirmar **100% de aprovação e cobertura completa**.
2. Realiza o commit dos testes adicionados na branch de desenvolvimento.
