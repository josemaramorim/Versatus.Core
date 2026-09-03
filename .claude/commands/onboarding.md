---
description: Relê o ritual de início do Versatus.Net8 (handoff, índice de módulos, incidentes) e apresenta um resumo do estado atual antes de começar a trabalhar.
argument-hint: [módulo opcional, ex. MOD-03]
---

Você está retomando trabalho no projeto Versatus.Net8. Antes de qualquer ação:

1. Releia agora, por completo e sem confiar em memória de contexto anterior (os arquivos
   podem ter mudado desde a última leitura):
   - `specs/04-CONTRATO-DA-IA.md`, Seção 5 (Log de Progresso e Handoff)
   - `specs/00-INDICE-GERAL.md` (tabela de Status dos Módulos)
   - `specs/decisoes/ALUCINACOES-DETECTADAS.md` (incidentes já registrados, se houver)
2. Se `$ARGUMENTS` indicar um módulo específico (ex. `MOD-03`), releia também a SPEC
   correspondente em `specs/modulos/MOD-XX-*.md` e confirme se ela cobre completamente a
   tarefa antes de prosseguir (Regra 7.2 de `specs/03-REGRAS-ANTI-ALUCINACAO.md`). Se algo
   estiver faltando na SPEC, diga isso agora — não comece a implementar.
3. Rode `git branch --show-current` e confira se bate com a branch registrada no Log de
   Progresso.
4. Apresente ao usuário um resumo curto (não o documento inteiro) com:
   - Módulo(s) ativo(s) e status atual
   - Branch atual (real, via git) vs. branch registrada no log — avise se divergirem
   - Última decisão relevante do Histórico Recente
   - Próxima Ação Pendente conforme o log
   - Qualquer `DÚVIDA:` ou pendência aberta encontrada nas specs
5. **Aguarde a confirmação do usuário antes de implementar qualquer coisa.** Esta etapa é
   só recontextualização — não gere código nem edite specs de módulo aqui.
