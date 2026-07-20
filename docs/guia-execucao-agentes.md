# Guia de Execução: Agentes de Migração de Formulários

> **Arquivo:** `docs/guia-execucao-agentes.md`  
> **Público:** Desenvolvedor / Responsável pela migração  
> **Pré-requisito:** Os arquivos em `.agents/` já estarem criados no repositório

---

## O que este guia explica

Como usar os agentes de IA para migrar formulários do sistema legado (C#/.NET Framework) para a nova arquitetura (C#/.NET 10 + React/Vite) **sem precisar explicar a arquitetura toda vez**.

---

## Antes de Começar (uma única vez)

Confirme que os seguintes arquivos existem no repositório:

```
.agents/
├── AGENTS.md
└── skills/
    └── migrate-crud/
        ├── SKILL.md
        └── references/
            ├── BACKEND/
            │   ├── EntidadeController.cs
            │   ├── EntidadeService.cs
            │   └── EntidadeDto.cs
            └── FRONTEND/
                ├── BaseCadastroConfig.ts
                ├── EntidadeCadastroConfig.tsx
                ├── useCrudListState.ts
                ├── spec_fentidade.md
                └── enum_mapping_guide.md
```

Se algum arquivo estiver faltando, consulte o time de desenvolvimento.

---

## Fluxo Resumido

```
Você faz:                          A IA faz:
─────────────────────────────────────────────────────────────
1. Cola o .cs legado no chat   →   Lê specs/, analisa .cs
                               ←   Entrega Spec Funcional
2. Revisa e escreve "Aprovado" →   Gera backend (.NET 10)
                               →   Gera frontend (React)
                               →   Registra rota + commit
                               ←   Apresenta resumo final
3. Faz merge para develop      →   (apenas você — nunca a IA)
─────────────────────────────────────────────────────────────
Total de mensagens suas: 2 por formulário
```

---

## Passo a Passo Detalhado

### Passo 1 — Prepare os arquivos do formulário legado

Antes de abrir o chat da IA, tenha em mãos **dois arquivos `.cs`** do sistema legado:

| Arquivo | Para que serve | Exemplo |
|---|---|---|
| **Classe de Entidade** | Define os campos e tipos | `CondicaoPagamento.cs` |
| **Serviço / Form legado** | Define as regras de negócio | `CondicaoPagamentoService.cs` |

> **Dica:** Quanto mais arquivos você fornecer, mais precisa será a análise. Se o formulário legado tiver um `FrmCondicaoPagamento.cs` (Winforms/WebForms), inclua também.

---

### Passo 2 — Abra o chat e envie a mensagem de migração

Use exatamente este modelo (copie, substitua e envie):

```
Migrar formulário: [NomeDoFormulario]

Entidade legada (.cs):
[cole aqui o conteúdo completo do arquivo de entidade]

Serviço/Form legado (.cs):
[cole aqui o conteúdo completo do arquivo de serviço ou formulário]

Observações extras (opcional):
- [descreva aqui qualquer regra de negócio que não esteja clara no código]
```

**Exemplo real:**

```
Migrar formulário: CondicaoPagamento

Entidade legada (.cs):
public class CondicaoPagamento : ObjetoNegocio
{
    public int IdCondicaoPagamento { get; set; }
    public string Descricao { get; set; }
    ...
}

Serviço/Form legado (.cs):
public class CondicaoPagamentoService
{
    public void Validar(CondicaoPagamento obj)
    {
        if (obj.NumParcelas <= 0)
            throw new Exception("Número de parcelas inválido");
        ...
    }
}
```

> **Importante:** Você **não** precisa dizer "use o skill" nem "siga o processo". A IA detecta automaticamente que é uma migração e aplica o pipeline completo.

---

### Passo 3 — Revise a Spec Funcional gerada

A IA vai:
1. Ler os documentos da pasta `specs/` (anti-alucinação, arquitetura, módulo correspondente)
2. Analisar os `.cs` fornecidos
3. Gerar o arquivo `docs/spec_f[nome].md`
4. Apresentar a Spec para sua revisão

**O que você deve verificar na Spec:**
- [ ] Os campos estão corretos e completos?
- [ ] As regras de negócio foram capturadas corretamente?
- [ ] Os endpoints da API estão certos?
- [ ] As colunas e filtros da listagem fazem sentido?
- [ ] Há alguma `DÚVIDA:` marcada pela IA que você precisa responder?

**O que você responde:**
- ✅ `Aprovado` — para a IA continuar com o backend e frontend
- ✏️ `Corrija: [descrição]` — para ajustar antes de prosseguir

---

### Passo 4 — Aguarde os agentes 2, 3 e 4

Após o seu `Aprovado`, a IA executa automaticamente:

| Agente | O que faz | Validação |
|---|---|---|
| **Agente 2 – Backend** | Cria DTO, Service e Controller C# | `dotnet build` |
| **Agente 3 – Frontend** | Cria types, schema, CadastroConfig e View | `npm run build` |
| **Agente 4 – Integrador** | Registra rota em App.tsx e faz commit na feature branch | Git push |

> A IA reporta o progresso de cada etapa. Se houver erro de build, ela tenta corrigir automaticamente antes de passar para a próxima fase.

---

### Passo 5 — Revisão final e merge

Após os 4 agentes concluírem, a IA apresenta um resumo com:
- Arquivos criados no backend
- Arquivos criados no frontend
- Nome da feature branch com o commit

**Você revisa a feature branch:**
```bash
git checkout feat/migrate-[nome]
# Abra os arquivos e revise
```

**Se aprovado, você (não a IA) faz o merge:**
```bash
git checkout develop
git merge --no-ff feat/migrate-[nome]
git push origin develop
```

> ⚠️ **Regra de ouro:** A IA **nunca** faz push para `develop` ou `main`. Apenas você faz isso, após validação.

---

## Estrutura de Branches

```
main              ← Produção — apenas você faz merge aqui
  └── develop     ← Homologação — você faz merge após validar
       └── feat/migrate-[nome]  ← A IA trabalha aqui
```

---

## Padrão de Commits (gerados automaticamente pela IA)

| Fase | Formato | Exemplo |
|---|---|---|
| Spec gerada | `Docs: Add spec_f[nome].md` | `Docs: Add spec_fcondicaopagamento.md` |
| Backend | `Feat(backend): Add [Nome] DTOs, Service and Controller` | `Feat(backend): Add CondicaoPagamento DTOs, Service and Controller` |
| Frontend | `Feat(frontend): Add F[Nome] page, schema, types and config` | `Feat(frontend): Add FCondicaoPagamento page, schema, types and config` |
| Rota | `Feat(route): Register F[Nome] in App.tsx navigation` | `Feat(route): Register FCondicaoPagamento in App.tsx navigation` |

---

## O que Fazer Quando a IA "Esquecer" o Contexto

Em sessões longas ou ao trocar de conversa, a IA pode perder o contexto do projeto. Nesses casos:

1. Abra o arquivo [`specs/05-ONBOARDING-IA-PROMPT.md`](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/05-ONBOARDING-IA-PROMPT.md)
2. Copie todo o conteúdo
3. Cole no **início** da nova conversa
4. Em seguida, envie a mensagem de migração normalmente

---

## Perguntas Frequentes

**❓ Preciso abrir uma nova conversa para cada formulário?**  
Não obrigatoriamente, mas é recomendado. Conversas separadas evitam que contexto de um formulário interfira em outro.

**❓ O que faço se a IA gerar um campo errado?**  
Aponte na revisão da Spec (Passo 3) antes de aprovar. É muito mais simples corrigir na Spec do que no código gerado.

**❓ Posso migrar dois formulários ao mesmo tempo?**  
Não. Faça um formulário de cada vez, na ordem definida em `specs/decisoes/DEC-004-ORDEM-MIGRACAO.md`.

**❓ E se o build falhar e a IA não conseguir corrigir?**  
A IA vai informar o erro e pausar. Você decide: corrige manualmente, fornece mais contexto, ou abre uma nova sessão com o erro descrito.

**❓ Onde vejo os formulários já migrados?**  
No índice de specs: [`specs/00-INDICE-GERAL.md`](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/00-INDICE-GERAL.md) — coluna **Status**.

---

## Referências

| Documento | Link |
|---|---|
| Índice Geral de SPECs | [00-INDICE-GERAL.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/00-INDICE-GERAL.md) |
| Arquitetura do Sistema | [01-VISAO-GERAL-ARQUITETURA.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/01-VISAO-GERAL-ARQUITETURA.md) |
| Regras Anti-Alucinação | [03-REGRAS-ANTI-ALUCINACAO.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/03-REGRAS-ANTI-ALUCINACAO.md) |
| Contrato da IA (Git + Leis) | [04-CONTRATO-DA-IA.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/04-CONTRATO-DA-IA.md) |
| Onboarding da IA | [05-ONBOARDING-IA-PROMPT.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/05-ONBOARDING-IA-PROMPT.md) |
| Spec de Exemplo (Entidade) | [spec_fentidade.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/docs/spec_fentidade.md) |
| Ordem de Migração | [DEC-004-ORDEM-MIGRACAO.md](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/specs/decisoes/DEC-004-ORDEM-MIGRACAO.md) |
