# Boas Práticas para Autoria de Skills (Anthropic Best Practices)

Este guia fornece diretrizes práticas para construir skills que a IA possa descobrir e executar com alta precisão.

---

## 1. Princípios Fundamentais

### A Concisão é Chave
A janela de contexto (*context window*) é um recurso valioso compartilhado com o histórico da conversa e o código-fonte. 
- No início da sessão, apenas o cabeçalho YAML (`name` e `description`) de cada skill é pré-carregado.
- O arquivo `SKILL.md` só é lido quando a skill se torna relevante para a tarefa atual.
- Portanto, ser conciso evita desperdício de contexto quando a skill é carregada.

### Defina o Nível de Liberdade Adequado
Ajuste a especificidade da instrução de acordo com a fragilidade da tarefa:

1. **Alta Liberdade (Instruções conceituais):**
   - Útil para auditorias de código e revisões de arquitetura.
   - Exemplo: "Analise a estrutura do código e sugira melhorias em Clean Architecture."

2. **Média Liberdade (Templates ou Pseudocódigo com Parâmetros):**
   - Útil para geração de DTOs, Controllers ou componentes React.
   - Exemplo: "Gere a estrutura da página React herdando BaseCadastroConfig<T>."

3. **Baixa Liberdade (Scripts e Comandos Exatos):**
   - Útil para operações frágeis como compilação, testes e deploys.
   - Exemplo: `dotnet build` ou `python scripts/migrate.py --verify`.

---

## 2. Padrões de Revelação Progressiva (Progressive Disclosure)

Para skills complexas:
- Mantenha o `SKILL.md` curto (menos de 500 linhas) como um índice/guia executivo.
- Mova detalhes técnicos, APIs e checklists longos para subpastas de um nível de profundidade (`references/`, `examples/`, `templates/`).
- O agente só lerá os arquivos das subpastas caso a tarefa exija tais detalhes adicionais.
