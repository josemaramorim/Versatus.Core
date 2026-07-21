---
name: migrate-crud
description: Pipeline de migração automatizada de formulários legados (C#) para a arquitetura .NET 10 + React OOP.
---

# Skill: migrate-crud

Este skill guia o agente através do pipeline de migração de formulários do ERP legado para o novo sistema.

---

## 0. Classificação Obrigatória do Formulário (ANTES de tudo)

Antes de qualquer geração de spec ou código, classifique o formulário em um dos dois padrões abaixo.
Analise o arquivo .cs legado e responda às perguntas:

| Pergunta | Sim | Não |
|---|---|---|
| O formulário possui botão Novo/Inserir? | → CRUD Padrão | |
| O formulário possui botão Excluir/Deletar? | → CRUD Padrão | |
| O formulário edita valores de itens já existentes em lote? | → Configuração em Lote | |
| A tela principal usa TreeList, Accordion ou agrupamento expansível? | → Configuração em Lote | |

### Padrão A: CRUD Padrão
Exemplos: FEntidade, FCondicaoPagamento, FProduto
- Grid paginado com Novo / Editar / Excluir
- Endpoints: GET paginado, GET/{id}, POST, PUT/{id}, DELETE/{id}
- Frontend herda BaseCadastroConfig<T> com grid e formulário lateral/modal
- **Siga as Fases 1→4 completas**

### Padrão B: Configuração em Lote
Exemplos: FParametro, FPermissao
- Accordion/TreeList agrupando itens, salvamento em lote
- Endpoints: GET /escopo (ou equivalente) + PUT /salvar-valores (ou equivalente)
- Botões Novo e Excluir são **completamente ocultados** na UI
- **Siga as Fases 1→4 com as variações marcadas como [LOTE]**

> [!IMPORTANT]
> Documente o padrão identificado no cabeçalho da Spec Funcional antes de qualquer outra coisa.

---

## 1. Ritual de Início (Fase 1: Analista de Esquema)

Quando o usuário solicitar a migração de um formulário fornecendo o nome, o módulo e os arquivos .cs legados:

1. Localize a fase do formulário no arquivo specs/00-INDICE-GERAL.md.
2. Leia atentamente as Regras Anti-Alucinação em specs/03-REGRAS-ANTI-ALUCINACAO.md.
3. Leia a SPEC do módulo correspondente em specs/modulos/MOD-0X-[MODULO].md.
4. Analise os arquivos .cs legados recebidos.
5. **[OBRIGATÓRIO] Mapeamento completo de propriedades:** Liste TODAS as propriedades/campos da entidade legada e verifique se cada uma tem mapeamento no C# atual (entidade + DTO + mapping EF). Anote discrepâncias como DÚVIDA: na spec. Propriedades ignoradas causam retrabalho.
6. Execute a **Classificação Obrigatória** da Seção 0 e documente o resultado.
7. Gere a Spec Funcional em docs/spec_f[nome].md seguindo rigorosamente o formato de docs/spec_fentidade.md.
   - Para Padrão B [LOTE]: documente os endpoints /escopo e /salvar-valores, o agrupador, os escopos suportados e as regras de ocultação dos botões.
8. Use o arquivo eferences/FRONTEND/enum_mapping_guide.md para identificar mapeamentos de enums legados.
9. Escreva qualquer ponto incerto ou divergente como um item DÚVIDA: no final do arquivo de Spec Funcional.
10. **PARE E PEÇA APROVAÇÃO DO USUÁRIO.** Não avance para geração de código C# ou React sem que o usuário responda Aprovado.

---

## 2. Geração de Back-end (Fase 2: Arquiteto Back-end)

Após aprovação da Spec Funcional pelo usuário:

> [!WARNING]
> **SQL Server 2008:** O banco de dados NÃO suporta OFFSET/FETCH nativos do EF Core.
> Sempre buscar para memória com ToListAsync() antes de paginar com .Skip().Take() na camada de serviço. Nunca use .Skip().Take() diretamente na query do banco.

> [!WARNING]
> **Lock de DLL:** Se dotnet run estiver ativo (servidor rodando), dotnet build falhará por lock nos arquivos DLL.
> Antes de rodar dotnet build, verifique tasks ativas com manage_task list e mate o servidor com manage_task kill se necessário.

1. Crie os arquivos DTO em src/Versatus.[Modulo]/Domain/DTOs/[Nome]Dto.cs.
   - [LOTE] Inclua DTOs específicos para leitura por escopo ([Nome]EscopoDto) e salvamento em lote (Salvar[Nome]ValorDto).
2. Crie a interface de serviço em src/Versatus.[Modulo]/Domain/Services/I[Nome]Service.cs.
3. Crie a implementação do serviço em src/Versatus.[Modulo]/Domain/Services/[Nome]Service.cs.
   - [LOTE] Implemente ObterPorEscopo(...) e SalvarValores(IList<...> alteracoes) em vez de CRUD padrão.
   - Paginação: use sempre ar lista = await query.ToListAsync(); var paginado = lista.Skip(...).Take(...);
4. Crie o controlador REST em src/Versatus.[Modulo]/Api/Controllers/[Nome]Controller.cs.
   - Use eferences/BACKEND/EntidadeController.cs como modelo base.
   - [LOTE] Exponha apenas GET /escopo e PUT /salvar-valores como endpoints principais da UI.
5. Mate o servidor se estiver rodando: manage_task kill.
6. Execute dotnet build na pasta raiz. Corrija todos os erros de compilação imediatamente.
7. Reinicie o servidor se necessário.
8. Crie o commit: git commit -m Feat(backend): Add [Nome] DTOs, Service and Controller.

---

## 3. Geração de Front-end (Fase 3: Arquiteto Front-end)

Após a compilação limpa do backend:

> [!IMPORTANT]
> A pasta do formulário DEVE seguir a estrutura por módulo: src/pages/[Modulo]/F[Nome]/
> Nunca criar diretamente em src/pages/F[Nome]/ (estrutura plana proibida).

1. Crie a pasta do formulário em src/Versatus.Frontend/src/pages/[Modulo]/F[Nome]/.
2. Crie 	ypes.ts contendo I[Nome]Form e defaultValues.
3. Crie schema.ts com as regras de validação Zod baseadas na Spec.
4. Crie [Nome]CadastroConfig.tsx estendendo BaseCadastroConfig<T>.
   - Sobrescreva mapBackendToForm e mapFormToBackend se houver diferenças de enums ou estrutura.
   - [LOTE] O config pode ser mínimo; a lógica principal fica em index.tsx.
5. Crie index.tsx com a View do formulário:
   - **[CRUD]** Grid paginado + botões Novo/Editar/Excluir usando MUI.
   - **[LOTE]** Accordions **fechados por padrão**, agrupados pelo campo Agrupador. Cada item exibe: (a) Descrição em destaque, (b) Chave técnica como subtexto, (c) campo de edição inline condicionado ao tipo. Botões Novo e Excluir completamente ausentes do JSX.
6. Execute 
pm run build na pasta do frontend. Corrija quaisquer erros ou warnings de tipo.
7. Crie o commit: git commit -m Feat(frontend): Add F[Nome] page, schema, types and config.

---

## 4. Integração e Finalização (Fase 4: Integrador)

Após o build limpo do frontend:

1. Importe o novo formulário em src/Versatus.Frontend/src/App.tsx.
2. Adicione a rota correspondente (ex: /[modulo]/[nome-url]) na estrutura de rotas.
3. Adicione o link para o novo cadastro no menu lateral de navegação.
4. Execute 
pm run build novamente para confirmar que a rota integrada não gerou quebras.
5. Crie o commit: git commit -m Feat(route): Register F[Nome] in App.tsx navigation.
6. Envie: git push origin feat/migrate-[nome].
7. Apresente um resumo detalhado dos arquivos criados/alterados e informe que a branch está pronta para revisão e merge manual.

