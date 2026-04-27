# SPEC — MOD-05: Gestão Financeira
## Módulo: servidor/objeto de negócio/gestao.financeira

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 4  
> **Status:** 📝 Rascunho  
> **Prioridade:** ALTA — contas a pagar/receber, caixa, bancos

---

## 1. Visão Geral do Módulo

O módulo **Gestão Financeira** controla contas a pagar e a receber, movimentações de caixa
e banco, cheques, adiantamentos, DRE e fluxo de caixa. É alimentado pelo Faturamento
(gera contas a receber) e pelas Compras (gera contas a pagar).

### Localização no legado
```
servidor/objeto de negócio/gestao.financeira/
```

### Dependências
- **Requer:** MOD-01, MOD-02 (Acesso Global), MOD-04 (Faturamento), MOD-06 (Compras)
- **É usado por:** Relatórios, Conciliação

---

## 2. Inventário de Classes por Grupo

### 2.1 Documento Financeiro — Centro do Módulo

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Documento.cs` | **101 KB** | **Entidade Central** | Documento financeiro (conta a pagar/receber) |
| `DocumentoFinanceiroBase.cs` | 41 KB | Base | Classe base de documentos financeiros |
| `DocumentoParcela.cs` | **71 KB** | Entidade | Parcela do documento (título) |
| `DocumentoParcelaLista.cs` | 10 KB | Lista | |
| `DocumentoParcelaManutencao.cs` | 25 KB | Operação | Manutenção de parcelas |
| `DocumentoParcelaImage.cs` | 3 KB | Relacionamento | Imagem/boleto |
| `DocumentoMovto.cs` | 25 KB | Entidade | Movimento do documento |
| `DocumentoMovtoLista.cs` | 3 KB | Lista | |
| `DocumentoManutencao.cs` | 13 KB | Operação | Manutenção do documento |
| `DoctoItemFinanceiro.cs` | 12 KB | Relacionamento | Item financeiro do documento |
| `DoctoItemFinanceiroLista.cs` | 4 KB | Lista | |
| `DoctoMovtoItemFinanceiro.cs` | 7 KB | Relacionamento | Movimento de item financeiro |
| `DoctoCancelado.cs` | 9 KB | Entidade | Documento cancelado |
| `DoctoCanceladoParcela.cs` | 5 KB | Relacionamento | Parcela cancelada |
| `DocumentoCartao.cs` | 5 KB | Relacionamento | Cartão no documento |
| `DocumentoComissionado.cs` | 9 KB | Relacionamento | Comissionado no documento |
| `DocumentoTributo.cs` | 11 KB | Relacionamento | Tributos do documento |

### 2.2 Liquidação

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Liquidacao.cs` | 25 KB | **Entidade Central** | Liquidação de título |
| `LiquidacaoEstorno.cs` | **97 KB** | Operação | Estorno de liquidação (ENORME) |
| `LiquidacaoEstornoFormaPagto.cs` | 42 KB | Relacionamento | Forma de pagamento do estorno |
| `LiquidacaoFormaMovimento.cs` | 4 KB | Relacionamento | Forma no movimento de liquidação |

### 2.3 Adiantamentos

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Adiantamento.cs` | 22 KB | Entidade | Adiantamento |
| `AdtoAcerto.cs` | **51 KB** | Operação | Acerto de adiantamento (GRANDE) |
| `AdtoAcertoDistribuicao.cs` | 14 KB | Relacionamento | Distribuição do acerto |
| `AdtoAcertoMovto.cs` | 17 KB | Relacionamento | Movimento do acerto |
| `AdtoLanctoEntidade.cs` | 3 KB | Relacionamento | Lançamento por entidade |

### 2.4 Domínio Financeiro (Período)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Dominio.cs` | **62 KB** | **Entidade Central** | Domínio financeiro (CaixaBanco) |
| `DominioPeriodo.cs` | 35 KB | Entidade | Período do domínio |
| `DominioPeriodoFechamento.cs` | 9 KB | Entidade | Fechamento do período |
| `DominioPeriodoFormaPagto.cs` | 14 KB | Relacionamento | Formas de pagamento do período |
| `DominioPeriodoLacto.cs` | 26 KB | Entidade | Lançamento do período |
| `DominioResponsavel.cs` | 10 KB | Relacionamento | Responsável pelo domínio |
| `DominioUsuario.cs` | 16 KB | Relacionamento | Usuários do domínio |

### 2.5 Caixa e Banco

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `CaixaBanco.cs` | **28 KB** | Entidade | Caixa ou conta bancária |
| `CaixaBancoUsuario.cs` | 10 KB | Relacionamento | Usuários do caixa/banco |
| `ContaBancaria.cs` | **57 KB** | Entidade | Conta bancária (GRANDE) |
| `SaldoCaixaBanco.cs` | 9 KB | Entidade | Saldo do caixa/banco |
| `SaldoRateio.cs` | 11 KB | Entidade | Saldo de rateio |
| `FechamentoCaixaBase.cs` | 13 KB | Base | Fechamento de caixa |

### 2.6 Movimentos Financeiros

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `MovimentoFinanceiro.cs` | **75 KB** | **Entidade Central** | Movimento financeiro |
| `MovimentoFinanceiroUpdate.cs` | 3 KB | Auxiliar | Atualização de movimento |
| `MovtoFinanceiroChequeEmitido.cs` | 3 KB | Relacionamento | Cheque emitido no movimento |
| `MovtoFinanceiroRateio.cs` | 12 KB | Relacionamento | Rateio no movimento |

### 2.7 Cheques

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Cheque.cs` | 9 KB | Entidade | Cheque |
| `ChequeRecebido.cs` | 37 KB | Entidade | Cheque recebido |
| `ChequeRecebidoMovto.cs` | **57 KB** | Entidade | Movimentos do cheque recebido |
| `ChequeEmitidoMovto.cs` | **57 KB** | Entidade | Movimentos do cheque emitido |
| `TalaoCheque.cs` | 39 KB | Entidade | Talão de cheque |
| `SuprimentoCheque.cs` | 7 KB | Entidade | Suprimento de cheque |
| `MotivoDevolucaoCheque.cs` | 8 KB | Entidade | Motivo de devolução |

### 2.8 Formas de Pagamento/Recebimento

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `formapagamentomov.cs` | Entidade | Forma de pagamento no movimento (15 KB) |
| `formapagamentomovlista.cs` | Lista | (18 KB) |
| `FormaMovInfo.cs` | Entidade | Informações da forma |
| `FormaMovInfoDinheiro.cs` | Entidade | Info — Dinheiro (8 KB) |
| `FormaMovInfoCartao.cs` | Entidade | Info — Cartão (8 KB) |
| `FormaMovInfoPix.cs` | Entidade | Info — PIX (11 KB) |
| `FormaMovInfoChequeCliente.cs` | Entidade | Info — Cheque cliente |
| `FormaMovInfoChequeEmpresa.cs` | Entidade | Info — Cheque empresa |
| `FormaMovInfoCredito.cs` | Entidade | Info — Crédito |
| `FormaMovInfoDeposito.cs` | Entidade | Info — Depósito |
| `FormaMovInfoAbatimento.cs` | Entidade | Info — Abatimento |

### 2.9 Reversão e Operações

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Reversao.cs` | **67 KB** | Operação | Reversão de movimentos |
| `ReversaoDoctoParcela.cs` | 6 KB | Relacionamento | Parcela revertida |
| `ReversaoItemFinanceiro.cs` | 7 KB | Relacionamento | Item financeiro revertido |
| `OperacaoDocumentoBase.cs` | 36 KB | Base | Operação base de documento |
| `aplicacaoitemfin.cs` | 12 KB | Operação | Aplicação de item financeiro |
| `aplicacaoitemfingeral.cs` | 8 KB | Operação | Aplicação geral |

### 2.10 DRE e Projeções

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `DRE.cs` | Entidade | DRE — Demonstrativo de Resultado (25 KB) |
| `DRETitulo.cs` | Relacionamento | Título do DRE |
| `DRETituloClasse.cs` | Relacionamento | Classe do título |
| `DRETituloOperacao.cs` | Relacionamento | Operação do DRE |
| `ProjecaoFluxoCaixa.cs` | Relatório | Projeção de fluxo de caixa (24 KB) |
| `ProjecaoFluxoCaixaLacto.cs` | Entidade | Lançamento da projeção (14 KB) |
| `PeriodosAbertos.cs` | Consulta | Períodos financeiros abertos (17 KB) |
| `SelecaoDocumento.cs` | Consulta | Seleção de documentos (34 KB) |

### 2.11 Outros

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Cobrador.cs` | Entidade | Cobrador (10 KB) |
| `IndiceConversor.cs` | Entidade | Índice de conversão |
| `ItemFinanceiro.cs` | Entidade | Item financeiro (15 KB) |
| `ItemFinanceiroBase.cs` | Base | Base de item financeiro (14 KB) |
| `ItemFinanceiroOperacao.cs` | Operação | Operação sobre item financeiro |
| `ParcelaBase.cs` | Base | Parcela base (25 KB) |
| `ParcelaBaseLista.cs` | Lista | (39 KB) |
| `ProgramacaoCobranca.cs` | Entidade | Programação de cobrança (11 KB) |
| `ManutencaoRateioFinanceiro.cs` | Operação | Manutenção de rateio (11 KB) |
| `ContraPartidaRateioDoctoFinanceiro.cs` | Entidade | Contra-partida de rateio (10 KB) |

---

## 3. Regras de Negócio Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-05-001 — Parcela é o Título
No sistema financeiro, cada parcela (`DocumentoParcela`) é um título independente
a pagar ou receber. A liquidação é sempre feita por parcela, não pelo documento.

### RN-05-002 — Domínio e Período
Cada movimento financeiro pertence a um `DominioPeriodo` (ex: "Caixa — Abril/2026").
O período pode ser fechado, impedindo novos lançamentos.

### RN-05-003 — Formas de Pagamento Múltiplas
Uma liquidação pode usar múltiplas formas de pagamento (ex: parte em dinheiro, parte em cartão).
Cada `FormaMovInfo` carrega as informações específicas da forma.

### RN-05-004 — Rateio
Movimentos financeiros podem ser rateados entre centros de custo e classes.
O rateio é registrado em `MovtoFinanceiroRateio`.

### RN-05-005 — PIX e Cartão
O sistema já tem suporte a PIX (`FormaMovInfoPix`) e cartão com bandeiras.
Preservar toda a lógica de integração.

### RN-05-006 — Conta Bancária é Complexa
`ContaBancaria.cs` com 57 KB tem toda a lógica de conciliação bancária, OFX, etc.
Analisar completamente antes de implementar.

---

## 4. Ordem de Implementação Recomendada

```
1. ItemFinanceiro base (base de toda parcela)
2. Dominio e DominioPeriodo
3. CaixaBanco
4. ContaBancaria (analisar primeiro)
5. DocumentoFinanceiroBase
6. DocumentoParcela (analisar antes)
7. Documento (conta a pagar/receber)
8. MovimentoFinanceiro
9. Formas de pagamento (FormaMovInfo e variações)
10. Liquidacao
11. LiquidacaoEstorno
12. Cheques (Recebido e Emitido)
13. Adiantamentos e Acertos
14. Reversao
15. DRE e Projeções
```

---

## 5. Roteiro de Tarefas (Ordem de Execução)

Siga a sequência abaixo para implementar MOD-05. Cada tarefa deve ser concluída em um commit separado, revisada por PR e alinhada à análise do legado. Erros de validação devem retornar `Result<T>`/`ValidationResult`; apenas falhas inesperadas usam exceções.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.GestaoFinanceira**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.GestaoFinanceira`
- Adicione referências a `Versatus.Framework`, `Versatus.AcessoGlobal`, `Versatus.Faturamento`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/gestao-financeira-project`
- Commit: `setup: Create Versatus.GestaoFinanceira project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Documentos/`, `Movimentos/`, `Liquidados/`, `Bancos/`, `Cheques/`, `DRE/`, `Operacoes/`
- Branch: `setup/gestao-financeira-structure`
- Commit: `setup: Create GestaoFinanceira folder structure`

**Tarefa 1.3 — Criar DbContext Base**
- Crie `Infrastructure/GestaoFinanceiraDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/gestao-financeira-dbcontext-base`
- Commit: `setup: Create GestaoFinanceiraDbContext base`

### Fase 2: Domínio Financeiro e Períodos

**Tarefa 2.1 — Implementar ItemFinanceiro base**
- Crie `Domain/Documentos/ItemFinanceiroBase.cs` e `Domain/Documentos/ItemFinanceiro.cs`
- Defina campos comuns usados por parcelas e movimentos financeiros
- Branch: `feat/item-financeiro-entity`
- Commit: `feat: Implement ItemFinanceiro base entities`

**Tarefa 2.2 — Implementar Dominio e DominioPeriodo**
- Crie `Domain/Dominio/Dominio.cs`, `DominioPeriodo.cs`, `DominioPeriodoFechamento.cs`, `DominioPeriodoFormaPagto.cs`, `DominioPeriodoLacto.cs`
- Garanta que o período controle abertura, fechamento e forma de pagamento
- Branch: `feat/dominio-periodo-entity`
- Commit: `feat: Implement DominioPeriodo entities`

**Tarefa 2.3 — Configurar Domínio no DbContext**
- Atualize `GestaoFinanceiraDbContext` com `DbSet` desses domain entities
- Branch: `feat/dominio-dbcontext`
- Commit: `feat: Configure dominio entities in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Caixa e Conta Bancária

**Tarefa 3.1 — Implementar CaixaBanco**
- Crie `Domain/Bancos/CaixaBanco.cs` e `CaixaBancoUsuario.cs`
- Inclua campos de saldo e controle de acesso por usuário
- Branch: `feat/caixa-banco-entity`
- Commit: `feat: Implement CaixaBanco entities`

**Tarefa 3.2 — Analisar ContaBancaria.cs**
- Leia completamente `servidor/objeto de negócio/gestao.financeira/ContaBancaria.cs` (57 KB)
- Documente regras de conciliação, OFX e controles bancários
- Branch: `analysis/conta-bancaria`
- Commit: `docs: Analyze legacy ContaBancaria behavior`

**Tarefa 3.3 — Implementar ContaBancaria**
- Crie `Domain/Bancos/ContaBancaria.cs` com todos os campos críticos identificados
- Inclua relacionamentos de saldo, cheques e contas vinculadas
- Branch: `feat/conta-bancaria-entity`
- Commit: `feat: Implement ContaBancaria entity`

**Tarefa 3.4 — Configurar bancos no DbContext**
- Atualize `GestaoFinanceiraDbContext` com `CaixaBanco` e `ContaBancaria`
- Branch: `feat/bancos-dbcontext`
- Commit: `feat: Configure banking entities in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Documento Financeiro e Parcela

**Tarefa 4.1 — Implementar DocumentoFinanceiroBase**
- Crie `Domain/Documentos/DocumentoFinanceiroBase.cs` como base para contas a pagar/receber
- Inclua campos de data, tipo, entidade e status
- Branch: `feat/documento-financeiro-base`
- Commit: `feat: Implement DocumentoFinanceiroBase`

**Tarefa 4.2 — Analisar DocumentoParcela.cs**
- Leia completamente `servidor/objeto de negócio/gestao.financeira/DocumentoParcela.cs` (71 KB)
- Documente dados do título, prazo, condições e eventos de cobrança
- Branch: `analysis/documento-parcela`
- Commit: `docs: Analyze legacy DocumentoParcela behavior`

**Tarefa 4.3 — Implementar DocumentoParcela**
- Crie `Domain/Documentos/DocumentoParcela.cs`, `DocumentoParcelaManutencao.cs`, `DocumentoParcelaImage.cs`
- No novo modelo, use coleções genéricas para imagens e manutenções
- Branch: `feat/documento-parcela-entity`
- Commit: `feat: Implement DocumentoParcela entities`

**Tarefa 4.4 — Implementar DocumentoFinanceiro**
- Crie `Domain/Documentos/Documento.cs` e `DocumentoMovto.cs`
- Relacione o documento com parcelas, itens financeiros e movimentos
- Branch: `feat/documento-financeiro-entity`
- Commit: `feat: Implement DocumentoFinanceiro entity`

**Tarefa 4.5 — Configurar documento financeiro no DbContext**
- Atualize `GestaoFinanceiraDbContext` com `Documento`, `DocumentoParcela`, `DocumentoMovto`
- Branch: `feat/documento-financeiro-dbcontext`
- Commit: `feat: Configure financial document entities in DbContext`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Movimentos Financeiros e Formas de Pagamento

**Tarefa 5.1 — Implementar MovimentoFinanceiro**
- Crie `Domain/Movimentos/MovimentoFinanceiro.cs` e `MovimentoFinanceiroUpdate.cs`
- Defina o relacionamento com rateios e formas de pagamento
- Branch: `feat/movimento-financeiro-entity`
- Commit: `feat: Implement MovimentoFinanceiro entity`

**Tarefa 5.2 — Implementar Formas de Pagamento**
- Crie `Domain/Movimentos/FormaMovInfo.cs`, `FormaMovInfoDinheiro.cs`, `FormaMovInfoCartao.cs`, `FormaMovInfoPix.cs`, `FormaMovInfoChequeCliente.cs`, `FormaMovInfoChequeEmpresa.cs`, `FormaMovInfoCredito.cs`, `FormaMovInfoDeposito.cs`, `FormaMovInfoAbatimento.cs`
- Garanta que cada forma carregue dados específicos corretamente
- Branch: `feat/formas-pagamento-entities`
- Commit: `feat: Implement payment form entities`

**Tarefa 5.3 — Configurar movimento financeiro no DbContext**
- Atualize `GestaoFinanceiraDbContext` com `MovimentoFinanceiro` e `FormaMovInfo` variantes
- Branch: `feat/movimentos-dbcontext`
- Commit: `feat: Configure financial movements in DbContext`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Liquidação e Estorno

**Tarefa 6.1 — Implementar Liquidacao**
- Crie `Domain/Liquidados/Liquidacao.cs`, `LiquidacaoFormaMovimento.cs`
- Implemente liquidação por parcela e controle de formas de pagamento
- Branch: `feat/liquidacao-entity`
- Commit: `feat: Implement Liquidacao entity`

**Tarefa 6.2 — Analisar LiquidacaoEstorno.cs**
- Leia completamente `servidor/objeto de negócio/gestao.financeira/LiquidacaoEstorno.cs` (97 KB)
- Documente regras de estorno e diferenças para a liquidação normal
- Branch: `analysis/liquidacao-estorno`
- Commit: `docs: Analyze legacy LiquidacaoEstorno behavior`

**Tarefa 6.3 — Implementar LiquidacaoEstorno**
- Crie `Domain/Liquidados/LiquidacaoEstorno.cs`, `LiquidacaoEstornoFormaPagto.cs`
- Assegure que o estorno reverta corretamente parcelas e saldos
- Branch: `feat/liquidacao-estorno-entity`
- Commit: `feat: Implement LiquidacaoEstorno entity`

**Tarefa 6.4 — Configurar liquidação no DbContext**
- Atualize `GestaoFinanceiraDbContext` com `Liquidacao` e `LiquidacaoEstorno`
- Branch: `feat/liquidacao-dbcontext`
- Commit: `feat: Configure liquidation entities in DbContext`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Cheques

**Tarefa 7.1 — Implementar Cheque**
- Crie `Domain/Cheques/Cheque.cs`, `ChequeRecebido.cs`, `ChequeRecebidoMovto.cs`, `ChequeEmitidoMovto.cs`, `TalaoCheque.cs`, `SuprimentoCheque.cs`, `MotivoDevolucaoCheque.cs`
- Garanta suporte a entradas e saídas de cheques
- Branch: `feat/cheques-entities`
- Commit: `feat: Implement cheque entities`

**Tarefa 7.2 — Testar movimentos de cheque**
- Adicione testes para emissão, recebimento e devolução de cheque
- Branch: `test/cheques`
- Commit: `test: Add cheque movement tests`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Adiantamentos e Acertos

**Tarefa 8.1 — Implementar Adiantamento**
- Crie `Domain/Operacoes/Adiantamento.cs`, `AdtoAcerto.cs`, `AdtoAcertoDistribuicao.cs`, `AdtoAcertoMovto.cs`, `AdtoLanctoEntidade.cs`
- Implemente lógica de adiantamento, acerto e rateio de valores
- Branch: `feat/adiantamento-entity`
- Commit: `feat: Implement adiantamento and settlement entities`

**Tarefa 8.2 — Testar adiantamentos**
- Adicione testes para acerto e distribuição de adiantamento
- Branch: `test/adiantamento`
- Commit: `test: Add adiantamento flow tests`
- Marque no checklist: ✅ Fase 8 completa

### Fase 9: Reversão e Operações

**Tarefa 9.1 — Implementar Reversao**
- Crie `Domain/Operacoes/Reversao.cs`, `ReversaoDoctoParcela.cs`, `ReversaoItemFinanceiro.cs`
- Implemente regras de reversão de documento, parcela e item financeiro
- Branch: `feat/reversao-entity`
- Commit: `feat: Implement reversal entities`

**Tarefa 9.2 — Configurar reversões no DbContext**
- Atualize `GestaoFinanceiraDbContext` com as entidades de reversão
- Branch: `feat/reversao-dbcontext`
- Commit: `feat: Configure reversal entities in DbContext`
- Marque no checklist: ✅ Fase 9 completa

### Fase 10: DRE e Projeções

**Tarefa 10.1 — Implementar DRE**
- Crie `Domain/DRE/DRE.cs`, `DRETitulo.cs`, `DRETituloClasse.cs`, `DRETituloOperacao.cs`
- Implemente estrutura de relatório para demonstração de resultado
- Branch: `feat/dre-entity`
- Commit: `feat: Implement DRE entities`

**Tarefa 10.2 — Implementar Projeção de Fluxo de Caixa**
- Crie `Domain/DRE/ProjecaoFluxoCaixa.cs`, `ProjecaoFluxoCaixaLacto.cs`
- Implemente consultas de projeção e períodos abertos
- Branch: `feat/projecao-fluxo-caixa-entity`
- Commit: `feat: Implement cash flow projection entities`

**Tarefa 10.3 — Configurar DRE no DbContext**
- Atualize `GestaoFinanceiraDbContext` com DRE e projeções
- Branch: `feat/dre-dbcontext`
- Commit: `feat: Configure DRE entities in DbContext`
- Marque no checklist: ✅ Fase 10 completa

### Fase 11: Repositórios e Migrations

**Tarefa 11.1 — Criar Repositórios-Chave**
- Crie `Infrastructure/Repositorios/IDocumentoFinanceiroRepository.cs`, `DocumentoFinanceiroRepository.cs`, `IMovimentoFinanceiroRepository.cs`
- Branch: `feat/financeira-repositories`
- Commit: `feat: Implement financial repository interfaces`

**Tarefa 11.2 — Criar Migrations EF Core**
- Crie a primeira migration para `GestaoFinanceiraDbContext`
- Valide todas as tabelas e relacionamentos
- Branch: `setup/gestao-financeira-migrations`
- Commit: `setup: Create initial EF Core migrations for GestaoFinanceira`

**Tarefa 11.3 — Registrar DI**
- Crie método de extensão para registrar DbContext, handlers e repositórios
- Branch: `setup/gestao-financeira-di`
- Commit: `setup: Configure dependency injection for GestaoFinanceira`
- Marque no checklist: ✅ Fase 11 completa

---

## 6. Checklist de Conclusão do Módulo

- [ ] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: Domínio e Períodos — Completa
- [ ] Fase 3: Caixa e Conta Bancária — Completa
- [ ] Fase 4: Documento Financeiro e Parcela — Completa
- [ ] Fase 5: Movimentos Financeiros e Formas de Pagamento — Completa
- [ ] Fase 6: Liquidação e Estorno — Completa
- [ ] Fase 7: Cheques — Completa
- [ ] Fase 8: Adiantamentos e Acertos — Completa
- [ ] Fase 9: Reversão e Operações — Completa
- [ ] Fase 10: DRE e Projeções — Completa
- [ ] Fase 11: Repositórios e Migrations — Completa

---

## 7. Checklist de Conclusão do Módulo

- [ ] `Documento.cs` analisado (101 KB)
- [ ] `DocumentoParcela.cs` analisado (71 KB)
- [ ] `MovimentoFinanceiro.cs` analisado (75 KB)
- [ ] `LiquidacaoEstorno.cs` analisado (97 KB)
- [ ] `ContaBancaria.cs` analisado (57 KB)
- [ ] Entidades base criadas
- [ ] Contas a pagar/receber implementadas
- [ ] Liquidação implementada com todas as formas de pagamento
- [ ] Cheques implementados
- [ ] Reversão implementada
- [ ] DRE implementado
- [ ] Testes de integração financeira com faturamento

---

## Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/gestao.financeira/` — 142 arquivos — `projeto_tag_1906`*
