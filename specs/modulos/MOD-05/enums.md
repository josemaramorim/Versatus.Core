# Inventário de Enums — MOD-05 Gestão Financeira

> **Versão:** 1.0 | **Data:** 2026-09-04 | **SDD — tarefa E0-T01** (`tipo: analysis`)
> **Base:** [`research.md §2`](./research.md) · [`data-model.md`](./data-model.md) ·
> [`plan.md §2`](./plan.md) · [`constitution.md`](../../memory/constitution.md) Artigo V.5 / II.6
>
> **Fontes legadas:**
> - `projeto_tag_1906/geral/tipoenumerado.cs` (7762 linhas) — família **persistida**
>   (`namespace Projeto.Geral.Enumerado`, decorada com `[TipoEnumerado(idPai)]`).
> - `projeto_tag_1906/geral/TipoEnumeradoObjeto.cs` (2007 linhas) — família
>   **não-persistida** (`namespace Projeto.Geral.EnumeradoObjeto`, sem atributo).
> - `projeto_tag_1906/geral/EnumDescriptor.cs` — leitura do atributo por reflexão.
> - Uso confirmado por `grep` nas 184 classes de
>   `projeto_tag_1906/servidor/objeto de negócio/gestao.financeira/`.

---

## 1. As duas famílias (ver `research.md §2`)

| | **Persistido** | **Não-persistido** |
| :--- | :--- | :--- |
| Arquivo / namespace | `tipoenumerado.cs` / `Projeto.Geral.Enumerado` | `TipoEnumeradoObjeto.cs` / `Projeto.Geral.EnumeradoObjeto` |
| Atributo | `[TipoEnumerado(idPai)]` (alguns sem atributo — ver Tabela A) | nenhum |
| Persistência | **valor inteiro gravado** em coluna `ID*` do banco | **nunca** vai para o banco — comportamento / UI / retorno / máquina de estados em memória |
| Valores | inteiros explícitos, ancorados em `GloTipoEnumerado` — **imutáveis** | podem não ter valor explícito (0,1,2 por posição); alguns `[Flags]` |
| Label | dinâmico via `GloTipoEnumerado` (`AcessoGlobalFactory.GetLabelEnum`) | estático / no código |
| Destino .NET | `Versatus.SharedKernel/Enums/` + `HasConversion<int>()` na coluna (no épico dono da entidade) | `Versatus.SharedKernel/Enums/` — **só código**, nunca coluna |

**Misto:** enum que mora em `EnumeradoObjeto` (sem atributo) mas cujos valores explícitos
aparecem em dados/config — tratado como não-persistido, sinalizado na Observação.

**Regra E0-T02:** criar todos abaixo em `Versatus.SharedKernel/Enums/`, **valor inteiro
preservado**, `[Flags]` mantido, comentário de origem (`Enumerado`/`EnumeradoObjeto` +
`idPai`). O `HasConversion<int>()` dos persistidos **não** é feito em E0 — é do épico que
mapeia a entidade que usa a coluna.

---

## 2. Tabela A — Enums **persistidos** (`tipoenumerado.cs`)

| Enum | idPai | Membros (`nome=valor`) | Coluna(s) `ID*` | `[Flags]` | Observação |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `SituacaoDocumento` | 82 | `Aberto=83, Liquidado=85, LiquidadoParcial=221, Cancelado=281` | `FINDOCUMENTO.IDSITUACAO`, **`FINDOCTOPARCELA.IDSITUACAO`**, `FINDOCTOMOVTOITEMFINANCEIRO`… | não | `data-model.md` cita "`SituacaoParcela`" — **esse enum não existe**; a parcela usa `SituacaoDocumento` (ver `DocumentoParcela.RetornarSituacaoParcela()`). |
| `SituacaoMovimento` | 287 | `Normal=288, Cancelado=289` | `FINMOVIMENTO.IDSITUACAO` | não | |
| `ChequeRecebidoSituacao` | 215 | `Aberto=216, Devolvido=217, Baixado=218, Negociado=235, Cancelado=286, Repassado=501, DevolucaoRepasse=605, Sacado=739` | `FINCHEQUERECEBIDO.IDSITUACAO`, `FINCHEQUERECEBIDOMOVTO` | não | Máquina de estados do cheque recebido (E9). |
| `SituacaoTalaoCheque` | 187 | `Disponivel=188, Cancelado=189, Emitido=190, Devolvido=498, Negociado=499, Sacado=740` | `FINTALAOCHEQUE.IDSITUACAO` | não | E9. |
| `SituacaoComissaoLancto` | 431 | `Previsto=432, Efetivado=433, Revertido=434, Cancelado=435, EfetivadoParcial=532` | `FINDOCUMENTOCOMISSIONADO`… | não | Usado em `Documento`/`Reversao`. |
| `SituacaoDistribuicao` | 386 | `Pendente=387, Atendido=388, AtendidoParcial=389, Cancelado=436, Devolvido=497` | `FINADTOACERTODISTRIBUICAO` | não | E10 (a confirmar coluna no `analysis` do épico). |
| `ProcessoOrigem` | 129 | `Venda=130, Compra=131, Liquidacao=132, Documento=133, ChequeRecebido=134, MovimentoChequeRecebido=135, CaixaBanco=196, DominioPeriodoLancto=292, Estorno=296, EntidadeMovimento=295, Reversao=294, MovimentoEstoque=335, LancamentoComissao=420, FechamentoComissao=421, MovimentoRateio=440, CancelamentoVendaCompra=469, Faturamento=476, ImplantacaoEstoque=478, DevolucaoVendaCompra=479, EstornoDevolucao=489, MovimentoChequeEmitido=500, AtendimentoRequisicao=581, DevolucaoRequisicao=582, FechamentoFolha=583, MovimentoConsignacao=627, CancelamentoDocumentoFinanceiro=644, OrdemServico=662, Adiantamento=691, AcertoAdiantamento=692, OrdemExpedicao=694, MovimentoVeiculo=695, Abastecimento=696, DistribuicaoAcerto=700, FechamentoOsFrota=716, CancelamentoOsFrota=717, RomaneioArmazem=718, AtendimentoRequisicaoObra=720, AtendimentoDevolucaoObra=721, DespesaVeiculoFrota=878, MovimentoEstoqueFiscal=882, DespesaVeiculoGaragem=1438, CancelamentoMovtoFinanceiroFrota=905, CancelamentoMovtoFinanceiroGaragem=906, ClienteFilial=1106, LiquidacaoContraPartida=1195, TransacaoFilial=1237, TicketPesagem=1297, Contrato=1382, Matricula=1406, CancelamentoContrato=1422, Transporte=1491, MovimentoProducao=1519, MovimentoItemProducao=1520, CancelamentoMovimentoProducao=1521, CancelamentoMovimentoItemProducao=1522, MDFeDocumento=1576, CancelamentoTransporte=1776, MovimentoContrato=1812, NotaFiscalServico=1850, AjusteMovimentoContratoQuantidade=1854, AjusteMovimentoContratoFinanceiro=1855, ICMSSubstituicaoEstoque=1906` | `FINMOVIMENTO.IDPROCESSOORIGEM` (NO), `FINDOCUMENTO.IDPROCESSOORIGEM` (YES), `FINDOCTOMOVTO`, `FINCHEQUERECEBIDOMOVTO`, `FINPROJECAOFLUXOCAIXALACTO` | não | Enum mais transversal do módulo (43 classes). `ProjecaoFluxoCaixa=723` está **comentado** no legado — não incluir. Lista completa acima = `tipoenumerado.cs:587`. |
| `PagarReceberTipo` | 171 | `Pagar=172, Receber=173, MovimentoCartao=1192` | `FINDOCTOMOVTO.IDRECEBERPAGAR`, `FINDOCUMENTO`, `FINPROJECAOFLUXOCAIXA` | não | |
| `NaturezaTipo` | 29 | `Credora=30, Devedora=31` | `FINITEMFINANCEIRO.IDNATUREZA`, rateio, DRE | não | 38 classes. Não confundir com FK `IDGLONATUREZA`. |
| `FormaPagtoTipo` | 121 | `Dinheiro=122, ChequeEmpresa=123, ChequeCliente=124, CartaoCredito=125, CartaoDebito=126, ParcelamentoProprio=127, ParcelamentoFinanceira=128, Credito=236, CreditoPortador=237, Deposito=256, Outros=293, Abatimento=483, PixEstatico=1962, PixDinamico=1963` | `FINFORMAPAGTOMOV`, `FORMAPAGAMENTOMOV`, `FINFORMAMOVINFO*` | não | E5/E6. |
| `RegimeRateioTipo` | 184 | `Economico=185, Financeiro=186` | `FINMOVTOFINANCEIRORATEIO`, `FINCONTRAPARTIDARATEIO*`, DRE | não | 16 classes. |
| `ContaTipo` | 174 | `Caixa=175, Banco=176` | `FINCAIXABANCO.IDTIPOCONTA` | não | `data-model.md` diz "`enum TipoConta`" — nome real é **`ContaTipo`**. |
| `TipoContaCaixa` | 1481 | `Normal=1482, Cofre=1483` | `FINCAIXABANCO.IDTIPOCONTACAIXA` (YES) | não | `0` quando a conta é Banco. |
| `TipoContaBancaria` | 1478 | `ContaCorrente=1479, Investimento=1480` | `FINCONTABANCARIA.IDTIPOCONTABANCARIA` (NO) | não | Nome legado do membro `ContaBancariaTipo`. |
| `ContaFinanceiroTipo` | 177 | `CentroCusto=178, Classe=179, Projeto=180, PlanoConta=181, Caixa=182, Banco=183` | tipo de destino de rateio / `FINAPLICACAOITEMFIN` | não | **Não** é o tipo de conta bancária. |
| `TipoDocumentoMovimento` | 253 | `Liquidacao=254, Estorno=255` | `FINDOCTOMOVTO.IDTIPODOCUMENTO` | não | E6/E7. |
| `TipoFormaLancamento` | 258 | `FormaPagamento=259, Troco=260` | `FINLIQUIDACAOFORMAMOVIMENTO`, `FINLIQESTORNOFORMAPAGTO` | não | |
| `RegistroDocumentoTipo` | — (sem atributo) | `Normal=223, JuroCapitalizado=224, BoletoAgrupado=225, Reversao=226, RevertidoAgrupado=227, Descontado=228` | `FINDOCTOPARCELA.IDTIPOREGISTRO` (NO) | não | **Sem `[TipoEnumerado]`** — mas valor gravado. Label sem `GloTipoEnumerado`. |
| `CalculoItemFinanceiro` | 238 | `Somar=239, Subtrair=240, MultiplicarSomar=241, MultiplicarDiminuir=242, DividirSomar=243, DividirSubtrair=257, PercentualSomar=244, PercentualSubtrair=245` | `FINITEMFINANCEIRO.IDCALCULO` (NO) | não | Base dos cálculos de juros/multa/desconto (E4/E6 — golden `CALC-*`). |
| `CalculoItemFinanceiroTipo` | — (sem atributo) | `Simples=147, Composto=148` | `FINITEMFINANCEIRO` (a confirmar) | não | Sem atributo. |
| `ItemFinanceiroAplicar` | 246 | `NaoAplicar=247, DepoisVencimento=248, AntesVencimento=249` | `FINITEMFINANCEIRO.IDAPLICAR` (NO) | não | |
| `ItemFinanceiroAplicacao` | 250 | `ValorCalculado=251, ValorParcelado=252` | `FINITEMFINANCEIRO.IDAPLICACAO` (NO) | não | |
| `OperacaoConversao` | 265 | `Nenhuma=266, Multiplicacao=267, Divisao=268` | `FININDICECONVERSOR` / conversão por índice | não | E1/E3 (`CALC` de conversão). |
| `IndiceModoCorrecao` | 105 | `UsaValorDia=210, UsarDataAnterior=106, UsarDataPosterior=107` | `FININDICECONVERSOR` | não | |
| `IndiceTipoCorrecao` | 108 | `Diario=109, Mensal=110` | `FININDICECONVERSOR` | não | |
| `LanctoDominioPeriodoTipo` | 262 | `Abertura=263, Fechamento=264, Suprimento=290, Sangria=291` | `FINDOMINIOPERIODOLACTO` | não | E2. |
| `TipoManutencaoRateio` | 441 | `Financeiro=442, Estoque=443` | `FINMANUTENCAORATEIO.IDTIPOMANUTENCAORATEIO` | não | E5. |
| `CondicaoPagtoTipo` | 35 | `Parcelada=36, FaixaDias=37, Semanal=38` | `GLOCONDICAOPAGAMENTO.IDTIPO` (MOD-02) | não | Consumido na geração de parcelas (E4). |
| `ParcelamentoTipo` | 118 | `DiaFixo=119, DiasEntreParcela=120, DiasUteis=693` | condição de pagamento (MOD-02) | não | |
| `ParcelamentoArredondamento` | 45 | `Primeira=46, Ultima=47` | condição de pagamento (MOD-02) | não | |
| `VencimentoTipo` | 58 | `Normal=59, AntecipaDiaUtil=60, ProrrogaDiaUtil=61` | condição de pagamento / portador | não | Ajuste de vencimento por dia útil (E4). |
| `Disponibilidade` | 55 | `Pagamento=56, Recebimento=57, Ambas=101` | forma de pagamento / portador | não | |
| `AcaoBloqueio` | 408 | `Bloquear=409, Desbloquear=410` | operação de bloqueio de parcela/portador | não | |
| `TipoCartao` | 562 | `Debito=563, Credito=564` | `FINDOCUMENTOCARTAO` | não | |
| `TipoCalculoDRE` | 1150 | `MovimentoRateio=1151, Formula=1152, Avulso=1153, CustoVenda=1168, CustoVendaTipoProduto=1166, CustoVendaGrupoEstoque=1167` | `FINDRETITULO.IDTIPOCALCULODRE` | não | E11. |
| `OperacaoTipo` | — (sem atributo) | `Caixa=1, Banco=2, Pagar=3, Receber=4, LiquidacaoReceber=5, LiquidacaoPagar=6, ChequeRecebido=7, MovimentacaoEstoque=8, Venda=9, Compra=10, Folha=11, EntradaVenda=12, MovimentoCartao=13, LiquidacaoMovimentoCartao=14, SaidaCompra=15, MovimentoProducao=16, Transporte=17, Contrato=18, NotaFiscalServico=19` | não gravado diretamente — chave de roteamento de operação | não | **Provável não-persistido** apesar de estar em `tipoenumerado.cs` (valores 1..19, sem `[TipoEnumerado]`, sem `GloTipoEnumerado`). Classificar em definitivo no `analysis` de E1/E6 — se nenhuma coluna o grava, mover para Tabela B. |

---

## 3. Tabela B — Enums **não-persistidos** (`TipoEnumeradoObjeto.cs`)

| Enum | Membros (`nome=valor`) | `[Flags]` | Observação |
| :--- | :--- | :--- | :--- |
| `PeriodoStatus` | `Aberto=1, Fechado=2, NaoAplicavel=3, UsuarioSemPermissao=4, PerfilSemPermissao=5` | não | Retorno da checagem de período aberto (E2) — 14 classes. Não é a coluna de situação do período. |
| `StatusDominioFinanceiro` | `Normal=1, Abertura=2, AberturaPadrao=3, Fechamento=4, FechamentoPadrao=5, SuprimentoDinheiro=6, SuprimentoCheque=7, AtualizarDominio=8, AtualizarDominioPadrao=9` | não | Estado transitório do processamento de domínio/período (E2). |
| `SequencialTipo` | `Geral=230, Empresa=231, Filial=232` | não | **Misto** — em `EnumeradoObjeto`, mas valores explícitos coincidem com `TipoSequencial` (idPai 229, persistido). Usado por `GeradorSequencialService` (`research.md §3`) em 34 classes. Reusar de `Versatus.Framework` se já existir lá. |
| `TipoRateioItem` | `Nenhum=0, Classe=1, CentroCusto=2, Projeto=4` | **sim** (potência de 2) | Dimensão de rateio. |
| `TipoRateioItemValidacao` | `NaoValidar=0, ValidarPercentual=1, ValidarValor=2` (por posição) | não | |
| `TipoCalculoValorRateio` | `Valor=1, Percentual=2, NaoCalculo=3, NaoCalculoPercentual=4, ValorNaturezaInvertida=5, PercentualDevedor=6, PercentualNatureza=7, PercentualNaturezaInvertida=8` | não | Núcleo do `RateioContainer` (E0-T03). |
| `TipoSaldo` | `Inicial=0, Lancamento=1, Atual=2, Final=3` (por posição) | não | Cálculo de saldo de caixa/banco (E5 — golden `CALC`). |
| `TipoProcessoComissao` | `Venda=1, Documento=2, Liquidacao=3, Reversao=4, CancelamentoVenda=5, DevolucaoVenda=6, NotaServicoFiscal=7` | não | Roteamento de comissão em `Documento`/`Liquidacao`/`Reversao`. |
| `TipoHistoricoLiquidacaoEstorno` | `Parcela=1, ParcelaQtde=2, Entidade=3, EntidadeRazao=4, EntidadeNomeRazao=5, Vencimento=6` | não | Montagem de histórico textual (E6/E7). |
| `DominioDestinoTalaoCheque` | `Proprio=1, Padrao=2` | não | E9. |
| `AcaoLiberarDoctoPagar` | `Liberar=1, Cancelar=2` | não | Fluxo de aprovação de documento a pagar (E4). |
| `AcaoAprovarDoctoPagar` | `Aprovar=1, Cancelar=2` | não | idem. |
| `EntradaSaida` | `Entrada=1, Saida=2` | não | Sinal de movimento. |
| `TipoLactoEditor` | `Avulso=1, Cadastro=2` | não | Origem do lançamento (editor legado — checar necessidade no backend). |
| `TipoItemFinanceiroBoleto` | `Desconto=1, Juros=2, Multa=3` | não | **Escopo E14** (integração bancária) — criar só quando E14 for atacado. |

---

## 4. Tabela C — Classificação das colunas `ID*` de `data-model.md`

| Coluna | Classificação | Alvo |
| :--- | :--- | :--- |
| `IDSITUACAO` (FINDOCUMENTO/FINDOCTOPARCELA) | enum-persistido | `SituacaoDocumento` |
| `IDSITUACAO` (FINMOVIMENTO) | enum-persistido | `SituacaoMovimento` |
| `IDSITUACAO` (FINCHEQUERECEBIDO*) | enum-persistido | `ChequeRecebidoSituacao` |
| `IDSITUACAO` (FINTALAOCHEQUE) | enum-persistido | `SituacaoTalaoCheque` |
| `IDPROCESSOORIGEM` | enum-persistido | `ProcessoOrigem` |
| `IDRECEBERPAGAR` | enum-persistido | `PagarReceberTipo` |
| `IDNATUREZA` | enum-persistido | `NaturezaTipo` (≠ FK `IDGLONATUREZA`) |
| `IDTIPOCONTA` | enum-persistido | `ContaTipo` |
| `IDTIPOCONTACAIXA` | enum-persistido (anulável) | `TipoContaCaixa` |
| `IDTIPOCONTABANCARIA` | enum-persistido | `TipoContaBancaria` |
| `IDTIPOREGISTRO` | enum-persistido (sem `[TipoEnumerado]`) | `RegistroDocumentoTipo` |
| `IDTIPODOCUMENTO` (FINDOCTOMOVTO) | enum-persistido | `TipoDocumentoMovimento` |
| `IDCALCULO` | enum-persistido | `CalculoItemFinanceiro` |
| `IDAPLICAR` | enum-persistido | `ItemFinanceiroAplicar` |
| `IDAPLICACAO` | enum-persistido | `ItemFinanceiroAplicacao` |
| `IDTIPOCALCULODRE` | enum-persistido | `TipoCalculoDRE` |
| `IDTIPOMANUTENCAORATEIO` | enum-persistido | `TipoManutencaoRateio` |
| `IDGLOINDICEECONOMICO`, `IDGLOOPERACAO`, `IDGLOPORTADOR`, `IDGLOENTIDADE`, `IDGLOFORMAPAGAMENTO`, `IDGLOCONDICAOPAGAMENTO`, `IDGLOAGENCIA`, `IDGLOINSTITUICAOFINANCEIRA`, `IDGLONATUREZA`, `IDFINCLASSE*`, `IDFINITEMFINANCEIRO*`, `IDFINCAIXABANCO`, `IDFINCONTABANCARIA*` | **FK** (não enum) | tabela de domínio (MOD-02 ou MOD-05) |
| `IDORIGEM`, `IDTIPOCONTACAIXA` quando `0` | valor de negócio / sentinela | tratar no `analysis` do épico |
| Colunas de integração bancária (`IDTIPOARQUIVOREMESSARETORNO`, `IDFINITEMFINANCEIRO{MULTA,DESCONTO,JUROS,…}`) | **E14** | fora de escopo até o épico E14 |

---

## 5. Pendências / a resolver no `analysis` de cada épico

1. **`OperacaoTipo`** — confirmar se alguma coluna grava o valor (1..19). Se não, reclassificar para Tabela B.
2. **`SituacaoDistribuicao`, `SituacaoComissaoLancto`** — confirmar a coluna exata (`E10`, `E4/E8`).
3. **`CalculoItemFinanceiroTipo`** (`Simples`/`Composto`) — confirmar coluna em `FINITEMFINANCEIRO`.
4. Enums **finos por épico** ainda não catalogados aqui (ex.: `SelecaoDocumento.TipoAcao/TipoDocumento`, `ProjecaoFluxoCaixa.FiltroTipo`) — cada `E?-T01` acrescenta uma linha nas Tabelas A/B (`research.md §2`: "Extração fina por épico").
5. `data-model.md` linha 193 (`enum SituacaoParcela`) e linha 127 (`enum TipoConta`) devem ser corrigidas para `SituacaoDocumento` e `ContaTipo` na próxima revisão do `data-model.md`.

---

## 6. Rastreabilidade

- **Cobre:** RN-05 (enums de situação/tipo das RN-05-001..020) · suporte a `data-model.md` (Tabela C).
- **Alimenta:** `E0-T02` (criação dos enums no `SharedKernel`), `E0-T04` (golden de valor — ênfase nos persistidos), e o `HasConversion<int>()` nas tarefas `E?-T03`/`E?-T04` de mapeamento de cada épico.
