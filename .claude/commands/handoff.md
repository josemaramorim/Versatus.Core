---
description: Encerra a sessão de trabalho atualizando o Log de Progresso e Handoff (Seção 5) de specs/04-CONTRATO-DA-IA.md, conforme a Seção 4 do próprio contrato e a REGRA 14.
argument-hint: [resumo opcional do que foi feito nesta sessão]
---

A sessão de trabalho está sendo encerrada. Siga a Seção 4 ("Como Responder ao Usuário") de
`specs/04-CONTRATO-DA-IA.md` e a REGRA 14 de `specs/03-REGRAS-ANTI-ALUCINACAO.md`:

1. Rode `git branch --show-current` e `git log --oneline -n 15` para levantar o que foi de
   fato commitado nesta sessão — não confie só na memória da conversa. Use `$ARGUMENTS` como
   contexto adicional se fornecido, mas confirme contra o git log real antes de escrever
   qualquer coisa.
2. Edite a **Seção 5 (Log de Progresso e Handoff)** de `specs/04-CONTRATO-DA-IA.md`:
   - Atualize (ou adicione) a linha da tabela para o módulo trabalhado: Módulo Atual |
     Fase/Status | Task Atual | Branch Ativa | Observação Crítica.
   - Adicione uma linha nova em **Histórico Recente de Decisões**, no mesmo formato das
     existentes: `- **DATA (Módulo):** resumo do que foi feito, decisões tomadas, branch.`
     (use a data de hoje).
   - Atualize **Próxima Ação Pendente** para refletir o que realmente falta.
   - Nunca reescreva ou apague entradas antigas — só adicione.
3. Isto é manutenção rotineira do log (mandatória por REGRA 14), não uma mudança de
   arquitetura — pode aplicar a edição direto, sem esperar aprovação prévia.
4. Depois de editar, responda ao usuário confirmando:
   - Branch em que o trabalho ficou
   - Resumo dos commits feitos nesta sessão
   - Se algum comando de push/merge para `develop`/`main` foi tentado e bloqueado pelo hook
     de Git do projeto — reporte isso explicitamente
   - Se a branch parece pronta para revisão, **pergunte** se o usuário quer que ela seja
     mesclada — nunca execute o merge sozinho
5. Não invente conteúdo do log: se algo ficou incerto (ex. se um teste realmente passou),
   marque como `DÚVIDA:` em vez de assumir.
