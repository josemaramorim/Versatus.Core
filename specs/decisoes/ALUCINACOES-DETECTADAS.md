# ALUCINAÇÕES DETECTADAS
## Registro de problemas gerados pela IA durante a migração

> **Propósito:** Registrar cada caso em que a IA gerou algo incorreto,
> para calibrar os prompts futuros e evitar repetição.
>
> **IMPORTANTE:** Este arquivo deve ser mantido atualizado por todo o time.

---

## Formato de Registro

```
## [DATA] — [Módulo/Arquivo] — [Título Curto]

**O que a IA fez errado:**
...

**O que deveria ter feito:**
...

**Como foi corrigido:**
...

**Regra violada:**
REGRA X do documento 03-REGRAS-ANTI-ALUCINACAO.md

**Lição aprendida (para ajustar o prompt):**
...
```

---

## Registros

## 2026-06-06 — AcessoGlobal/GestaoTributo — Violações de Clean Architecture e Alucinações de Mapeamento Físico

**O que a IA fez errado:**
1. **Violação de Clean Architecture (REGRA 17)**: Injetou repositórios (`IRepository`, `IUsuarioRepository`, `IEntidadeRepository`) diretamente nos controladores API (`EntidadeController`, `AuthController`, `TributacaoController`), violando a regra de mantê-los como adaptadores finos HTTP e contornando a camada de domínio/serviços. Declarou DTOs aninhados e regras de geração de token diretamente no controlador.
2. **Alucinações de Mapeamento com Banco Legado (Lei nº 4)**:
   - Chaves e Colunas Inexistentes: Tentou mapear `IdPais` em `GloEstado`, `IdGloSerieDocumentoFilial` e `IdSequencialSerieDocto` em `GloSerieDocumentoFilial`, e `ProximoNumero` em `GloSerieDocumento` / `GloSerieDocumentoFilial`. Nenhuma destas colunas/chaves existia fisicamente na base.
   - Nomes de Colunas Errados: Mapeou `IdSinteticoAnalitico` para `GloCentroCusto` (correto: `IDTIPO`), `GloCategoria` (correto: `IDANALITICOSINTETICO`) e `FinClasse` (correto: `IDTIPO`). Mapeou `IdTipoNatureza` em `FinClasse` (correto: `IDNATUREZA`). Mapeou `IdForma` em `GloFormaPagamento` como `IdFormaPagamento` (correto: `IDGLOFORMAPAGAMENTO`). Mapeou `Tipo` (IdTipoValor) em `GloParametro` como string (correto: `int` nullable).
   - Tipos e Nullabilidade: Mapeou `Latitude` e `Longitude` como `double` em vez de `decimal?` (colunas são `numeric` no SQL Server, gerando `InvalidCastException`). Mapeou propriedades de auditoria (`IdUsuarioInclusao`, `DataInclusao`, `HoraInclusao`) e outros campos de cadastro legados (`Abreviacao`, `MascaraClasse`, `CPF`, `CNPJ`, `Numero`) como não-nulos ou obrigatórios, provocando `SqlNullValueException` e erros de restrição ao materializar registros legados que continham nulos.

**O que deveria ter feito:**
1. Injetar apenas interfaces de serviços (`IEntidadeService`, `IAutenticacaoService`, `ITributoService`) nos controladores API, delegando regras de negócio e consultas complexas à camada de domínio. Declarar DTOs em arquivos dedicados na camada de domínio.
2. Consultar fisicamente o esquema da base legada SQL Server local antes de mapear entidades e relacionamentos no EF Core, garantindo o uso correto dos nomes de tabelas/colunas, tipos compatíveis (.NET `decimal?` para SQL Server `numeric`), chaves primárias compostas reais (ex: composite `IDGLOSERIEDOCUMENTO`/`IDGLOFILIAL` em `GloSerieDocumentoFilial`) e nullabilidade correta.

**Como foi corrigido:**
1. Refatoração completa dos controladores para atuarem como adaptadores HTTP finos, dependendo apenas dos novos serviços de aplicação criados.
2. Correção de todos os modelos de domínio e mapeamentos do EF Core para usar tipos compatíveis (mudando propriedades de `double` para `decimal?` e marcando propriedades legadas nulas como opcionais / nullable). Mapeamento das chaves primárias e relacionamentos reais do SQL Server, incluindo o composite key em `SerieDocumentoFilial`.
3. Inclusão de testes de esquema real executados contra a instância SQL Server para materializar o primeiro registro de cada DbSet e detectar falhas em tempo de teste unitário/integrado.

**Regra violada:**
REGRA 17 do documento `03-REGRAS-ANTI-ALUCINACAO.md` e Lei nº 4 do `04-CONTRATO-DA-IA.md`.

**Lição aprendida (para ajustar o prompt):**
Nunca assumir a estrutura ou tipos de dados de uma base relacional legada. Sempre verificar a nullabilidade física e nomes de colunas usando scripts de inspeção (`INFORMATION_SCHEMA.COLUMNS`) antes de codificar mapeamentos EF Core. Manter controladores 100% livres de dependências de persistência.

---

*Arquivo criado em: 2026-04-27 — Manter atualizado durante toda a migração*
