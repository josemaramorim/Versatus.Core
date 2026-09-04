# MOD-05 — Pesquisa e Decisões de Investigação

> SDD etapa 3 (`sdd-plan`). Cada item: problema → opções → **decisão** → impacto.
> Sem decisão fechada → `DÚVIDA:` (nenhuma bloqueia o `/tasks`).

---

## 1. Substitutos .NET 10 para libs de terceiros (épico E14)

### 1.1 `BoletoNet` / `Boleto.Net` (geração de remessa CNAB e boleto)
- **Legado:** `Boleto.Net.csproj` (.NET Framework), usado em `View/ArquivoRemessaView.cs`,
  `FBoletoCobranca`, `FEnviarBoletoEmail` (6 `using BoletoNet`).
- **Opções:** (a) `BoletoNetCore` (fork .NET Standard/Core, NuGet ativo); (b) `BoletoNet.Core`;
  (c) implementação própria de CNAB (240/400) a partir das especificações FEBRABAN;
  (d) manter geração no legado e o novo só orquestra.
- **Decisão:** avaliar `BoletoNetCore` como candidato principal na tarefa `analysis` de E14;
  fallback = parser/gerador CNAB próprio (o layout por banco já está codificado no legado —
  `ArquivoRetorno240View`/`400View` + `ArquivoRetornoBaseView<T,I>`). **Não decidir agora** —
  E14 é a última fase.
- **Impacto:** referência de pacote só entra no `.csproj` no E14; núcleo não depende.

### 1.2 `BarcodeLib` (código de barras do boleto)
- **Opções:** `BarcodeStandard` / `ZXing.Net` (net10) ou geração do payload numérico apenas
  (a renderização gráfica é do frontend — CLR-08).
- **Decisão:** backend calcula **linha digitável / código de barras (string)**; renderização
  gráfica no frontend. Sem `BarcodeLib`.

### 1.3 OFX (conciliação bancária — `FConciliacaoBancaria`)
- **Opções:** `OfxSharp` / `OfxParser` (net) ou parser SGML/XML próprio (OFX 1.x é SGML,
  2.x é XML).
- **Decisão:** avaliar `OfxSharp` no E14; fallback parser próprio. **Não decidir agora.**

---

## 2. Enums — inventário, valores inteiros e persistência

- **Duas famílias no legado — classificar cada enum:**
  - **Persistido** — `Projeto.Geral.Enumerado`, decorado com `[TipoEnumerado(idPai)]`. O
    **valor inteiro é gravado** numa coluna `ID*` do banco (ex.: `IDSITUACAO`,
    `IDRECEBERPAGAR`, `IDPROCESSOORIGEM`). Valores **imutáveis** — dado histórico depende
    deles. Label dinâmico via `GloTipoEnumerado` em runtime
    (ver `docs/analise_integracao_enumerados.md`).
  - **Não-persistido** — `Projeto.Geral.EnumeradoObjeto`, **sem** `[TipoEnumerado]`.
    Comportamento / UI / código de retorno (ex.: `MascaraPredefinida`, `MensagemTipo`,
    `AcessoTipo` `[Flags]`). **Nunca** vira coluna; pode não ter valor explícito; label
    estático.
  - **Misto** — está em `EnumeradoObjeto` mas tem valores explícitos que aparecem em
    dados/config (ex.: `SequencialTipo` = 230/231/232). Sinalizar caso a caso.
- **Decisão:** os enums do MOD-05 entram em `Versatus.SharedKernel` com os **valores
  inteiros preservados**. No mapeamento das entidades, **só os persistidos** ganham
  `HasConversion<int>()` na coluna `ID*` correspondente (feito no épico dono da entidade);
  os não-persistidos ficam apenas no código. Labels seguem a estratégia dinâmica do doc de
  enumerados (não hardcode no React).
- **Extração fina por épico** (na tarefa `analysis`), a partir de:
  - `projeto_tag_1906/Geral/tipoenumerado.cs` (persistidos), `TipoEnumeradoObjeto.cs`
    (não-persistidos), `EnumDescriptor.cs`
  - os arquivos de enum em `Projeto.Geral.Enumerado*`
  - colunas `ID<Algo>` do `data-model.md` que são enum (não FK): `IDSITUACAO`
    (`SituacaoDocumento`/`SituacaoParcela`), `IDRECEBERPAGAR` (`PagarReceberTipo`),
    `IDTIPOCONTA`, `IDTIPOREGISTRO`, `IDPROCESSOORIGEM` (`ProcessoOrigem`), `IDAPLICAR`,
    `IDAPLICACAO`, `IDCALCULO`, `IDNATUREZA`, `IDTIPOCALCULODRE`, `IDTIPOCONTABANCARIA`,
    `IDTIPOARQUIVOREMESSARETORNO`, `IDTIPOMANUTENCAORATEIO`.
- **DÚVIDA-R2:** alguns `ID*` podem ser FK a tabelas de domínio do MOD-02 (ex.:
  `IDGLOINDICEECONOMICO`) e não enum — classificar 1 a 1 no `analysis` do épico.

---

## 3. `GeradorSequencialService` (IDs por filial)

- **Já existe** em `Versatus.Framework` (`ProximoAsync(nomeObjeto, SequencialTipo.Filial)`,
  tabela `Sequencia`, incremento por `(Tabela, IdFilial)`).
- **Decisão:** usar como está. Cada `IncluirXHandler` chama `ProximoAsync("<nomeLegado>",
  Filial)` antes de inserir; o `<nomeLegado>` é o mesmo string usado no `[AutoSequencial]`
  legado (ex.: `"IdDocumento"`, `"IdMovimentoFinanceiro"`) — **confirmar o string exato por
  entidade no `analysis`** (o Gentle usava o 1º argumento do atributo).
- **Impacto:** mapping de PK sempre `.ValueGeneratedNever()`; nenhum `IDENTITY`.

---

## 4. `Lookup` (busca cacheada de entidade relacionada)

- **Legado:** `Lookup(typeof(IEntidade), "IdEntidade")` — carrega sob demanda e cacheia o
  objeto relacionado dentro da entidade de negócio.
- **Decisão:** **não replicar** o `Lookup` como está. Onde o legado usa `Lookup` para
  navegar a outra entidade do **mesmo módulo**, usar navegação EF (`HasOne`). Para
  **cross-módulo**, o campo é só `int` e o serviço que precisar do dado relacionado chama a
  API/serviço do outro módulo (ou um cache `IMemoryCache` no serviço). Um utilitário leve
  `Lookup<T>` pode ir ao SharedKernel se ≥3 épicos precisarem do mesmo padrão de cache —
  senão, não criar (`clarify.md` CLR-02: ampliar SharedKernel só sob necessidade).

---

## 5. Golden values de cálculo com banco vazio (R-2)

- **Situação:** o banco de dev tem quase todas as tabelas `Fin*` com 0 linhas; só
  `FINITEMFINANCEIRO` (6), `FINDRETITULO` (41), `FINCLASSE` (142), `FINMOTIVODEVOLUCAOCHEQUE`
  (38), `FINDOMINIO` (2), `FINCAIXABANCO` (3), `FINDRE` (1) têm dados.
- **Decisão — fonte dos golden por ordem de preferência:**
  1. **Execução do legado** (`.NET Framework`) com entradas controladas — capturar
     entrada + resultado (juros/multa/desconto/valor convertido/rateio).
  2. **Conferência manual** de um caso pelo usuário quando (1) não for viável — registrar
     `origem: manual` no `golden/CALC-xx.csv`.
  3. **Registros reais** de uma base de produção/homologação, se o usuário disponibilizar.
- Cada linha do golden carrega a coluna `origem` (`legado` | `manual` | `producao`).
- **DÚVIDA-R3:** existe base de homologação com dados financeiros reais que eu possa
  consultar para golden values? (não bloqueia — afeta só a qualidade da amostra)

---

## 6. Tipos legados problemáticos

| Tipo legado | Ocorrência | Decisão |
| :--- | :--- | :--- |
| `text` (TEXT) | `HISTORICO`, `MENSAGEMERROENVIOEMAILBOLETO` | `string?` + `.HasColumnType("text")`. Não migrar para `varchar(max)` (Artigo II — schema sagrado). |
| `datetime` para hora | `HORAINCLUSAO`, `HORALIBERACAO`, … (datetime cheio) | Propriedade `DateTime?` separada da data. **Não** fundir data+hora (Regra 5). |
| `varchar(8)` para hora | `FINDOMINIOPERIODOFORMAPAGTO.HORA` | `string`. |
| `smallint` para bool | `ATIVO`, `GERABOLETO`, … | `bool` + `HaveConversion<short>()`. `smallint` que for enum → `int`/`enum` (checar no `analysis`). |
| `numeric(23,8)` | valores monetários | `decimal` + `.HasPrecision(23, 8)`. |
| `numeric(17,2)` | `ULTIMONOSSONUMERO`, `NOSSONUMEROSEQUENCIAL` | `decimal` + `.HasPrecision(17, 2)`. |

---

## 7. Impressão (CLR-08)

- **Decisão:** endpoints de DRE, cheque e recibo de liquidação retornam **record de
  dados** (`DreImpressaoResponse`, `ReciboLiquidacaoResponse`, `ChequeImpressaoResponse`);
  layout/PDF é responsabilidade do frontend. A lógica de *montagem dos dados* (somatórios
  do DRE, dados do recibo) migra para serviço; a de *desenho* não.

---

## 8. Pendências de pesquisa (não bloqueiam `/tasks`)

- DÚVIDA-R1: `FINPROJECAOFLUXOCAIXA(+LACTO)` e `FINLOGDOMINIOPERIODO` ausentes no banco de
  dev → obter schema de produção antes de E2/E11 (também em `plan.md §7 R-1`).
- DÚVIDA-R2: classificação enum vs. FK das colunas `ID*` — por épico.
- DÚVIDA-R3: base de homologação para golden values de cálculo.
- Lib CNAB/boleto/OFX: decisão final no `analysis` de E14.
