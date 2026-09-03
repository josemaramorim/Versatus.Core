# MOD-05 — Modelo de Dados

> **Fonte:** inspeção real de `localhost\SQLEXPRESS2008 / versatus` (SQL Server 2008 R2),
> 2026-09-03, via `INFORMATION_SCHEMA`. Extração bruta commitada em
> [`legacy-schema/fin_columns.txt`](./legacy-schema/fin_columns.txt) e
> [`legacy-schema/fin_meta.txt`](./legacy-schema/fin_meta.txt) — **ground truth**.
>
> ⚠️ **Os nomes físicos são MAIÚSCULOS e diferem dos atributos `[TableName]` /
> `[AutoSequencial]` do Gentle no legado.** Ex.: `[TableName("FinAdiantamento")]` +
> `[AutoSequencial("IdAdiantamento")]` → tabela real `FINADIANTAMENTO`, PK real
> `IDFINADIANTAMENTO`. **Mapear SEMPRE a partir deste documento / do extract, nunca dos
> atributos `.cs`.** (Regra 1/2 e incidente de 2026-06-06 em `ALUCINACOES-DETECTADAS.md`.)

---

## 1. Convenções observadas (aplicam-se a todas as entidades do módulo)

| Aspecto | Regra no legado real | Mapeamento novo |
| :--- | :--- | :--- |
| **PK composta por filial** | Quase toda tabela `Fin*` tem PK **`(IDFIN<Entidade>, IDGLOFILIAL)`** — sequencial por filial. **A ordem das colunas varia** (`FINDOCUMENTOPARCELA` e `FINCHEQUERECEBIDOMOVTO` têm `IDGLOFILIAL` **primeiro**). | `builder.HasKey(x => new { x.IdXxx, x.IdFilial })` respeitando a ordem física. `IdFilial` é propriedade normal (`int`), não navegação. |
| **PK global (sem filial)** | Algumas tabelas de cadastro têm PK só `IDFIN<Entidade>`: `FINITEMFINANCEIRO`, `FINMOTIVODEVOLUCAOCHEQUE`, `FINPRIORIDADEPAGAMENTO`, `FINFILIALMOVTO`, `FINTRANSACAOFILIAL`, `FINTRANSACAOFINANCEIRA`. | `HasKey(x => x.IdXxx)`. Sequencial `Global` (RN-05-006). |
| **PK de 3 colunas** | `FINCAIXABANCOUSUARIO` (+`IDGLOUSUARIO`), `FINDOMINIOUSUARIO` (+`IDGLOUSUARIO`), `FINDOMINIORESPONSAVEL` (+`IDFINDOMINIOPAI`), `FINSALDOCAIXABANCO` (+`DATASALDO`), `FINTALAOCHEQUE` (+`CHEQUE`), `FINDOCUMENTOCOMISSIONADO` (+`IDGLOCOMISSIONADO`), `FINDOCUMENTOTRIBUTO` (+`IDGLOTRIBUTO`), `FINDRETITULOCLASSE` (+`IDFINCLASSE`), `FINDRETITULOOPERACAO` (+`IDFINDRETITULOCALCULO`), `FINITEMFINANCEIROOPERACAO` (+`IDGLOOPERACAO`/`IDGLOFILIAL`/`IDFINITEMFINANCEIRO`), `FINDOCTOMOVTOITEMFINANCEIRO` (3 col.), `FINLIQUIDACAOFORMAMOVIMENTO` (3 col.). | `HasKey(x => new { ... })` com todas as colunas na ordem física. |
| **`ValueGeneratedNever()`** | IDs vêm do `GeradorSequencial` legado. | Toda PK: `.ValueGeneratedNever()` (Artigo II.5). |
| **Dinheiro** | `numeric(23,8)` — precisão 23, escala **8** (não 2). | `decimal` + `.HasPrecision(23, 8)`. Nunca `double`. |
| **`ULTIMONOSSONUMERO` / `NOSSONUMEROSEQUENCIAL`** | `numeric(17,2)`. | `decimal` + `.HasPrecision(17, 2)`. |
| **Data e hora separadas** | Colunas `DATA*` e `HORA*` são **ambas `datetime`** (a hora é um `datetime` cheio). Exceção: `FINDOMINIOPERIODOFORMAPAGTO.HORA` é `varchar(8)`. | Duas propriedades `DateTime` / `DateTime?` conforme nulidade real. `varchar(8)` → `string`. **Não** fundir em um só campo (Regra 5). |
| **Booleanos** | `smallint NOT NULL` (`ATIVO`, `ENTRAFLUXOCAIXA`, `TESOURARIA`, `GERABOLETO`, `DOCUMENTOLIBERADO`…). | `bool` no domínio + `HaveConversion<short>()` em `ConfigureConventions` (Artigo III.5). **Checar caso a caso**: `smallint` que carrega enum (raro) fica `int`/enum. |
| **Textos longos** | Tipo legado **`text`** (`HISTORICO`, `MENSAGEMERROENVIOEMAILBOLETO`) — `CHARACTER_MAXIMUM_LENGTH = 2147483647`. | `string?` + `.HasColumnType("text")` (não alterar o tipo físico — Artigo II). |
| **Auditoria** | Padrão `IDGLOUSUARIOINCLUSAO, DATAINCLUSAO, HORAINCLUSAO, IDGLOUSUARIOALTERACAO, DATAALTERACAO, HORAALTERACAO` — prefixo **`IDGLO`**. **A nulidade varia por tabela** (em `FINDOCUMENTO`, `DATAALTERACAO`/`HORAINCLUSAO`/`HORAALTERACAO` são `NOT NULL`; em `FINCAIXABANCO` todas `NULL`; `FINADIANTAMENTO` tem só `IDGLOUSUARIO` + auditoria de alteração). | Extrair a nulidade **coluna a coluna** do extract. Nunca assumir "auditoria = tudo nullable". |
| **FKs cross-módulo** | Prefixo `IDGLO*` (`IDGLOFILIAL`, `IDGLOENTIDADE`, `IDGLOOPERACAO`, `IDGLOCONDICAOPAGAMENTO`, `IDGLOINDICEECONOMICO`, `IDGLOPORTADOR`, `IDGLOFORMACOBRANCA`, `IDGLOBANCO`, `IDGLOAGENCIA`, `IDGLOUSUARIO*`, `IDGLOTRIBUTO`, `IDGLOCOMISSIONADO`) e `IDFINCLASSE` (**`FinClasse` está em `acesso.global`**). | `int` / `int?` simples. **Sem `HasOne`/`Include` cross-projeto** (Artigo VIII.1). |
| **FKs intra-módulo** | `IDFINDOMINIO`, `IDFINDOMINIOPERIODO`, `IDFINDOCUMENTO`, `IDFINCAIXABANCO`, `IDFINITEMFINANCEIRO`, `IDFINCOBRADOR`, `IDFINDRETITULO`, `IDFINPROGRAMACAOCOBRANCAULTIMO`… | `HasOne(...).WithMany(...).HasForeignKey(...)` com as colunas reais; agregado usa `List<T>` privada + `IReadOnlyList<T>`. |
| **Enums** | Colunas `ID<Algo>` que não são FK a tabela (`IDSITUACAO`, `IDRECEBERPAGAR`, `IDTIPOCONTA`, `IDTIPOREGISTRO`, `IDTIPOCALCULODRE`, `IDPROCESSOORIGEM`, `IDAPLICAR`, `IDAPLICACAO`, `IDCALCULO`, `IDNATUREZA`…) — inteiros de enum. | `enum` com valores inteiros preservados (Artigo V.5) — inventário fino no `research.md` §2. |

---

## 2. Catálogo de tabelas (PK real + contagem de linhas + épico)

> Extraído de `fin_meta.txt`. `linhas` = estado do banco de dev/teste em 2026-09-03
> (majoritariamente vazio — ver `research.md §5` sobre golden tests).

| Tabela real | PK real (ordem física) | linhas | Entidade nova | Épico |
| :--- | :--- | :--- | :--- | :--- |
| `FINCAIXABANCO` | `IDFINCAIXABANCO, IDGLOFILIAL` | 3 | `CaixaBanco` | E-Caixa |
| `FINCONTABANCARIA` | `IDFINCAIXABANCO, IDGLOFILIAL` | 1 | `ContaBancaria` (1:1 de `CaixaBanco`) | E-Caixa (entidade) / E14 (integração) |
| `FINCAIXABANCOUSUARIO` | `IDFINCAIXABANCO, IDGLOFILIAL, IDGLOUSUARIO` | 0 | `CaixaBancoUsuario` | E-Caixa |
| `FINSALDOCAIXABANCO` | `IDFINCAIXABANCO, IDGLOFILIAL, DATASALDO` | 0 | `SaldoCaixaBanco` | E-Caixa |
| `FINSALDORATEIO` | (ver extract) | 0 | `SaldoRateio` | E-Caixa |
| `FINCOBRADOR` | `IDFINCOBRADOR, IDGLOFILIAL` | 0 | `Cobrador` | E-Caixa |
| `FINDOMINIO` | `IDFINDOMINIO, IDGLOFILIAL` | 2 | `Dominio` | E-Domínio |
| `FINDOMINIOPERIODO` | `IDFINDOMINIOPERIODO, IDGLOFILIAL` | 0 | `DominioPeriodo` | E-Domínio |
| `FINDOMINIOPERIODOFECHAMENTO` | `IDFINDOMINIOPERIODOFECHAMENTO, IDGLOFILIAL` | 0 | `DominioPeriodoFechamento` | E-Domínio |
| `FINDOMINIOPERIODOFORMAPAGTO` | `IDFINDOMINIOPERIODOFORMAPAGTO, IDGLOFILIAL` | 0 | `DominioPeriodoFormaPagto` | E-Domínio |
| `FINDOMINIOPERIODOLANCTO` | `IDFINDOMINIOPERIODOLANCTO, IDGLOFILIAL` | 0 | `DominioPeriodoLacto` | E-Domínio |
| `FINDOMINIORESPONSAVEL` | `IDFINDOMINIO, IDGLOFILIAL, IDFINDOMINIOPAI` | 0 | `DominioResponsavel` | E-Domínio |
| `FINDOMINIOUSUARIO` | `IDFINDOMINIO, IDGLOUSUARIO, IDGLOFILIAL` | 2 | `DominioUsuario` | E-Domínio |
| `FINDOCUMENTO` | `IDFINDOCUMENTO, IDGLOFILIAL` | 0 | `Documento` | E-Documento |
| `FINDOCUMENTOPARCELA` | `IDGLOFILIAL, IDFINDOCUMENTOPARCELA` ⚠️filial 1º | 0 | `DocumentoParcela` | E-Documento |
| `FINDOCUMENTOMOVTO` | `IDFINDOCUMENTOMOVTO, IDGLOFILIALMOVTO` | 0 | `DocumentoMovto` | E-Documento |
| `FINDOCUMENTOPARCELAIMAGEM` | `IDFINDOCUMENTOPARCELA, IDGLOFILIAL` | 0 | `DocumentoParcelaImage` | E-Documento |
| `FINDOCUMENTOTRIBUTO` | `IDFINDOCUMENTO, IDGLOFILIAL, IDGLOTRIBUTO` | 0 | `DocumentoTributo` | E-Documento |
| `FINDOCUMENTOCARTAO` | `IDFINDOCUMENTO, IDGLOFILIAL` | 0 | `DocumentoCartao` | E-Documento |
| `FINDOCUMENTOCOMISSIONADO` | `IDFINDOCUMENTO, IDGLOCOMISSIONADO, IDGLOFILIAL` | 0 | `DocumentoComissionado` | E-Documento |
| `FINDOCTOITEMFINANCEIRO` | `IDFINDOCTOITEMFINANCEIRO, IDGLOFILIAL` | 0 | `DoctoItemFinanceiro` | E-Documento |
| `FINDOCTOMOVTOITEMFINANCEIRO` | `IDFINITEMFINANCEIRO, IDFINDOCUMENTOMOVTO, IDGLOFILIALMOVTO` | 0 | `DoctoMovtoItemFinanceiro` | E-Documento |
| `FINDOCTOCANCELADO` | `IDFINDOCTOCANCELADO, IDGLOFILIAL` | 0 | `DoctoCancelado` | E-Documento |
| `FINDOCTOCANCELADOPARCELA` | `IDFINDOCTOCANCELADOPARCELA, IDGLOFILIAL` | 0 | `DoctoCanceladoParcela` | E-Documento |
| `FINITEMFINANCEIRO` | `IDFINITEMFINANCEIRO` (global) | 6 | `ItemFinanceiro` | E-Movimento |
| `FINITEMFINANCEIROOPERACAO` | `IDGLOOPERACAO, IDGLOFILIAL, IDFINITEMFINANCEIRO` | 18 | `ItemFinanceiroOperacao` | E-Movimento |
| `FINMOVIMENTO` | `IDFINMOVIMENTO, IDGLOFILIAL` | 0 | `MovimentoFinanceiro` | E-Movimento |
| `FINMOVIMENTORATEIO` | `IDFINMOVIMENTORATEIO, IDGLOFILIAL` | 0 | `MovtoFinanceiroRateio` (base `RateioMovtoItem` → MOD-02, CLR-01) | E-Movimento |
| `FINLIQUIDACAOESTORNO` | `IDFINLIQUIDACAOESTORNO, IDGLOFILIAL` | 0 | `LiquidacaoEstorno` | E-Estorno |
| `FINLIQUIDACAOESTORNOFORMAPAGTO` | `IDFINLIQUIDACAOESTORNOFORMAPAGTO, IDGLOFILIAL` | 0 | `LiquidacaoEstornoFormaPagto` | E-Estorno |
| `FINLIQUIDACAOFORMAMOVIMENTO` | `IDFINLIQUIDACAOESTORNOFORMAPAGTO, IDMOVIMENTO, IDGLOFILIAL` | 0 | `LiquidacaoFormaMovimento` | E-Liquidação |
| `FINREVERSAO` | `IDFINREVERSAO, IDGLOFILIAL` | 0 | `Reversao` | E-Reversão |
| `FINREVERSAODOCTOPARCELA` | `IDFINREVERSAODOCTOPARCELA, IDGLOFILIAL` | 0 | `ReversaoDoctoParcela` | E-Reversão |
| `FINREVERSAOITEMFINANCEIRO` | `IDFINREVERSAOITEMFINANCEIRO, IDGLOFILIAL` | 0 | `ReversaoItemFinanceiro` | E-Reversão |
| `FINCHEQUERECEBIDO` | `IDFINCHEQUERECEBIDO, IDGLOFILIAL` | 0 | `ChequeRecebido` | E-Cheques |
| `FINCHEQUERECEBIDOMOVTO` | `IDGLOFILIAL, IDFINCHEQUERECEBIDOMOVTO` ⚠️filial 1º | 0 | `ChequeRecebidoMovto` | E-Cheques |
| `FINCHEQUEEMITIDOMOVTO` | `IDFINCHEQUEEMITIDOMOVTO, IDGLOFILIAL` | 0 | `ChequeEmitidoMovto` | E-Cheques |
| `FINTALAOCHEQUE` | `IDFINCAIXABANCO, IDGLOFILIAL, CHEQUE` | 0 | `TalaoCheque` | E-Cheques |
| `FINMOTIVODEVOLUCAOCHEQUE` | `IDFINMOTIVODEVOLUCAOCHEQUE` (global) | 38 | `MotivoDevolucaoCheque` | E-Cheques |
| `FINADIANTAMENTO` | `IDFINADIANTAMENTO, IDGLOFILIAL` | 0 | `Adiantamento` | E-Adiantamentos |
| `FINADTOACERTO` | `IDFINADTOACERTO, IDGLOFILIAL` | 0 | `AdtoAcerto` | E-Adiantamentos |
| `FINADTOACERTODISTRIBUICAO` | `IDFINADTOACERTODISTRIBUICAO, IDGLOFILIAL` | 0 | `AdtoAcertoDistribuicao` | E-Adiantamentos |
| `FINADTOACERTOMOVTO` | `IDFINADTOACERTOMOVTO, IDGLOFILIAL` | 0 | `AdtoAcertoMovto` | E-Adiantamentos |
| `FINDRE` | `IDFINDRE, IDGLOFILIAL` | 1 | `DRE` | E-DRE |
| `FINDRETITULO` | `IDFINDRETITULO, IDGLOFILIAL` | 41 | `DRETitulo` | E-DRE |
| `FINDRETITULOCLASSE` | `IDFINDRETITULO, IDGLOFILIAL, IDFINCLASSE` | 53 | `DRETituloClasse` | E-DRE |
| `FINDRETITULOOPERACAO` | `IDFINDRETITULO, IDGLOFILIAL, IDFINDRETITULOCALCULO` | 6 | `DRETituloOperacao` | E-DRE |
| `FINPROGRAMACAOCOBRANCA` | `IDFINPROGRAMACAOCOBRANCA, IDGLOFILIAL` | 0 | `ProgramacaoCobranca` | E-Cobrança |
| `FINPROGRAMACAOCOBRANCAPARCELA` | `IDFINPROGRAMACAOCOBRANCAPARCELA, IDGLOFILIAL` | 0 | `ProgramacaoCobrancaParcela` | E-Cobrança |
| `FINTRANSACAOFILIAL` | `IDFINTRANSACAOFILIAL` (global) | 0 | `TransacaoFilial` | E-TransacaoFilial |
| `FINTRANSACAOFINANCEIRA` | `IDFINTRANSACAOFINANCEIRA` (global) | 0 | `TransacaoFinanceira` | E-TransacaoFilial |
| `FINFILIALMOVTO` | `IDFINFILIALMOVTO` (global) | 0 | `FilialMovimento` | E-TransacaoFilial |
| `FINCNAB`, `FINCNABDETALHE` | `..., IDGLOFILIAL` | 0 | armazenamento CNAB | **E14** |

**Tabelas presentes no banco fora do inventário da spec** (avaliar no épico correspondente):
`FINDOCTOITEMFINANCEIROTMP` (temp — provável staging, não migrar como entidade),
`FINFORMAPAGTOCONDICAOPAGTO` (PK `IDGLOCONDICAOPAGAMENTO, IDGLOFORMAPAGAMENTO, IDTIPOPARCELA`
— vínculo forma×condição, pode ser MOD-02), `FINPRIORIDADEPAGAMENTO` (cadastro simples),
`FINRATEIO` / `FINCLASSE` (**pertencem a `acesso.global` / MOD-02** — `Classe.cs` está lá;
para o MOD-05 são `int` lógico: `IdFinClasse`).

---

## 3. Detalhamento de colunas — épicos iniciais (E-Caixa, E-Domínio, E-Documento núcleo)

> Formato: `coluna física` · tipo SQL · `NULL?` · → `Propriedade C#` : tipo .NET · nota.
> Auditoria abreviada como **[audit-parcial]** = `IDGLOUSUARIOINCLUSAO int NULL`,
> `DATAINCLUSAO/HORAINCLUSAO datetime NULL`, `IDGLOUSUARIOALTERACAO int NULL`,
> `DATAALTERACAO/HORAALTERACAO datetime NULL` (confirmar sempre no extract).

### 3.1 `CaixaBanco` ← `FINCAIXABANCO`
| Coluna | SQL | NULL | Propriedade | Tipo .NET |
| :--- | :--- | :--- | :--- | :--- |
| `IDFINCAIXABANCO` | int | NO | `IdCaixaBanco` (PK1) | `int` `.ValueGeneratedNever()` |
| `IDGLOFILIAL` | int | NO | `IdFilial` (PK2) | `int` |
| `DESCRICAO` | varchar(100) | NO | `Descricao` | `string` |
| `IDTIPOCONTA` | int | NO | `IdTipoConta` | `enum TipoConta` |
| `ATIVO` | smallint | NO | `Ativo` | `bool` |
| `ENTRAFLUXOCAIXA` | smallint | NO | `EntraFluxoCaixa` | `bool` |
| `ULTIMADATACONFERIDA` | datetime | YES | `UltimaDataConferida` | `DateTime?` |
| `SALDO` | numeric(23,8) | YES | `Saldo` | `decimal?` |
| `CONTACONTABIL` | varchar(20) | YES | `ContaContabil` | `string?` |
| `IDCONPLANOCONTABIL` | int | YES | `IdPlanoContabil` | `int?` (cross-módulo Contábil) |
| `IDTIPOCONTACAIXA` | int | YES | `IdTipoContaCaixa` | `int?` / enum |
| _[audit-parcial]_ | | YES | `IdUsuarioInclusao`, `DataInclusao`, `HoraInclusao`, `IdUsuarioAlteracao`, `DataAlteracao`, `HoraAlteracao` | `int?` / `DateTime?` |

### 3.2 `ContaBancaria` ← `FINCONTABANCARIA` (55 colunas; 1:1 com `CaixaBanco` via PK compartilhada)
- **PK = FK:** `(IDFINCAIXABANCO, IDGLOFILIAL)` → `HasOne(cb).WithOne().HasForeignKey<ContaBancaria>(x => new { x.IdCaixaBanco, x.IdFilial })`.
- **Núcleo (E-Caixa):** `IDGLOAGENCIA int NO`, `TITULAR varchar(50) YES`, `NUMEROCONTA varchar(15) NO`, `DIGITOCONTA varchar(2) YES`, `LIMITE/CREDITOPENDENTE/DEBITOPENDENTE/CHEQUEPENDENTE numeric(23,8) YES`, `IDTIPOCONTABANCARIA int NO`, `PERMITEEMITIRCHEQUE smallint NO`, `CONTATERCEIRO smallint NO`, `IDFINCONTABANCARIAVINCULADA int YES`, `IDGLOINSTITUICAOFINANCEIRA int YES`, `CPFCNPJ varchar(14) YES`, _[audit-parcial]_.
- **Integração bancária (E14) — mesma tabela, comportamento separado:** `TIPOCARTEIRA`, `ULTIMONOSSONUMERO numeric(17,2)`, `POSTOCOBRANCA`, `ACEITE`, `ESPECIEDOCTO`, `LOCALPAGAMENTO`, `IDTIPOARQUIVOREMESSARETORNO`, `IDFINITEMFINANCEIRO{MULTA,DESCONTO,JUROS,ABATIMENTO,ACRECIMOS}`, `DIASPARAPROTESTO`, `OUTRASINSTRUCOES1/2`, `REGISTROCOBRANCA`, `DIRETORIOARQUIVO{RETORNO,REMESSA}`, `DATAULTIMOENVIO`, `QUANTIDADEULTIMOENVIO`, `IDENTIFICACAOEMPRESA`, `NUMEROCONVENIO`, `ULTIMONUMEROARQUIVO`, `GERABOLETO/GERAREMESSA/PROCESSARETORNO smallint NO`, `CODIGOCARTEIRAREMESSA`, `ENVIARSPED smallint NO`, `BOLETOBENEFICIARIODIFERENTE/BOLETOSACADOAVALISTA smallint NO` + `IDGLOENTIDADE{BENEFICIARIOBOLETO,SACADOAVALISTABOLETO} int YES`, `ULTIMONUMEROARQUIVO int YES`.
- A entidade `ContaBancaria` **carrega todas as 55 propriedades** (Regra 4 — não remover campos); só o *comportamento* que usa as colunas de integração vive em serviços de E14.

### 3.3 `SaldoCaixaBanco` ← `FINSALDOCAIXABANCO`
PK `(IDFINCAIXABANCO, IDGLOFILIAL, DATASALDO)`. Colunas: `SALDOANTERIOR`, `TOTALDEBITO`,
`TOTALCREDITO`, `SALDOANTERIORCONCILIADO`, `TOTALDEBITOCONCILIADO`, `TOTALCREDITOCONCILIADO`
(todas `numeric(23,8) NULL` → `decimal?`), `CONFERIDO smallint NO` → `bool`.

### 3.4 `Cobrador` ← `FINCOBRADOR`
PK `(IDFINCOBRADOR, IDGLOFILIAL)`. `IDGLOENTIDADE int NO`, `NOME varchar(100) NO`,
`ATIVO smallint NO` → `bool`, `IDGLOUSUARIO int YES`, `IDGLOMEIOCONTATO int YES`,
_[audit-parcial]_.

### 3.5 `Dominio` ← `FINDOMINIO`
PK `(IDFINDOMINIO, IDGLOFILIAL)`. **`IDFINCAIXABANCO int NO`** → `Dominio` depende de
`CaixaBanco` (ver `plan.md §3` — reordena épicos). `IDFINDOMINIOPERIODO int YES` = ponteiro
para o período aberto atual. `DESCRICAO varchar(100) NO`; flags de permissão
`ABRIRPERIODO/FECHARPERIODO/ABRIROUTROSPERIODOS/FECHAROUTROSPERIODOS/MOVIMENTOBANCO/CONSULTATODOSPERIODOS/ATIVO/TESOURARIA smallint NO`
→ `bool`; `SALDO/SALDODINHEIRO/SALDOCHEQUERECEBIDO/SALDOCARTAO numeric(23,8) YES` →
`decimal?`; _[audit-parcial]_.

### 3.6 `DominioPeriodo` ← `FINDOMINIOPERIODO`
PK `(IDFINDOMINIOPERIODO, IDGLOFILIAL)`. `IDFINDOMINIO int NO`,
`IDGLOUSUARIOFECHAMENTO int YES`, `DATAHORAABERTURA datetime NO`,
`DATAHORAFECHAMENTO/DATAHORAFECHAMENTOTESOURARIA datetime YES`,
**+ colunas redundantes** `DATAABERTURA datetime NO`,
`DATAFECHAMENTO/DATAFECHAMENTOTESOURARIA datetime YES` (legado — manter, Regra 4),
_[audit-parcial]_.

### 3.7 `DominioPeriodoFormaPagto` ← `FINDOMINIOPERIODOFORMAPAGTO`
PK `(IDFINDOMINIOPERIODOFORMAPAGTO, IDGLOFILIAL)`. `IDFINDOMINIOPERIODO int NO`,
`IDGLOFORMAPAGAMENTO int NO`, `IDPROCESSOORIGEM int NO` → `enum ProcessoOrigem`,
`CREDITO/DEBITO numeric(23,8) YES`, `DATA datetime NO`, **`HORA varchar(8) NO`** →
`string` (não `DateTime`), `IDORIGEM int NO`. Sem colunas de auditoria.

### 3.8 `Documento` ← `FINDOCUMENTO`
PK `(IDFINDOCUMENTO, IDGLOFILIAL)`. Mapa (nome físico → propriedade):
`IDGLOTIPODOCUMENTO`→`IdTipoDocumento` (int, cross-módulo), `NUMERODOCUMENTO varchar(50) YES`,
`IDGLOOPERACAO`→`IdOperacao`, `IDSITUACAO int NO`→`enum SituacaoDocumento`,
`IDGLOCONDICAOPAGAMENTO`, `IDGLOINDICEECONOMICO`, `IDGLOINDICECONVERSAO`,
`DATAEMISSAO datetime NO`, `DATAINCLUSAO datetime NO`, `VALOR numeric(23,8) NO`,
`VALORLIQUIDADO/VALORREVERTIDO/VALORPARCELADO/VALORCONVERTIDO/VALORCANCELADO numeric(23,8) YES`,
`IDGLOENTIDADE int NO`, `IDRECEBERPAGAR int NO`→`enum PagarReceberTipo`,
`IDPROCESSOORIGEM int YES`→`enum? ProcessoOrigem`, `IDORIGEM int YES`,
`HISTORICO text YES`→`string?` `HasColumnType("text")`, `IDGLOUSUARIOLIBERACAO int YES`,
`DATALIBERACAO/HORALIBERACAO datetime YES`, `DOCUMENTOLIBERADO smallint NO`→`bool`,
`NUMEROCEDENTE varchar(20) YES`, `IDGLOUSUARIOINCLUSAO int YES`,
`IDGLOUSUARIOALTERACAO int YES`, **`DATAALTERACAO datetime NO`**,
**`HORAINCLUSAO datetime NO`**, **`HORAALTERACAO datetime NO`** (⚠️ nulidade
não-uniforme — não assumir padrão).

### 3.9 `DocumentoParcela` ← `FINDOCUMENTOPARCELA` (55 colunas)
PK **`(IDGLOFILIAL, IDFINDOCUMENTOPARCELA)`** (filial 1º). `IDFINDOCUMENTO int NO`,
`NUMEROPARCELA int NO`, `ATIVO smallint NO`→`bool`, `IDSITUACAO int NO`→`enum SituacaoParcela`,
`IDTIPOREGISTRO int NO`→enum, `IDGLOPORTADOR int NO`, `DATAVENCIMENTO datetime NO`,
`VALOR numeric(23,8) NO`, `VALORLIQUIDADO/VALORREVERTIDO/VALORCONVERTIDO/VALORCANCELADO numeric(23,8) YES`,
`ITEMFINANCEIRO/ITEMFINANCEIROCONVERTIDO numeric(23,8) YES` (juros/multa/desconto acumulado),
`IDGLOINDICEECONOMICO int YES`, `DATAULTIMALIQUIDACAO datetime YES`, `AVISTA smallint YES`→`bool?`,
`IDGLOFORMACOBRANCA int NO`, `IDFINCAIXABANCO int YES`, `IDFINCOBRADOR int YES`,
`DATACOBRANCA/HORACOBRANCA datetime YES`, `IDGLOUSUARIOAPROVACAO int YES`,
`DATAAPROVACAO/HORAAPROVACAO datetime YES`, `DOCUMENTOAPROVADO smallint NO`→`bool`.
**Colunas de boleto/CNAB (E14, mesma tabela):** `BOLETO smallint NO`, `NOSSONUMERO varchar(20) YES`,
`CODIGOBARRA varchar(60) YES`, `DIGITONOSSONUMERO varchar(1) YES`, `IDGLOBANCO int YES`,
`NUMEROAGENCIA/DIGITOAGENCIA/CONTACORRENTE/DIGITOCONTA/FAVORECIDO/TIPOCONTA/CPFCNPJFAVORECIDO/COMPLEMENTO varchar YES`,
`IDFINPROGRAMACAOCOBRANCAULTIMO int YES`, `LINHADIGITAVEL/CODIGOBARRAFORMATADO/NOSSONUMEROFORMATADO/AGENCIACODIGOCEDENTEFORMATADO varchar YES`,
`NOMEARQUIVOREMESSA varchar(20) YES`, `DATAREMESSA/HORAREMESSA/DATAPROCESSAMENTOBOLETO datetime YES`,
`INSTRUCAOCOMPLEMENTAR varchar(100) YES`, `MENSAGEMERROENVIOEMAILBOLETO text YES`,
`IDBOLETOENVIADOEMAIL int YES`, `NOSSONUMEROSEQUENCIAL numeric(17,2) YES`, `REMESSA smallint YES`→`bool?`.

### 3.10 `ItemFinanceiro` ← `FINITEMFINANCEIRO` (**sem `IDGLOFILIAL`** — cadastro global)
PK `IDFINITEMFINANCEIRO`. `DESCRICAO varchar(100) NO`, `IDAPLICAR/IDAPLICACAO/IDNATUREZA/IDCALCULO int NO`
→ enums, `IDGLOINDICEECONOMICO int YES`, `IDGLOOPERACAO int YES`, `DIAS int YES`,
`VALOR numeric(23,8) YES`, `PADRAO/ATIVO smallint NO`→`bool`.

### 3.11 `FinClasse` (referência cross-módulo — **não é entidade do MOD-05**)
`FINCLASSE` (`Classe.cs` em `acesso.global`): PK `(IDFINCLASSE, IDGLOFILIAL)`, hierárquica
(`IDFINCLASSEPAI`), `IDTIPOCALCULODRE`, `SALDOECONOMICO/SALDOFINANCEIRO`. No MOD-05:
`DRETituloClasse.IdFinClasse` e rateios usam `int` lógico.

---

## 4. Detalhamento dos demais épicos

Feito **por épico, na tarefa `analysis` correspondente do `/implement`**, sempre contra
[`legacy-schema/fin_columns.txt`](./legacy-schema/fin_columns.txt) (colunas) e
[`legacy-schema/fin_meta.txt`](./legacy-schema/fin_meta.txt) (PKs). `sdd-analyze` V5
verifica cobertura de 100% das colunas de cada tabela do escopo contra este extract.

Tabelas ainda a detalhar: `FINMOVIMENTO`, `FINMOVIMENTORATEIO`, `FINDOCUMENTOMOVTO`,
`FINDOCTOITEMFINANCEIRO`, `FINDOCTOMOVTOITEMFINANCEIRO`, `FINDOCUMENTOTRIBUTO`,
`FINDOCUMENTOCARTAO`, `FINDOCUMENTOCOMISSIONADO`, `FINDOCTOCANCELADO(+PARCELA)`,
`FINITEMFINANCEIROOPERACAO`, `FINLIQUIDACAOESTORNO(+FORMAPAGTO)`, `FINLIQUIDACAOFORMAMOVIMENTO`,
`FINREVERSAO(+DOCTOPARCELA/+ITEMFINANCEIRO)`, `FINCHEQUERECEBIDO(+MOVTO)`,
`FINCHEQUEEMITIDOMOVTO`, `FINTALAOCHEQUE`, `FINMOTIVODEVOLUCAOCHEQUE`,
`FINADIANTAMENTO`, `FINADTOACERTO(+DISTRIBUICAO/+MOVTO)`, `FINDRE(+TITULO/+TITULOCLASSE/+TITULOOPERACAO)`,
`FINPROJECAOFLUXOCAIXA(+LACTO)` (⚠️ não apareceu no extract `Fin%` — confirmar nome real),
`FINPROGRAMACAOCOBRANCA(+PARCELA)`, `FINTRANSACAOFILIAL/FINTRANSACAOFINANCEIRA/FINFILIALMOVTO`,
`FINDOMINIOPERIODOLANCTO`, `FINDOMINIOPERIODOFECHAMENTO`, `FINDOMINIORESPONSAVEL`,
`FINDOMINIOUSUARIO`, `FINCAIXABANCOUSUARIO`, `FINSALDORATEIO`, `FINCNAB(+DETALHE)`.

> **Achado (risco):** o banco de dev tem **59 tabelas `Fin%`**. Estão **ausentes**:
> `FINPROJECAOFLUXOCAIXA`, `FINPROJECAOFLUXOCAIXALACTO` (épico E-DRE/Projeção) e
> `FINLOGDOMINIOPERIODO` (= `[TableName("FinLogDominioPeriodo")]` de `DominioPeriodoLog`).
> Este banco **não contém 100% do schema do MOD-05** — a tarefa `analysis` de cada épico
> deve reconfirmar a existência da tabela e, se ausente, pedir um dump de schema de
> produção. Registrado em `plan.md §7`.
>
> Tabelas presentes e ainda não classificadas: `FINRATEIO` (rateio — MOD-02),
> `FINCLASSE` (MOD-02), `FINPRIORIDADEPAGAMENTO`, `FINFORMAPAGTOCONDICAOPAGTO`,
> `FINCNAB`/`FINCNABDETALHE` (E14), `FINDOCTOITEMFINANCEIROTMP` (staging — não migrar).
