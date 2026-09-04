-- DEC-007 — correção pontual de dado (não é migration EF Core, é 1x manual)
-- Contexto: specs/decisoes/DEC-007-SHAREDKERNEL-ESCOPO.md, seção "Impacto".
--
-- EntidadeTipoPessoa (Versatus.AcessoGlobal) foi criado com Fisica=1, Juridica=2,
-- divergente do legado (EntidadeFisicaJuridica: Fisica=2, Juridica=3, GloTipoEnumerado
-- idPai=1). O mapeamento EF (IdFisicaJuridica) não tinha HasConversion, então o valor
-- errado foi gravado direto em produção/dev por dois cadastros feitos via backend novo.
--
-- Confirmado por SELECT antes da correção (2026-09-04, localhost\SQLEXPRESS2008 / versatus):
--   IDGLOENTIDADE=1001,  NOME='João da Silva',           DATAINCLUSAO=2026-06-05, IDFISICAJURIDICA=1
--   IDGLOENTIDADE=10004, NOME='Josemar Amorim Anjos',    DATAINCLUSAO=2026-07-15, IDFISICAJURIDICA=1
-- Ambos nomes de pessoa física — confirma que "1" era a intenção "Fisica" do enum antigo.
-- Nenhuma linha tinha IDFISICAJURIDICA=1 originada do legado (linhas legadas, DATAINCLUSAO
-- 2026-04-24, já usavam 2/3 corretamente).

UPDATE GLOENTIDADE SET IDFISICAJURIDICA = 2 WHERE IDFISICAJURIDICA = 1;

-- Executado em 2026-09-04, autorização explícita do usuário. Resultado esperado após rodar:
-- SELECT IDFISICAJURIDICA, COUNT(*) FROM GLOENTIDADE GROUP BY IDFISICAJURIDICA;
--   2 -> 335 (333 antigas + as 2 corrigidas)
--   3 -> 143
--   (nenhuma linha com valor fora de {2,3})
