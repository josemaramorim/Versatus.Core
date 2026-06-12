# Prompt de Análise — MOD-03 Fase 4: Produto.cs (189 KB)

> **Para a IA executora:** Sua tarefa aqui é **exclusivamente ANÁLISE** — não escreva nenhum arquivo `.cs` ainda.  
> O objetivo é ler o arquivo legado e documentar tudo na spec.  
> Ao final, atualize a spec com os resultados.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**

### Antes de começar:
```powershell
git checkout develop
git pull origin develop
git checkout -b docs/analise-produto-legado
```

### Ao finalizar a análise (somente após escrever na spec):
```powershell
git add specs/modulos/MOD-03-GESTAO-MATERIAL.md
git commit -m "docs(gestao-material): documenta análise de Produto.cs — Fase 4"
# NÃO faça merge — deixe a branch para o usuário revisar
```

> **Regra:** Nunca use `git merge`, `git push origin develop` ou `git rebase` sem permissão do usuário.

---


## Contexto

Você está migrando o sistema legado **Versatus** (`.NET Framework 4.x + Gentle.NET + .NET Remoting`) para **.NET 8 + EF Core + REST API**.

O `Produto.cs` é a entidade mais crítica do módulo **Gestão de Material**.  
Com 189 KB, ele contém propriedades, relacionamentos, enums e regras de negócio que DEVEM ser documentados antes de qualquer implementação.

**Regra fundamental do projeto:** A IA não "projeta" — ela **traduz fielmente** o legado.

---

## Arquivo a Analisar

```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\Produto.cs
```

**Tamanho:** ~189 KB — leia em partes se necessário (800 linhas por vez).

---

## O Que Documentar

Leia o arquivo completamente e extraia:

### 1. Identificação da Tabela
- Localize o atributo `[TableName("...")]` na classe principal
- Anote o nome exato da tabela no banco (ex: `PrcProduto`)

### 2. Identificação da PK
- Localize o atributo `[AutoSequencial("...", SequencialTipo.xxx)]`
- Anote o nome da coluna PK e o tipo de sequencial usado

### 3. Todas as Propriedades
Para cada propriedade pública, documente:
- Nome da propriedade C#
- Tipo C# (int, string, decimal, bool, DateTime, etc.)
- Nome da coluna no banco (via `[TableColumn]` ou inferido)
- Se é FK para outra entidade — qual?
- Se é nullable ou obrigatória

### 4. Enums/Constantes Internos
- Qualquer enum, const ou propriedade que retorne um conjunto fixo de valores

### 5. Métodos de Regra de Negócio
- Liste cada método que contenha lógica de negócio (não getters/setters simples)
- Para cada um: nome do método, resumo do que faz (2-3 linhas)
- **NÃO copie o código legado inteiro** — apenas o resumo funcional

### 6. Subclasses / Herança
- O que `Produto` herda? (provavelmente `ObjectBase` — ignorar essa herança)
- Existem subclasses de `Produto`?

### 7. Relacionamentos Identificados
Liste todas as propriedades que referenciam outras entidades:
```
Produto → GrupoEstoque (IdGrupoEstoque: int)
Produto → Unidade (IdUnidade: int)
Produto → ProdutoFilial[] (coleção)
... etc
```

---

## Padrões do Legado para Identificar

```csharp
// Identificar a tabela:
[TableName("PrcProduto")]

// Identificar a PK e sequencial:
[AutoSequencial("IdProduto", SequencialTipo.Filial)]

// Identificar colunas:
[TableColumn("Descricao")]
public string Descricao { get; set; }

// Identificar FKs (geralmente int com nome "IdXxx"):
public int IdGrupoEstoque { get; set; }
public int IdUnidade { get; set; }
```

---

## Formato de Saída Esperado

Ao terminar a análise, **atualize a spec** no arquivo:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\modulos\MOD-03-GESTAO-MATERIAL.md
```

Preencha a **seção 3.1.1** com o resultado no seguinte formato:

```markdown
#### 3.1.1 Propriedades do Produto — ANÁLISE CONCLUÍDA

**Tabela:** `PrcProduto` (confirmar)
**PK:** `IdProduto` — SequencialTipo: Filial

| Propriedade C# | Tipo | Coluna no BD | Nullable | FK para |
|---|---|---|---|---|
| IdProduto | int | IdPrcProduto | Não | — |
| Descricao | string | Descricao | Não | — |
| IdGrupoEstoque | int | IdPrcGrupoEstoque | Não | GrupoEstoque |
| IdUnidade | int | IdPrcUnidade | Não | Unidade |
| CustoAtual | decimal | CustoAtual | Sim | — |
| Ativo | bool | Ativo | Não | — |
... (todas as propriedades)

**Enums/Constantes Identificados:**
- `TipoProduto`: { ProdutoAcabado = 1, MateriaPrima = 2, Servico = 3 }
- ... (todos)

**Regras de Negócio Identificadas:**
- `CalcularPreco()`: calcula preço de venda com base no custo e margem.
- `ValidarGrade()`: verifica se o produto tem grade configurada corretamente.
- ... (todas)

**Relacionamentos:**
- `Produto → GrupoEstoque` via `IdGrupoEstoque (int)`
- `Produto → Unidade` via `IdUnidade (int)`
- `Produto → ProdutoFilial[]` (coleção)
- ... (todos)
```

---

## O Que NÃO Fazer

- ❌ **Não crie nenhum arquivo `.cs` ainda** — apenas análise
- ❌ **Não simplifique** as propriedades — documente todas, mesmo as que pareçam obsoletas
- ❌ **Não invente** propriedades que não estejam no legado
- ❌ **Não descarte** enums ou métodos de regra de negócio

---

## Verificação de Conclusão

Sua análise está completa quando:
- ✅ A seção `3.1.1` da spec `MOD-03-GESTAO-MATERIAL.md` está preenchida
- ✅ Todas as propriedades públicas foram documentadas em tabela
- ✅ Todos os métodos de negócio foram resumidos
- ✅ Todos os relacionamentos foram listados
- ✅ A linha do checklist `- [ ] Fase 4: Análise + Produto — Completa` está marcada como `- [/] Fase 4: Em Análise`

---

## Arquivos Auxiliares para Consultar (se necessário)

```
# Outros arquivos do mesmo módulo legado (para entender relacionamentos):
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\ProdutoFilial.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\GrupoEstoque.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\Unidade.cs

# Spec do módulo (onde você vai escrever os resultados):
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\modulos\MOD-03-GESTAO-MATERIAL.md
```
