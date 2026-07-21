# Prompt de Análise — MOD-07 Fase 5: SPED (EFD-ICMS/IPI e EFD-PIS/COFINS)

> **Para a IA executora:** Sua tarefa aqui é **exclusivamente ANÁLISE** — não escreva nenhum arquivo `.cs` ainda.  
> Esta análise é pré-requisito para implementação. O SPED é entregue ao governo federal e um erro pode gerar multa.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**

### Antes de começar:
```powershell
git checkout develop
git pull origin develop
git checkout -b docs/analise-sped-legado
```

### Ao finalizar a análise (somente após escrever na spec):
```powershell
git add specs/modulos/MOD-07-GESTAO-TRIBUTO.md
git commit -m "docs(gestao-tributo): documenta análise de EFD e EFDPisCofins — Fase 5 SPED"
# NÃO faça merge — deixe a branch para o usuário revisar
```

> **Regra:** Nunca use `git merge`, `git push origin develop` ou `git rebase` sem permissão do usuário.

---



## Contexto

Você está analisando o módulo **SPED** do sistema legado **Versatus** (`.NET Framework 4.x + Gentle.NET`).

O SPED (Sistema Público de Escrituração Digital) é a escrituração fiscal eletrônica entregue à Receita Federal. É composto por:
- **EFD-ICMS/IPI** — Escrituração Fiscal Digital de ICMS e IPI (`EFD.cs`)
- **EFD-PIS/COFINS** — Escrituração das contribuições PIS e COFINS (`EFDPisCofins.cs`)

A geração do SPED envolve **blocos** de registros com regras específicas de preenchimento obrigatório. Um erro no layout pode causar rejeição pelo governo.

---

## Arquivos a Analisar

### Arquivos principais (dentro do módulo gestao.tributo):
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.tributo\EFD.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.tributo\EFDPisCofins.cs
```

### Arquivos do módulo SPED dedicado (ler para entender os blocos):
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\SPED.Fiscal\SpedBase.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\SPED.Fiscal\EFD_SPED.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\SPED.Fiscal\FuncoesSPED.cs
```

### Arquivos auxiliares do tributo:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.tributo\Inventario.cs
C:\Pasta de Trabalho\Projetos\Analises\Versatus\projeto_tag_1906\servidor\objeto de negócio\gestao.tributo\NotaFiscal.cs
```

> ⚠️ `SpedBase.cs` tem 260 KB — leia em partes (800 linhas por vez). Foque nos métodos de geração de blocos.

---

## O Que Documentar

### PARTE A — EFD.cs (EFD-ICMS/IPI)

#### A.1 — Identificação da Entidade
- Atributo `[TableName("...")]` → nome da tabela no banco
- Atributo `[AutoSequencial("...", SequencialTipo.xxx)]` → PK e sequencial
- O que representa: uma apuração mensal? Um arquivo por competência?

#### A.2 — Todas as Propriedades
Tabela completa: Nome C#, Tipo, Coluna BD, Nullable, FK.

**Foco especial:**
- `Competencia` / `Periodo` / `DataInicio` / `DataFim`
- `IdFilial` — como é referenciado (cross-module?)
- `SituacaoTransmissao` / `StatusEnvio` (enum?)
- `CaminhoArquivo` / `NomeArquivo`
- `HashArquivo` ou similar

#### A.3 — Blocos Gerados
Liste quais blocos SPED esta entidade gera (ex: Bloco 0, C, D, E, G, H, K):
- Para cada bloco: qual método é chamado? (ex: `GerarBloco0()`, `GerarBlocoC()`)
- Quais entidades são necessárias para cada bloco?

#### A.4 — Método Principal de Geração
Documente o método que gera o arquivo SPED completo:
- Nome do método
- Parâmetros (competência, filial, etc.)
- Sequência de chamadas de blocos
- Como o arquivo é gravado no disco

#### A.5 — Integrações Necessárias
Quais outras entidades são consultadas durante a geração?
```
EFD consulta:
- NotaFiscal (quais campos?)
- Inventario (quais campos?)
- ClassificacaoFiscal / CFOP (para blocos fiscais)
- Estoque (para Bloco H — Inventário)
```

---

### PARTE B — EFDPisCofins.cs (EFD-PIS/COFINS)

#### B.1 — Identificação da Entidade
- Tabela, PK, sequencial

#### B.2 — Diferenças em Relação ao EFD-ICMS/IPI
- Quais blocos são específicos do PIS/COFINS (ex: Bloco A, C, D, F, M, P)?
- Quais campos são diferentes?

#### B.3 — Método Principal de Geração
Mesmo que A.4, mas para PIS/COFINS.

---

### PARTE C — Análise dos Blocos (SpedBase.cs e EFD_SPED.cs)

Estes arquivos contêm a implementação técnica dos blocos SPED.

#### C.1 — Estrutura de Blocos Identificados
Liste todos os blocos encontrados:
```
Bloco 0 — Abertura, Identificação e Referências
Bloco C — Documentos Fiscais I (NF-e Mercadorias)
Bloco D — Documentos Fiscais II (Serviços)
Bloco E — Apuração do ICMS e do IPI
Bloco G — Controle de Créditos de ICMS do Ativo Permanente
Bloco H — Inventário Físico
Bloco K — Controle da Produção e do Estoque
Bloco 9 — Encerramento
... (documentar todos encontrados)
```

Para cada bloco:
- Qual classe/método o gera?
- Quais registros ele contém? (ex: C100, C170, C190)
- Quais entidades do domínio são usadas?

#### C.2 — Validações Obrigatórias
Existem validações antes de gerar o arquivo? (ex: nota fiscal deve ter CFOP, produto deve ter NCM):
- Liste todas as validações encontradas
- Qual exceção ou resultado é retornado em caso de falha?

---

## Formato de Saída Esperado

Atualize a spec em:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\modulos\MOD-07-GESTAO-TRIBUTO.md
```

Adicione uma nova seção após a seção existente de Fases:

```markdown
## Análise SPED — Fase 5 (Resultado)

### EFD-ICMS/IPI (EFD.cs)

**Tabela:** `TribEFD` (confirmar)
**PK:** `IdEFD` — SequencialTipo: Filial

| Propriedade C# | Tipo | Coluna | Nullable | FK |
|---|---|---|---|---|
| IdEFD | int | IdTribEFD | Não | — |
| IdFilial | int | IdGloFilial | Não | (cross-module) |
| Competencia | DateTime | Competencia | Não | — |
...

**Blocos Gerados:**
| Bloco | Método | Entidades Necessárias |
|---|---|---|
| Bloco 0 | GerarBloco0() | Empresa, Filial, Periodo |
| Bloco C | GerarBlocoC() | NotaFiscal, ItemNF, ClassificacaoFiscal |
| Bloco H | GerarBlocoH() | Inventario, SaldoEstoque |
...

**Validações Obrigatórias:**
1. Toda NF deve ter CFOP preenchido
2. Todo produto deve ter NCM para EFD
3. Período não pode ter lacunas (datas contínuas)
...

### EFD-PIS/COFINS (EFDPisCofins.cs)

[mesma estrutura]

### Decisões de Implementação (preencher após análise)

> **Como implementar no .NET 8:**
> - [ ] Entidade `EFD` no Domain (propriedades acima)
> - [ ] Serviço `EFDService` para geração do arquivo
> - [ ] `EFDGerador` como serviço de aplicação (responsável pelos blocos)
> - [ ] Arquivo gerado via `StreamWriter` em pasta configurável
```

---

## O Que NÃO Fazer

- ❌ **Não implemente** nenhum arquivo `.cs` ainda
- ❌ **Não simplifique** os blocos SPED — cada registro tem regra específica do governo
- ❌ **Não assuma** que os blocos funcionam como o manual SPED público — documente o que o legado faz
- ❌ **Não pule** o `SpedBase.cs` por ser grande (260 KB) — contém os registros detalhados

---

## Verificação de Conclusão

- ✅ Seção de Análise SPED adicionada à spec `MOD-07-GESTAO-TRIBUTO.md`
- ✅ Tabelas de propriedades de `EFD` e `EFDPisCofins` preenchidas
- ✅ Lista de blocos e registros documentada
- ✅ Integrações com outras entidades mapeadas
- ✅ Validações obrigatórias listadas
- ✅ Checklist: `- [/] Fase 5: SPED — Em Análise`
