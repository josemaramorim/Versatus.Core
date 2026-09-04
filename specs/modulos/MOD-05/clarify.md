# Clarificações — MOD-05

> Rodada iniciada em 2026-09-03 (SDD etapa 2). Cada item alimenta uma alteração rastreável
> na `spec.md`. IDs estáveis.

| ID | Origem | Pergunta | Opções | Resposta (data) | Efeito na spec |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **CLR-01** | spec §9 DÚVIDA-03 | Onde vivem as bases de rateio `RateioMovto` / `RateioMovtoItem` / `ManutencaoRateio` (hoje em `acesso.global`, não migradas)? | (a) migrar no MOD-02 primeiro; (b) SharedKernel; (c) trazer p/ MOD-05 | **(a) migrar no MOD-02 primeiro** (2026-09-03) | §1.3 pré-requisito: MOD-02 expõe `RateioMovto`/`RateioMovtoItem`/`ManutencaoRateio` antes do épico E5. §6 E5 `depende de` += MOD-02(rateio). §4.1 atualizado. |
| **CLR-02** | spec §9 DÚVIDA-01 | Escopo do `Versatus.SharedKernel` (E0)? | (a) mínimo; (b) mínimo + helpers; (c) amplo | **(a) mínimo** (2026-09-03) | §1.3: SharedKernel = enums financeiros + `Lookup` + container de rateio + interface `IMovimentoPeriodo`/`IDadosPeriodoFormaPagto`/`IDadosRateioFinanceiro` (ver CLR-05). `Result`/`ValidationResult`/`IContextoExecucao` reusados de `Versatus.Framework`. Ampliar só sob necessidade comprovada de épico. §6 E0 atualizado. |
| **CLR-03** | spec §9 DÚVIDA-07 | Dividir `ContaBancaria.cs` (1.704 l) entre entidade e integração bancária? | (a) separar E3/E14; (b) tudo E3; (c) decidir no /plan | **(a) separar** (2026-09-03) | §2.3: entidade `ContaBancaria` (campos: dados bancários, carteira, cedente, nosso-número) fica em E3; lógica de remessa/retorno CNAB e conciliação OFX vira serviço em E14. A análise da classe grande roda em E3 já marcando o que é E14. §6 E3/E14 atualizados. RN-05-016 aponta a divisão. |
| **CLR-04** | spec §9 DÚVIDA-13 | Fixar regra para as classes-base legadas? | (a) fixar agora; (b) caso a caso no /plan | **(a) fixar agora** (2026-09-03) | **Regra:** base legada (`DocumentoFinanceiroBase`, `OperacaoDocumentoBase`, `ItemFinanceiroBase`, `ParcelaGeral`/`ParcelaBase`, `FormaMovInfo`, `FechamentoCaixaBase`) → **classe abstrata POCO só com campos compartilhados** (sem herdar nada do framework); o **comportamento** (validação, sequência de persistência, rateio) → **serviço/handler compartilhado do módulo**, coberto pela Matriz ROT. §2.1 e novo §3.3 na spec. |
| **CLR-05** | spec §9 DÚVIDA-04 | Onde ficam `IMovimentoPeriodo`, `IDadosComissao`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro`? | (a) MOD-05 exceto `IDadosComissao` no SharedKernel; (b) todas no SharedKernel; (c) todas no MOD-05 | **(a)** (2026-09-03) | §4.2: `IMovimentoPeriodo`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro` → `Versatus.GestaoFinanceira.Domain`. `IDadosComissao` → `Versatus.SharedKernel` (futuro Faturamento consome sem depender do MOD-05). §6 E0 inclui `IDadosComissao`. |
| **CLR-06** | spec §9 DÚVIDA-09 | Como modelar `SelecaoDocumento` (1.226 l, sem tabela)? | (a) serviço de consulta sem entidade em E11; (b) entidade/agregado; (c) decidir no /plan | **(a)** (2026-09-03) | §2.11: `SelecaoDocumento` → serviço de consulta/filtro (sem entidade, sem tabela) no épico E11, exposto por interface. E6 (Liquidação), E12 (Cobrança) e E14 (Remessa) consomem. §6 E11 atualizado. |
| **CLR-07** | spec §9 DÚVIDA-08 | Quais telas de cheque (E9, ~15 forms) recebem React nesta rodada? | (a) só movimento + consulta; (b) todas; (c) nenhuma | **(a) só movimento + consulta** (2026-09-03) | §1.5: E9 React = cheque recebido, cheque emitido, movimento de cheque e consultas. Talão, suprimento, exclusão/manutenção e impressão → backend-only nesta rodada. |
| **CLR-08** | spec §9 DÚVIDA-10 | Impressão (DRE, cheque, recibo de liquidação)? | (a) backend devolve dados, frontend imprime; (b) PDF no backend; (c) decidir no /plan | **(a)** (2026-09-03) | §4.1 (linha `Interface.Impressao`): endpoint retorna modelo de dados estruturado; renderização/impressão é do frontend. Fora do domínio do MOD-05. §6 E6/E9/E11 sem serviço de impressão. |
| **CLR-09** | spec §9 DÚVIDA-02 | `Versatus.GestaoFinanceira` referencia `Versatus.Faturamento`? | — | **Resolvido por Artigo VIII** (2026-09-03) | §1.4 / dependency-graph: nenhuma `ProjectReference` para Faturamento (que nem existe). Ligação com Faturamento é 100% `int` lógico (`IdOrigem` + `ProcessoOrigem`). DÚVIDA-02 fechada. |
| **CLR-10** | spec §9 DÚVIDA-05 | Inventário dos DTOs que o estrangulamento já publica, para reconciliar. | — | **Resolvido por inspeção** (2026-09-03) | `Servidor.Strangler/Gestao.Financeira/DTOs/` contém só `EntidadeDto`, `ClienteDto`, `FornecedorDto`, `ParametroDto` — todos **entidades do AcessoGlobal**. O estrangulamento do MOD-05 apenas substituiu *lookups* de AcessoGlobal dentro de `Documento.cs`/`DocumentoFinanceiroBase.cs` por chamadas HTTP à API nova do AcessoGlobal. **Nenhum contrato financeiro é publicado.** `plan.md §4` vira nota curta: nada a reconciliar do lado financeiro; o legado continua chamando a API do AcessoGlobal como está. |
| **CLR-11** | spec §9 DÚVIDA-06 | Acesso ao banco legado para `INFORMATION_SCHEMA` no /plan; há dicionário de dados? | — | **Confirmado** (2026-09-03) | `localhost\SQLEXPRESS2008 / versatus` disponível nesta máquina. Sem dicionário adicional: `data-model.md` usa `INFORMATION_SCHEMA.COLUMNS` + materialização do 1º registro de cada tabela para confirmar NULL de fato. |
| **CLR-12** | spec §9 DÚVIDA-11 | `AdtoAcerto` é operação transacional ou CRUD? | — | **Operação** (2026-09-03) | §2.10 / §6 E10: `AdtoAcerto` tratado como **operação transacional** (Handler + Matriz ROT + golden tests), não CRUD, apesar de `FAcertoAdiantamento : FBaseCadastro` no legado. `Adiantamento` (o cadastro em si) permanece CRUD. |
| **CLR-13** | spec §9 DÚVIDA-12 | Nomenclatura das pastas `Domain/` de `Versatus.GestaoFinanceira`. | proposta da spec | **Proposta aceita** (2026-09-03) | `Bases/`, `Dominio/`, `Bancos/`, `Documentos/`, `Movimentos/`, `Liquidacao/`, `Reversao/`, `Cheques/`, `Adiantamentos/`, `DRE/`, `Cobranca/`, `TransacaoFilial/`, `Consultas/`. Ajuste fino permitido no `/plan §1`. |

> **Atualização pós-`/tasks` (2026-09-04):** durante `E0-T01` foi constatado que 6 enums que
> o MOD-05 precisa já existiam em `Versatus.AcessoGlobal` (MOD-02). O escopo "mínimo" da
> CLR-02 acima **não é revogado**, mas **[DEC-007](../../decisoes/DEC-007-SHAREDKERNEL-ESCOPO.md)**
> amplia o critério: `Versatus.SharedKernel` passa a ser o kernel do **ERP inteiro**, não só
> do MOD-05 — um enum usado por 2+ módulos mora lá, não é duplicado por módulo. Ver DEC-007
> para o racional e o impacto no MOD-02.

## Pendências remanescentes (bloqueantes: nenhuma)

Nenhuma pendência bloqueante para o `/plan`. Itens que continuarão sendo detalhados **dentro
do `/plan`** (não bloqueiam):
- Inventário fino de enums adicionais por épico (bandeira de cartão, alínea de devolução,
  tipo de conta/talão).
- Conjunto exato de campos de cada entidade (`data-model.md` via `INFORMATION_SCHEMA`).
- Decisão POCO-abstrata vs. serviço para cada base legada individual (a regra CLR-04 guia;
  a aplicação é por base).
