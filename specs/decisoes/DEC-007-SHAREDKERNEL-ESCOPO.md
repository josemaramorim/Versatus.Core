# DEC-007 — `Versatus.SharedKernel` vira o kernel do ERP, não só do MOD-05

## Contexto e Desafio

O projeto `src/Versatus.SharedKernel` foi criado em `S-T01` (2026-09-03) como pré-requisito
do MOD-05 Gestão Financeira, com escopo fixado em `specs/modulos/MOD-05/clarify.md` (CLR-02)
como **"mínimo"**: *"enums financeiros + `Lookup` + container de rateio + interfaces
transversais... ampliar só sob necessidade comprovada de épico"*.

Durante a tarefa `E0-T01` (inventário de enums do MOD-05,
[`enums.md`](../modulos/MOD-05/enums.md)), foi constatado que **7 dos ~50 enums** que o
financeiro precisa **já existem**, criados durante a migração do MOD-02 (Acesso Global):

- `src/Versatus.AcessoGlobal/Domain/Entities/Enums.cs` — `FormaPagtoTipo`,
  `CondicaoPagtoTipo`, `ParcelamentoArredondamento`, `Disponibilidade`, `VencimentoTipo`,
  `ParcelamentoTipo` (valores idênticos ao legado, verificados).
- `src/Versatus.Framework/Sequences/SequencialTipo.cs` — `SequencialTipo`.

Além do MOD-02, `src/Versatus.GestaoTributo/Domain/Rules/Enums.cs` também mantém seu
próprio `Enums.cs` com enums tributários. Isso confirma um padrão emergente no repositório:
**cada módulo cria seus próprios enums**, sem um lugar único para os que são usados por
mais de um módulo (`CondicaoPagtoTipo` e os enums de forma de pagamento não são conceito
exclusivo de "acesso global" nem de "gestão financeira" — são conceito de ERP).

Seguir `E0-T02` ao pé da letra recriaria esses 6 enums dentro de `Versatus.SharedKernel`,
produzindo **dois enums `FormaPagtoTipo` diferentes** no mesmo repositório — a duplicação
que `plan.md §2` já proíbe ("Reusa de `Versatus.Framework`... **Não duplicar**").

Ao investigar a fidelidade desses enums já existentes, foi encontrado adicionalmente um
defeito em `EntidadeTipoPessoa` (não relacionado a duplicação, mas relevante para a decisão
de consolidar em um único lugar auditável — ver "Impacto" abaixo).

---

## Decisão

`Versatus.SharedKernel` deixa de ser "o kernel mínimo do MOD-05" (CLR-02) e passa a ser
**o kernel compartilhado do ERP inteiro**: o local único para enums, VOs e abstrações
usados por **dois ou mais módulos**. Isso substitui/amplia CLR-02 — a subseção "mínimo" ali
continua válida (não se cria kernel "para todo caso hipotético"), mas o critério de
"necessidade comprovada" passa a incluir "já existe e é reusado por outro módulo", não só
"comprovada por um épico do MOD-05".

Regras:
1. Um enum/VO usado por **um só módulo** continua no `Domain/` desse módulo
   (`Enums.cs` local — como hoje em `GestaoTributo`).
2. Um enum/VO usado por **dois ou mais módulos** (comprovado por uso real, não hipótese)
   mora em `Versatus.SharedKernel/Enums/` (ou pasta equivalente), e os módulos que o usam
   **referenciam** o SharedKernel (`ProjectReference`), nunca copiam a definição.
3. Migração do que já existe: os 6 enums de `AcessoGlobal/Domain/Entities/Enums.cs` que o
   MOD-05 também precisa (listados acima) são **movidos** para `Versatus.SharedKernel`;
   `Versatus.AcessoGlobal.csproj` ganha `ProjectReference` para `Versatus.SharedKernel`
   (direção seguro — `SharedKernel` só referencia `Versatus.Framework`, sem ciclo).
   `SequencialTipo` de `Versatus.Framework.Sequences` **permanece no Framework** (é mais
   baixo na pirâmide de dependências que o próprio SharedKernel e já é `Framework`-nativo);
   o MOD-05 reusa de lá, sem duplicar em `SharedKernel`.
4. Módulos futuros (MOD-03, MOD-04…) que precisarem de um enum já presente no
   `SharedKernel` **referenciam**, nunca recriam.

---

## Impacto

- **MOD-02 (Acesso Global) — status "Concluído", reaberto para esta mudança mecânica:**
  mover 6 enums + atualizar `.csproj` + os arquivos que os referenciam (6 arquivos) +
  rodar `Versatus.AcessoGlobal.Tests` antes de mesclar.
- **Efeito colateral encontrado e corrigido na mesma frente:** `EntidadeTipoPessoa`
  (`Fisica = 1, Juridica = 2`) diverge do legado `EntidadeFisicaJuridica`
  (`Fisica = 2, Juridica = 3`, `GloTipoEnumerado` idPai 1). O mapeamento EF
  (`IdFisicaJuridica`) não tem `HasConversion`, então o valor errado é gravado/lido
  diretamente. O frontend já contornava isso com uma conversão manual não documentada em
  `EntidadeCadastroConfig.tsx` (`1↔2` backend, `2↔3` UI). Corrigido nesta mesma decisão:
  renumerar o enum para `Fisica = 2, Juridica = 3` e remover o remendo do frontend — ver
  commit da PR de MOD-02 vinculada a esta DEC.
- **MOD-05:** `E0-T02` cria só os enums exclusivos do financeiro; os compartilhados vêm de
  `Versatus.SharedKernel` (já criado no MOD-02) — menos arquivos, sem duplicação.
- **`clarify.md` CLR-02:** referência cruzada adicionada apontando para esta DEC (a decisão
  original não é apagada — Regra Git / REGRA 14 de nunca reescrever histórico de decisão).

---

## Rationale (por que agora, e por que assim)

1. **Nome já sugere o escopo certo.** "SharedKernel" nunca foi nomeado
   `Versatus.GestaoFinanceira.Kernel` — o nome do projeto já era genérico; só a decisão de
   escopo (CLR-02) o restringia. Alinhar o escopo ao nome evita a pergunta se repetir a
   cada módulo novo.
2. **Custo baixo agora, custo crescente depois.** Hoje só 6 enums e 6 arquivos são
   afetados. Se a consolidação for adiada, cada módulo novo que reusar `FormaPagtoTipo`
   (por exemplo) crica sua própria cópia ou aponta para `AcessoGlobal` por conveniência —
   e desfazer isso depois, com mais módulos dependendo, é bem mais caro.
3. **Rastreabilidade > silêncio.** A alternativa (mover só quando o MOD-05 precisar, sem
   registrar) reescreveria o efeito de CLR-02 sem deixar rastro nas specs — proibido pelas
   Regras Anti-Alucinação. Uma DEC nova mantém o histórico de decisão íntegro.
4. **Corrigir o bug na mesma janela é mais barato que abrir um incidente separado.** O
   `EntidadeTipoPessoa` já está sendo tocado (mesmo `Enums.cs`, mesmo módulo, mesma bateria
   de testes a rodar) — adiar geraria um segundo ciclo de análise/PR/teste no mesmo arquivo.

---

## Não-decisão (fora de escopo aqui)

Esta DEC não move `Lookup`, container de rateio nem as interfaces `IMovimentoPeriodo`/
`IDadosPeriodoFormaPagto`/`IDadosRateioFinanceiro` fixadas em CLR-02/CLR-05 — essas
continuam com o escopo já decidido (MOD-05, exceto `IDadosComissao` que CLR-05 já apontava
para o SharedKernel). Só o critério de **quando um enum entra no SharedKernel** muda.
