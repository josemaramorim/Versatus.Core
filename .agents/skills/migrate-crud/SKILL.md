---
name: migrate-crud
description: Pipeline de migração automatizada de formulários legados (C#) para a arquitetura .NET 10 + React OOP.
---

# Skill: migrate-crud

Este skill guia o agente através do pipeline de migração de formulários do ERP legado para o novo sistema.

---

## 1. Ritual de Início (Fase 1: Analista de Esquema)

Quando o usuário solicitar a migração de um formulário fornecendo o nome e os arquivos `.cs` legados:
1. Localize a fase do formulário no arquivo `specs/00-INDICE-GERAL.md`.
2. Leia atentamente as Regras Anti-Alucinação em `specs/03-REGRAS-ANTI-ALUCINACAO.md`.
3. Leia a SPEC do módulo correspondente em `specs/modulos/MOD-0X-[MODULO].md`.
4. Analise os arquivos `.cs` legados recebidos.
5. Gere a Spec Funcional em `docs/spec_f[nome].md` seguindo rigorosamente o formato de `docs/spec_fentidade.md`.
6. Use o arquivo `references/FRONTEND/enum_mapping_guide.md` para identificar mapeamentos de enums legados.
7. Escreva qualquer ponto incerto ou divergente como um item `DÚVIDA:` no final do arquivo de Spec Funcional.
8. **PARE E PEÇA APROVAÇÃO DO USUÁRIO.** Não avance para a geração de código C# ou React sem que o usuário responda "Aprovado".

---

## 2. Geração de Back-end (Fase 2: Arquiteto Back-end)

Após aprovação da Spec Funcional pelo usuário:
1. Crie os arquivos DTO em `src/Versatus.AcessoGlobal/Domain/DTOs/[Nome]Dto.cs` contendo as classes de persistência/leitura (`Salvar[Nome]Dto`, `Obter[Nome]Dto`).
2. Crie a interface de serviço em `src/Versatus.AcessoGlobal/Domain/Services/I[Nome]Service.cs`.
3. Crie a implementação do serviço em `src/Versatus.AcessoGlobal/Domain/Services/[Nome]Service.cs` incorporando as validações, regras de negócio e campos de auditoria idênticos ao legado.
4. Crie o controlador REST em `src/Versatus.AcessoGlobal/Api/Controllers/[Nome]Controller.cs` com suporte a paginação, filtros, inserção, atualização e exclusão, usando como modelo `references/BACKEND/EntidadeController.cs`.
5. Execute `dotnet build` na pasta raiz do projeto de API para verificar erros de compilação. Corrija-os imediatamente.
6. Crie o commit: `git add -A && git commit -m "Feat(backend): Add [Nome] DTOs, Service and Controller"`.

---

## 3. Geração de Front-end (Fase 3: Arquiteto Front-end)

Após a compilação limpa do backend:
1. Crie a pasta do formulário em `src/Versatus.Frontend/src/pages/F[Nome]/`.
2. Crie `types.ts` contendo o formulário `I[Nome]Form` e `defaultValues`.
3. Crie `schema.ts` contendo as regras de validação Zod baseadas nos campos e na Spec.
4. Crie `[Nome]CadastroConfig.tsx` estendendo `BaseCadastroConfig`. Configure as colunas, filtros e a conversão bidirecional (`mapBackendToForm` e `mapFormToBackend`) de enums e campos legados se houver.
5. Crie `index.tsx` contendo a View do formulário usando MUI (Material UI), abas para sub-tabelas e os campos estruturados da Spec.
6. Execute `npm run build` na pasta do frontend para validar tipagem TypeScript e empacotamento Vite. Corrija quaisquer avisos ou erros.
7. Crie o commit: `git add -A && git commit -m "Feat(frontend): Add F[Nome] page, schema, types and config"`.

---

## 4. Integração e Finalização (Fase 4: Integrador)

Após o build limpo do frontend:
1. Importe o novo formulário em `src/Versatus.Frontend/src/App.tsx`.
2. Adicione a rota correspondente (ex: `/cadastro/[nome-url]`) na estrutura de rotas do sistema.
3. Adicione o link para o novo cadastro no menu lateral de navegação.
4. Execute `npm run build` novamente para confirmar que a rota integrada não gerou quebras de build.
5. Crie o commit: `git add -A && git commit -m "Feat(route): Register F[Nome] in App.tsx navigation"`.
6. Envie as alterações para o repositório remoto: `git push origin feat/migrate-[nome]`.
7. Apresente um resumo detalhado dos arquivos criados e alterados e informe ao usuário que a branch está pronta para revisão e merge manual.
