---
name: migrate-crud
description: Pipeline de migração automatizada de formulários legados (C#) para a arquitetura .NET 10 + React OOP seguindo SOLID, Clean Architecture e Clean Code.
---

# Skill: migrate-crud

Este skill guia o agente através do pipeline de migração de formulários do ERP legado para o novo sistema, garantindo conformidade com **SOLID, Clean Architecture, Clean Code e recursos modernos do .NET 10**.

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

### Formato de Solicitação Aceito

O usuário pode fornecer os arquivos legados de **duas formas equivalentes**:

**Forma A — Caminho do arquivo (preferida):**
```
Migrar formulário: [Nome]
Módulo: [NomeDoModulo]
Branch: feat/migrate-[nome]

Entidade legada:
c:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\projeto_tag_1906\servidor\objeto de negócio\[modulo]\[Nome].cs

Formulário legado:
c:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\projeto_tag_1906\cliente\cliente.aplicativo\[modulo]\F[Nome].cs

Observações:
(opcional — apenas restrições de negócio que não estão no código)
```

**Forma B — Conteúdo colado:**
```
Migrar formulário: [Nome]
Módulo: [NomeDoModulo]

Entidade legada (.cs):
[cole EXATAMENTE o conteúdo completo do arquivo, sem simplificar]

Formulário legado (.cs):
[cole EXATAMENTE o conteúdo completo do arquivo, sem simplificar]
```

> [!IMPORTANT]
> Quando o usuário fornecer **caminhos de arquivo**, use a ferramenta `view_file` para ler o conteúdo completo de cada arquivo antes de qualquer análise. Nunca assuma o conteúdo sem ler.
> Quando o usuário fornecer **conteúdo colado**, verifique se parece completo. Se o código tiver aparência de stub ou simplificado (ex: "// restante do código..."), peça o arquivo real antes de prosseguir.

Quando o usuário solicitar a migração:

1. Localize a fase do formulário no arquivo specs/00-INDICE-GERAL.md.
2. Leia atentamente as Regras Anti-Alucinação em specs/03-REGRAS-ANTI-ALUCINACAO.md.
3. Leia a SPEC do módulo correspondente em specs/modulos/MOD-0X-[MODULO].md.
4. **Leia os arquivos legados** via `view_file` (se caminhos) ou analise o conteúdo colado.
5. **[OBRIGATÓRIO] Mapeamento completo de propriedades e Nulidade:**
   - Liste TODAS as propriedades/campos da entidade legada.
   - Verifique o esquema do banco de dados legado (ou campos condicionais do formulário). Propriedades que aceitam `NULL` no banco ou que são preenchidas apenas em determinadas abas/tipos DEVEM ser declaradas como nulas (`int?`, `decimal?`, enums nulos) na entidade C# para evitar exceções de runtime `SqlNullValueException: Data is Null`.
6. Execute a **Classificação Obrigatória** da Seção 0 e documente o resultado.
7. Gere a Spec Funcional em `docs/spec_f[nome].md` seguindo rigorosamente o formato de `docs/spec_fentidade.md`.
   - **[OBRIGATÓRIO] Requisitos Arquiteturais (SOLID / Clean Architecture):** Documente na Spec que a implementação backend deverá respeitar a separação Clean Architecture (Domain POCO, Application/Services, Infrastructure EF Core Mapping, Api Controller limpo) e comunicação via Result Pattern.
   - **[OBRIGATÓRIO] Critérios de Aceite:** Inclua a seção `## 7. Critérios de Aceite (Cenários de Teste)` detalhando o comportamento em cenários felizes (ex: busca/listagem com filtros, retorno `200/201`), cenários de falha via Result Pattern (`400 BadRequest` com `ValidationError`), alinhamento de nulidade e regras de UI/UX.
   - Para Padrão B [LOTE]: documente os endpoints `/escopo` e `/salvar-valores`, o agrupador, os escopos suportados e as regras de ocultação dos botões.
8. Use o arquivo `references/FRONTEND/enum_mapping_guide.md` para identificar mapeamentos de enums legados.
9. Escreva qualquer ponto incerto ou divergente como um item `DÚVIDA:` no final do arquivo de Spec Funcional.
10. **PARE E PEÇA APROVAÇÃO DO USUÁRIO.** Não avance para geração de código C# ou React sem que o usuário responda "Aprovado".

---

## 2. Geração de Back-end (Fase 2: Arquiteto Back-end)

Após aprovação da Spec Funcional pelo usuário:

> [!IMPORTANT]
> **Boas Práticas C# / .NET 10 / SOLID / Clean Architecture / CQRS:**
> - **Domain:** Classes POCO puras em `Domain/Entities/`, sem DataAnnotations, com file-scoped namespaces e Nullable Reference Types habilitados.
> - **SOLID & Injeção de Dependência:** Interfaces desacopladas em `Domain/Services/I[Nome]Service.cs` e `Domain/Repositories/`. Injeção via construtor no Controller.
> - **CQRS DB Split:** Repositórios utilizam `ReadContext` / `ReadDbSet` (com `NoTracking` na `ReadConnection`) para consultas e `Context` / `DbSet` (na `WriteConnection`) para gravações.
> - **Clean Code:** Métodos focados, nomes expressivos alinhados à Spec.
> - **Result Pattern:** PROIBIDO usar `throw new ...Exception(...)` para indicar erros de validação de dados ou falta de registros. Sempre retornar `Result<T>` com `ValidationError`.

> [!WARNING]
> **SQL Server 2008:** O banco de dados NÃO suporta OFFSET/FETCH nativos do EF Core.
> Sempre buscar para memória com ToListAsync() antes de paginar com .Skip().Take() na camada de serviço. Nunca use .Skip().Take() diretamente na query do banco.

> [!WARNING]
> **Lock de DLL:** Se dotnet run estiver ativo (servidor rodando), dotnet build falhará por lock nos arquivos DLL.
> Antes de rodar dotnet build, encerre processos `dotnet` que estejam travando os arquivos DLL.

1. Crie os arquivos DTO em `src/Versatus.[Modulo]/Domain/DTOs/[Nome]Dto.cs`.
   - [LOTE] Inclua DTOs específicos para leitura por escopo (`[Nome]EscopoDto`) e salvamento em lote (`Salvar[Nome]ValorDto`).
2. Crie as Entidades POCO e Mapeamentos Fluent API:
   - Garanta que propriedades opcionais no banco legado sejam nulas sem `.IsRequired()` no EF Core Mapping.
3. Crie a interface de serviço em `src/Versatus.[Modulo]/Domain/Services/I[Nome]Service.cs`.
4. Crie a implementação do serviço em `src/Versatus.[Modulo]/Domain/Services/[Nome]Service.cs`.
   - [LOTE] Implemente ObterPorEscopo(...) e SalvarValores(IList<...> alteracoes) em vez de CRUD padrão.
   - Paginação: use sempre `var lista = await query.Include(...).ToListAsync(); var paginado = lista.Skip(...).Take(...);`
   - Se o DTO de resposta ler propriedades navegáveis filhas (ex: `Regras`, `Itens`), inclua `.Include(x => x.[Colecao])` na query paginada para evitar referências nulas.
   - Suporte filtros adicionais relevantes no método de paginação (`disponibilidade`, `ativo`, `status`, etc.).
5. Crie o controlador REST em `src/Versatus.[Modulo]/Api/Controllers/[Nome]Controller.cs`.
   - Controller enxuto delegando para `I[Nome]Service`.
   - Exponha parâmetros adicionais de filtro via `[FromQuery]`.
   - Trate retornos do tipo `Result<T>`, convertendo falhas de validação em `400 BadRequest`.
6. **[OBRIGATÓRIO] Crie os Testes Unitários de Negócio (TDD):**
   - Crie `tests/Versatus.[Modulo].Tests/[Nome]ServiceTests.cs`.
   - Escreva 1 teste unitário (`[Fact]`) para **CADA LINHA DA MATRIZ RTV** mapeada na Spec.
   - Teste todos os cenários de falha funcional (`Result.IsSuccess == false`) e o cenário de sucesso.
7. Encerre o servidor se necessário.
8. Execute `dotnet build` e `dotnet test tests/Versatus.[Modulo].Tests/`. Corrija todos os erros de compilação ou testes com falha imediatamente.
9. Crie o commit: `git commit -m "feat(backend): Add [Nome] DTOs, Service, Controller and 100% RTV Unit Tests"`.

---

## 3. Geração de Front-end (Fase 3: Arquiteto Front-end)

Após a compilação limpa do backend:

> [!IMPORTANT]
> A pasta do formulário DEVE seguir a estrutura por módulo: `src/pages/[Modulo]/F[Nome]/`
> Nunca criar diretamente em `src/pages/F[Nome]/` (estrutura plana proibida).

1. Crie a pasta do formulário em `src/Versatus.Frontend/src/pages/[Modulo]/F[Nome]/`.
2. Crie `types.ts` contendo `I[Nome]Form` e `defaultValues`.
3. Crie `schema.ts` com as regras de validação Zod baseadas na Spec.
4. Crie `[Nome]CadastroConfig.tsx` estendendo `BaseCadastroConfig<T>` (OOP / Clean Architecture no frontend).
   - Sobrescreva `mapBackendToForm` e `mapFormToBackend` se houver diferenças de enums ou estrutura.
   - Configure em `getFiltros()` os filtros específicos da tela (ex: texto de busca, seletores de status/disponibilidade).
5. Crie `index.tsx` com a View do formulário:
   - **[CRUD]** Grid paginado + botões Novo/Editar/Excluir usando MUI.
   - **[LOTE]** Accordions **fechados por padrão**, agrupados pelo campo Agrupador. Cada item exibe: (a) Descrição em destaque, (b) Chave técnica como subtexto, (c) campo de edição inline condicionado ao tipo. Botões Novo e Excluir completamente ausentes do JSX.
   - **[OBRIGATÓRIO] Confirmação de Alterações Não Salvas:**
       - **Botão Cancelar / Voltar:** Exibe banner inline amarelo com Título `"Alterações não salvas"` e Mensagem `"Você possui alterações não salvas no formulário. Deseja realmente cancelar e descartar as alterações?"` com botões `[Descartar e Sair]` e `[Continuar Editando]`.
       - **Fechamento de Aba [X]:** Exibe confirmação com o texto `"Você possui alterações não salvas na aba \"[Nome da Aba]\". Deseja realmente fechar e descartar as alterações?"`.
   - **[OBRIGATÓRIO] Padrão Visual de Campos MUI — Floating Label:**
      - Todos os campos do formulário DEVEM usar `variant="outlined"` no MUI (`TextField`, `Select`, `FormControl`).
      - O label NUNCA deve ser externo (acima do campo). Ele deve ser o `InputLabel` do próprio MUI, que flutua sobre a borda superior do campo no padrão floating label.
      - **Estado normal:** borda cinza (1px), label pequeno flutuando sobre a borda em cinza.
      - **Estado focado:** borda azul (2px, `primary.main`), label em azul flutuando sobre a borda.
      - **Estado de erro:** borda vermelha (2px), label em vermelho, `helperText` abaixo em vermelho com a mensagem de validação Zod.
      - **Proibido:** usar `placeholder` como substituto de label. O `label` prop do MUI é obrigatório em todos os campos.
      - Exemplo correto `TextField`: `<TextField variant="outlined" label="Descrição" required error={!!errors.descricao} helperText={errors.descricao?.message} />`
      - Exemplo correto `Select`: `<FormControl variant="outlined" fullWidth required error={!!errors.tipo}><InputLabel>Tipo Condição</InputLabel><Select label="Tipo Condição" ...>`
   - **[OBRIGATÓRIO] Sinalização visual de obrigatoriedade:** Todo campo definido como obrigatório na Spec (`✅`) DEVE receber a prop `required` no componente MUI (`TextField`, `FormControl`). O MUI exibirá o asterisco `*` automaticamente no label. Omitir `required` é proibido pela Regra 10 do AGENTS.md.
   - **[OBRIGATÓRIO] Checklist de Cobertura de Propriedades:** Antes de finalizar o JSX de `index.tsx`, compare a lista de propriedades da interface `I[Nome]Form` com o formulário e garanta que TODAS as propriedades editáveis (como `ativo`/`situacao`, flags, observações) tenham componentes de entrada (TextField, Switch, Checkbox, Select) correspondentes na tela. Omitir campos do DTO na UI é proibido pela Regra 3 do AGENTS.md.
6. **[OBRIGATÓRIO] Crie os Testes Unitários de Schema Zod (`schema.test.ts`):**
   - Crie `src/Versatus.Frontend/src/pages/[Modulo]/F[Nome]/schema.test.ts`.
   - Escreva testes unitários Vitest para **CADA REGRA DA MATRIZ RTV** no Zod schema.
7. Execute `npm test` e `npm run build` na pasta do frontend. Corrija quaisquer erros de teste ou compilação imediatamente.
8. Crie o commit: `git commit -m "feat(frontend): Add F[Nome] page, schema, types, config and 100% RTV schema tests"`.

---

## 4. Integração e Finalização (Fase 4: Integrador)

Após o build limpo do frontend:

1. Importe o novo formulário em `src/Versatus.Frontend/src/App.tsx`.
2. Adicione a rota correspondente (ex: `/[modulo]/[nome-url]`) na estrutura de rotas.
3. Adicione o link para o novo cadastro no menu lateral de navegação.
4. Execute `npm run build` novamente para confirmar que a rota integrada não gerou quebras.
5. Crie o commit: `git commit -m "feat(route): Register F[Nome] in App.tsx navigation"`.
6. Envie: `git push origin feat/migrate-[nome]`.
7. Apresente um resumo detalhado dos arquivos criados/alterados e faça o merge para a branch `develop`.
8. **[OBRIGATÓRIO] Limpeza de Branch:** Após o merge para `develop`, PERGUNTE AO USUÁRIO se ele deseja excluir a branch de recurso local e remota (`feat/migrate-[nome]`) para manter o repositório limpo e evitar confusão.
