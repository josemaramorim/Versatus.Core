# Prompt de Análise — MOD-03 Fases 9–10: Estoque.cs e MovimentoEstoque.cs

> **Para a IA executora:** Sua tarefa aqui é **exclusivamente ANÁLISE** — não escreva nenhum arquivo `.cs` ainda.  
> Execute esta análise **somente após** a Fase 4 (Produto.cs) ter sido concluída e documentada.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**

### Antes de começar:
```powershell
git checkout develop
git pull origin develop
git checkout -b docs/analise-estoque-movimento-legado
```

### Ao finalizar a análise (somente após escrever na spec):
```powershell
git add specs/modulos/MOD-03-GESTAO-MATERIAL.md
git commit -m "docs(gestao-material): documenta análise de Estoque.cs e MovimentoEstoque.cs — Fases 9-10"
# NÃO faça merge — deixe a branch para o usuário revisar
```

> **Regra:** Nunca use `git merge`, `git push origin develop` ou `git rebase` sem permissão do usuário.

---



## Contexto

Você está analisando as duas entidades de estoque mais críticas do módulo **Gestão de Material** do sistema legado **Versatus** (`.NET Framework 4.x + Gentle.NET`).

- `Estoque.cs` (38 KB) — controla saldos, custo médio, reservas e limites
- `MovimentoEstoque.cs` (72 KB) — registra toda entrada/saída e **recalcula o custo médio ponderado**

> ⚠️ A fórmula de custo médio é CRÍTICA. Se for implementada errado, todos os relatórios financeiros ficam incorretos. Documente com precisão cirúrgica.

---

## Arquivos a Analisar

```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\Estoque.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\MovimentoEstoque.cs
```

Arquivos auxiliares úteis:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\SaldoEstoque.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\EstoqueReserva.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.material\EstoqueComposicao.cs
```

---

## PARTE A — Analisar Estoque.cs (38 KB)

### O Que Documentar

#### A.1 — Identificação da Tabela e PK
- Atributo `[TableName("...")]` → nome da tabela
- Atributo `[AutoSequencial("...", SequencialTipo.xxx)]` → PK e tipo de sequencial

#### A.2 — Todas as Propriedades
Tabela completa: Nome C#, Tipo, Coluna BD, Nullable, FK para qual entidade.

**Foco especial nestas propriedades (se existirem):**
- `SaldoFisico` / `SaldoFinanceiro` / `SaldoDisponivel`
- `SaldoMinimo` / `SaldoMaximo` / `QuantidadeReorden`
- `CustoMedio` / `CustoAtual` / `UltimoCusto`
- `QuantidadeReservada`
- `IdProduto`, `IdFilial`, `IdAlmoxarifado`
- `Ativo`

#### A.3 — Lógica de Saldo Disponível
Documente exatamente como é calculado o saldo disponível:
```
Saldo Disponível = ??? - ???
```
Muitas vezes é: `SaldoFisico - SaldoReservado` ou similar.

#### A.4 — Lógica de Saldo Mínimo/Máximo
Como o sistema detecta estoque abaixo do mínimo? Existe algum método ou flag?

#### A.5 — Métodos de Regra de Negócio
Para cada método relevante: nome + resumo funcional (2-3 linhas).

---

## PARTE B — Analisar MovimentoEstoque.cs (72 KB)

### O Que Documentar

#### B.1 — Identificação da Tabela e PK

#### B.2 — Todas as Propriedades
Tabela completa. Foco especial em:
- `TipoMovimento` (enum?) — quais valores? (Entrada, Saída, Ajuste, Transferência, Devolução, etc.)
- `Quantidade`, `QuantidadeConvertida`
- `PrecoUnitario`, `CustoUnitario`, `ValorTotal`
- `DataMovimento`, `HoraMovimento`
- `IdEstoque`, `IdFilial`, `IdProduto`
- `IdDocumento` / referência ao documento de origem (NF, pedido)
- `SaldoAnterior`, `SaldoPosterior` (se existirem)

#### B.3 — CRÍTICO: Fórmula do Custo Médio Ponderado

Localize o método que recalcula o custo médio após uma entrada e documente a fórmula COMPLETA:

```
// Exemplo do que pode estar no legado:
novoCustoMedio = (SaldoAtual × CustoMedioAtual + Quantidade × CustoEntrada) / (SaldoAtual + Quantidade)
```

Documente:
1. O nome exato do método
2. Os parâmetros que ele recebe
3. A fórmula linha por linha
4. Se há tratamento para saldo zero (divisão por zero)
5. Se o custo médio é calculado por filial, por almoxarifado, ou global

#### B.4 — Tipos de Movimento
Liste TODOS os tipos de movimento identificados (enum ou constante):
```
TipoMovimento:
  1 = Entrada (aumenta saldo, recalcula custo médio)
  2 = Saída (diminui saldo)
  3 = Ajuste de Inventário (aumenta ou diminui)
  4 = Transferência entre almoxarifados
  5 = Devolução de venda (entrada especial)
  ... (documentar todos)
```

Para cada tipo: qual o impacto no saldo e no custo médio?

#### B.5 — Impacto no Estoque
Quando um `MovimentoEstoque` é persistido, o que acontece automaticamente?
- O saldo do `Estoque` é atualizado imediatamente?
- O custo médio é recalculado?
- Alguma trigger ou método de callback?

#### B.6 — Métodos de Regra de Negócio
Todos os métodos relevantes com resumo.

---

## Formato de Saída Esperado

Atualize a spec em:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\modulos\MOD-03-GESTAO-MATERIAL.md
```

Preencha as seções **3.2.1** e **3.3** com o formato:

```markdown
#### 3.2.1 Propriedades do Estoque — ANÁLISE CONCLUÍDA

**Tabela:** `PrcEstoque`
**PK:** `IdEstoque` — SequencialTipo: Filial

| Propriedade C# | Tipo | Coluna | Nullable | FK |
|---|---|---|---|---|
| IdEstoque | int | IdPrcEstoque | Não | — |
| IdProduto | int | IdPrcProduto | Não | Produto |
| IdFilial | int | IdGloFilial | Não | (cross-module) |
...

**Fórmula do Saldo Disponível:**
```
SaldoDisponivel = SaldoFisico - QuantidadeReservada
```

---

### 3.3 MovimentoEstoque.cs — ANÁLISE CONCLUÍDA

**Tabela:** `PrcMovimentoEstoque`
**PK:** `IdMovimento` — SequencialTipo: Filial

| Propriedade C# | Tipo | Coluna | Nullable | FK |
|---|---|---|---|---|
...

**Tipos de Movimento (TipoMovimento):**
| Valor | Nome | Impacto Saldo | Impacto Custo Médio |
|---|---|---|---|
| 1 | Entrada | +Quantidade | Recalcula |
| 2 | Saída | -Quantidade | Não altera |
...

**Fórmula do Custo Médio Ponderado:**
```
Método: RecalcularCustoMedio(decimal quantidadeEntrada, decimal custoUnitarioEntrada)

novoCustoMedio = (saldoFisicoAnterior × custoMedioAnterior + quantidadeEntrada × custoUnitarioEntrada)
                 / (saldoFisicoAnterior + quantidadeEntrada)

Caso especial (saldo zero): novoCustoMedio = custoUnitarioEntrada
```
```

---

## O Que NÃO Fazer

- ❌ **Não implemente** nenhum arquivo `.cs`
- ❌ **Não simplifique** a fórmula de custo médio — documente ela exatamente
- ❌ **Não assuma** que o custo médio funciona de forma padrão — leia o código
- ❌ **Não pule** o `MovimentoEstoque.cs` por ser grande — é o mais crítico

---

## Verificação de Conclusão

- ✅ Seção `3.2.1` da spec preenchida com todas as propriedades de `Estoque`
- ✅ Seção `3.3` da spec preenchida com todas as propriedades de `MovimentoEstoque`
- ✅ Fórmula de custo médio documentada com caso especial (saldo zero)
- ✅ Todos os tipos de movimento documentados com impacto em saldo e custo
- ✅ Checklist: `- [/] Fase 9: Em Análise` e `- [/] Fase 10: Em Análise`
