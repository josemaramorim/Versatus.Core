# GUIA DE HANDOFF — Como Passar Trabalho para Outra IA

> **Para o usuário:** Este documento explica exatamente o que falar para uma nova IA começar a trabalhar no projeto, em qual ordem usar os prompts, e o que verificar antes de passar para o próximo.

---

## ⚠️ Regra de Git — Leia Antes de Tudo

> **NUNCA permita que a IA commite em `develop` ou `main`.**

Cada prompt cria uma branch específica:

| Prompt | Branch criada |
|---|---|
| Fases 1-3 (Execução) | `feat/gestao-material-fases1-3` |
| Fase 4 (Análise) | `docs/analise-produto-legado` |
| Fases 5-8 (Execução) | `feat/gestao-material-fases5-8` |
| Fases 9-10 (Análise) | `docs/analise-estoque-movimento-legado` |
| Fases 11-12 (Execução) | `feat/gestao-material-fases11-12` |
| MOD-07 Fase 5 (Análise) | `docs/analise-sped-legado` |

**Fluxo correto:**
```
develop → feat/xxx → (IA trabalha) → commit na feat/xxx → você revisa → merge com --no-ff
```

**Nunca deixe a IA fazer:**
- `git merge develop` ou `git push origin develop`
- `git push origin main`
- `git rebase` sem sua permissão

---



## 📂 Onde estão os prompts de execução

Todos os prompts estão na pasta do projeto:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\prompts-execucao\
```

| Arquivo | Tipo | Fase |
|---|---|---|
| `MOD-03-FASES1-3-EXECUTION-PROMPT.md` | ✅ Execução | Criar projeto + Unidades + Classificação |
| `MOD-03-FASE4-ANALISE-PRODUTO.md` | 🔍 Análise | Ler `Produto.cs` e documentar na spec |
| `MOD-03-FASES5-8-EXECUTION-PROMPT.md` | ✅ Execução | Grades, Composição, Localização, Lote/Série |
| `MOD-03-FASES9-10-ANALISE-ESTOQUE.md` | 🔍 Análise | Ler `Estoque.cs` e `MovimentoEstoque.cs` |
| `MOD-03-FASES11-12-EXECUTION-PROMPT.md` | ✅ Execução | Auxiliares + Repositórios + Migration |
| `MOD-07-FASE5-ANALISE-SPED.md` | 🔍 Análise | Ler arquivos SPED e documentar |

---

## 🔢 Ordem de Execução

```
PASSO 1 → MOD-03-FASES1-3-EXECUTION-PROMPT.md   (criar projeto do zero)
   ↓ verificar: dotnet build = 0 erros
PASSO 2 → MOD-03-FASE4-ANALISE-PRODUTO.md        (analisar Produto.cs 189KB)
   ↓ verificar: seção 3.1.1 da spec preenchida
PASSO 3 → MOD-03-FASES5-8-EXECUTION-PROMPT.md    (Grades, Composição, Lote)
   ↓ verificar: dotnet build + testes passando
PASSO 4 → MOD-03-FASES9-10-ANALISE-ESTOQUE.md    (analisar Estoque + Movimento)
   ↓ verificar: seções 3.2.1 e 3.3 da spec preenchidas
PASSO 5 → MOD-03-FASES11-12-EXECUTION-PROMPT.md  (Auxiliares + Migration final)
   ↓ verificar: migration criada + testes passando

PARALELO → MOD-07-FASE5-ANALISE-SPED.md          (pode rodar junto com qualquer passo)
```

---

## 📋 Passo a Passo: Como Falar com a Nova IA

### Mensagem de abertura (copie e cole):

```
Você é um engenheiro de software trabalhando no projeto Versatus .NET 8.

ANTES DE QUALQUER COISA, leia estes arquivos na ordem:
1. C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\04-CONTRATO-DA-IA.md
2. C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\03-REGRAS-ANTI-ALUCINACAO.md
3. C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\prompts-execucao\[NOME DO ARQUIVO].md

Após ler os três arquivos, confirme que entendeu e AGUARDE minha aprovação antes de escrever qualquer código.
```

> Substitua `[NOME DO ARQUIVO]` pelo arquivo do passo que você quer executar.

---

### Para cada passo, verifique antes de passar para o próximo:

#### ✅ Passo 1 concluído quando:
- `dotnet build` = 0 erros, 0 avisos
- Pasta `src\Versatus.GestaoMaterial\` existe com os arquivos listados no prompt
- `dotnet test` = todos os testes existentes passando

#### ✅ Passo 2 concluído quando:
- Abra `specs\modulos\MOD-03-GESTAO-MATERIAL.md`
- Seção `3.1.1 Propriedades do Produto` está preenchida (não mais "PENDENTE DE ANÁLISE")
- A tabela de propriedades tem pelo menos 15 linhas
- Relacionamentos e regras de negócio estão documentados

#### ✅ Passo 3 concluído quando:
- `dotnet build` = 0 erros
- Pastas `Domain/Grades/`, `Domain/Composicao/`, `Domain/Lote/`, `Domain/Serie/` existem
- `dotnet test` = todos os testes passando

#### ✅ Passo 4 concluído quando:
- Seções `3.2.1` e `3.3` da spec estão preenchidas
- Fórmula de custo médio está documentada com caso especial (saldo zero)
- Todos os tipos de movimento estão listados

#### ✅ Passo 5 concluído quando:
- `dotnet build` = 0 erros
- Pasta `src\Versatus.GestaoMaterial\Migrations\` existe com `InitialCreate`
- `dotnet test` = todos os testes passando

---

## ⚠️ Avisos Importantes

1. **Não pule etapas** — os prompts de execução (Passos 3 e 5) têm pré-requisitos explícitos
2. **Não permita que a IA implemente `Produto.cs` sem a análise do Passo 2 concluída**
3. **Não permita que a IA implemente `Estoque.cs` ou `MovimentoEstoque.cs` sem o Passo 4 concluído**
4. **Se a IA inventar propriedades** que não estão no legado — peça para ela reler o prompt de análise

---

## 🔗 Arquivos de Referência do Projeto (para informar à nova IA se ela pedir)

```
Raiz: C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\

Spec do módulo:   specs\modulos\MOD-03-GESTAO-MATERIAL.md
Spec MOD-07:      specs\modulos\MOD-07-GESTAO-TRIBUTO.md
Arquitetura:      specs\01-VISAO-GERAL-ARQUITETURA.md
Contrato da IA:   specs\04-CONTRATO-DA-IA.md
Anti-alucinação:  specs\03-REGRAS-ANTI-ALUCINACAO.md

Legado (referência):
  gestao.material: C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\
  gestao.tributo:  C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.tributo\
  SPED.Fiscal:     C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\SPED.Fiscal\

Projeto modelo (copiar padrões daqui):
  src\Versatus.GestaoTributo\    — estrutura mais recente e completa
  src\Versatus.AcessoGlobal\     — padrões de mapping e repositório
```
