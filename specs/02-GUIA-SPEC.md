# SPEC — Guia de Uso das SPECs
## Documento: 02-GUIA-SPEC.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Leitura obrigatória antes de qualquer trabalho de migração**

---

## 1. O que é uma SPEC neste projeto?

Uma **SPEC** (Especificação Técnica) é um documento que descreve **o que existe** no sistema legado
e **o que deve ser criado** no novo sistema, com mapeamento direto entre ambos.

A SPEC **não é** um documento de design livre — ela é um espelho fiel do que o código legado faz,
traduzido para o novo padrão tecnológico.

> **Princípio Git-first:** use Git para versionar tanto as SPECs quanto o código. O fluxo deve usar `develop` para integração contínua, `release/*` para estabilização e `main` apenas para merges aprovados de release.

---

## 2. Anatomia de uma SPEC de Módulo

Cada arquivo `MOD-XX-NOME.md` segue esta estrutura obrigatória:

```
# SPEC — [Nome do Módulo]

## 1. Visão Geral do Módulo
   - O que este módulo faz (lido do código legado)
   - Dependências de outros módulos
   - Tabelas de banco envolvidas

## 2. Inventário de Classes Legadas
   - Lista de todas as classes .cs encontradas no módulo
   - Classificação: Entidade | Lista | Situação | Consulta | Auxiliar

## 3. Mapeamento Legado → Novo
   - Tabela com: Classe Legada | Tipo | Equivalente Novo | Observações

## 4. Entidades do Domínio (a criar)
   - Para cada entidade: propriedades mapeadas das colunas do Gentle

## 5. Repositórios (a criar)
   - Interfaces dos repositórios
   - Quais queries do legado precisam ser replicadas

## 6. Casos de Uso / Handlers (a criar)
   - Baseados nas classes *Situacao* do legado
   - Cada operação de negócio vira um Handler
   - Handlers devem retornar `Result<T>` / `ValidationResult` para validação esperada

## 7. Endpoints da API (a criar)
   - Rotas REST correspondentes às operações

## 8. Regras de Negócio Críticas
   - Regras identificadas no código legado que NÃO devem ser perdidas

## 9. Checklist de Conclusão
   - Lista de verificação antes de considerar o módulo migrado
```

---

## 3. Como trabalhar com a IA usando as SPECs

### 3.1 Fluxo correto

```
VOCÊ lê a SPEC do módulo
        │
        ▼
VOCÊ revisa todas as tarefas e itens listados na SPEC
        │
        ▼
VOCÊ abre a conversa com a IA e diz:
"Vou trabalhar no [MOD-XX]. Leia a SPEC abaixo e implemente APENAS
o item [seção específica]. Não faça mais nada além do que está na SPEC."
        │
        ▼
IA implementa SOMENTE o pedido
        │
        ▼
VOCÊ revisa o código gerado comparando com a SPEC
        │
        ▼
VOCÊ marca o item como concluído na SPEC
```

> Antes de começar qualquer módulo, confirme se a SPEC já descreve todas as tarefas necessárias. Se faltar algo, atualize a SPEC primeiro e só então peça implementação.

### 3.2 Prompts de segurança — use sempre

Ao iniciar uma sessão com a IA, copie e cole este cabeçalho:

```
=== MODO SPEC-FIRST ===
Estou migrando o sistema Versatus de .NET Framework para .NET Core 8.
Regras desta sessão:
1. Implemente SOMENTE o que está descrito na SPEC abaixo
2. Se uma SPEC não especificar algo, PERGUNTE — não invente
3. Preserve TODOS os nomes de tabelas do banco (não renomeie)
4. Preserve a lógica de negócio exatamente como no legado
5. Use `Result<T>` / `ValidationResult` para fluxos de validação esperados; não use exceções como controle de fluxo
6. Se detectar algo ambíguo no legado, documente como "DÚVIDA" e pare
7. Se precisar de mudança na SPEC, sugira primeiro — não implemente até a SPEC ser atualizada
=== FIM CABEÇALHO ===
```

### 3.3 Tamanho das sessões — nunca peça demais de uma vez

| Tamanho certo de pedido | Tamanho errado |
|---|---|
| "Crie a entidade `Cliente` conforme SPEC seção 4.1" | "Crie todo o módulo de Acesso Global" |
| "Implemente o repositório de `Cliente`" | "Implemente todos os repositórios do módulo" |
| "Crie o handler de `CadastrarCliente`" | "Implemente todos os casos de uso do módulo" |
| "Adicione o endpoint POST /clientes" | "Crie toda a API do módulo" |

**Regra de ouro:** Uma sessão = uma classe ou uma seção da SPEC.

### 3.4 Uso de Git

O projeto usa Git como controle de versão oficial. Todas as alterações de SPEC e código devem seguir o fluxo Git + SPEC:

> ⛔ **REGRA ABSOLUTA — IA NUNCA DEVE:**  
> - Fazer commit diretamente em `develop` ou `main`  
> - Executar `git merge develop`, `git push origin develop` ou `git push origin main`  
> - Executar `git rebase` sem permissão explícita do usuário  
>
> ✅ **FLUXO CORRETO:**
> ```
> git checkout develop && git pull origin develop
> git checkout -b feat/modulo-descricao   ← IA trabalha AQUI
> (commits incrementais na branch feat/)
> ← Para aqui. Usuário revisa e aprova o PR.
> ```

- Use branches nomeadas por módulo/tarefa, por exemplo `feat/mod-03-produto`, `docs/analise-estoque`, `fix/mod-02-senha`
- Convenção de commits: `feat(modulo):`, `fix(modulo):`, `docs(modulo):`, `test(modulo):`
- Faça commits pequenos e lógicos, com mensagens claras que referenciem o módulo ou a SPEC alterada
- Nunca implemente mudanças sem que a SPEC correspondente exista ou tenha sido ajustada primeiro
- Se a IA sugerir uma alteração na SPEC, atualize a SPEC primeiro e commite essa mudança antes de implementar o código
- Mantenha a branch focada: uma mudança principal por branch sempre que possível
- Use Pull Requests/Merge Requests para revisão antes de integrar ao ramo principal
- Adote um fluxo com `develop` para integração contínua de funcionalidades e `release/*` para estabilização
- Só integre em `main` através de um merge de `release/*` aprovado — **nunca diretamente**
- Mantenha `.gitignore` atualizado para artefatos de build, arquivos de IDE e dados locais
- Ao revisar um PR, verifique se a implementação está alinhada com a SPEC e se a mensagem de commit descreve a mudança

> Git é o registro oficial do projeto. A SPEC é a fonte da verdade; o código só muda depois que a SPEC é definida ou atualizada.


---

## 4. Como atualizar as SPECs

### 4.1 Quando atualizar
- Ao descobrir que a SPEC está errada (bateu com o código real)
- Ao tomar uma decisão de implementação que não estava na SPEC
- Ao concluir um item (marcar o checklist)

### 4.2 Como atualizar — nunca reescreva, apenas anote

No final de cada seção da SPEC, existe um campo `Histórico de Alterações`.
Adicione uma linha com:
```
| DATA | AUTOR | O QUE MUDOU |
```

Nunca delete conteúdo de uma SPEC. Use ~~strikethrough~~ para marcar algo obsoleto
e adicione o substituto logo abaixo.

### 4.3 Quem pode atualizar
- **Desenvolvedor humano:** pode atualizar qualquer parte
- **IA:** pode SUGERIR atualizações, mas o humano deve confirmar e aplicar

---

## 5. Alertas de Qualidade

### ⚠️ Sinais de que a IA está alucinnando (pare imediatamente)

- Criou classes que não existem no legado sem explicar por quê
- Renomeou tabelas do banco sem motivo
- Adicionou campos novos que não existem no legado
- Refatorou regras de negócio sem ser pedido
- Criou uma "arquitetura melhorada" sem você pedir
- Disse "este método pode ser simplificado" e simplificou sem validar

### ✅ Sinais de que o trabalho está correto

- Cada classe nova tem correspondência direta na SPEC
- Nomes de tabelas batem com o campo `[TableName]` do legado
- Regras de negócio mencionam de onde vieram (ex: "de DocumentoVendaSituacao.cs linha X")
- Quando há dúvida, a IA pergunta em vez de inventar

---

## 6. Exemplo de Sessão Bem-Sucedida

### Prompt:
```
=== MODO SPEC-FIRST ===
[...cabeçalho de segurança...]
=== FIM CABEÇALHO ===

Leia a SPEC MOD-02-ACESSO-GLOBAL, seção 4.1 (Entidade Cliente).
Crie APENAS a classe de domínio `Cliente` em C# com as propriedades
listadas na SPEC. Use records imutáveis. Não crie repositório nem handler agora.
```

### Resposta esperada da IA:
```csharp
// Baseado em: acesso.global/Cliente.cs (legado)
// Tabela: EntCliente (conforme [TableName] identificado na SPEC)
public record Cliente(
    int IdCliente,
    string Nome,
    string Cpf,
    // ... propriedades listadas na SPEC
);
```

---

## 7. Estrutura de Pastas do Novo Projeto

```
Versatus.Net8/
├── specs/                          ← Documentação (este diretório)
├── src/
│   ├── Versatus.Framework/         ← MOD-01: Infraestrutura base
│   ├── Versatus.AcessoGlobal/      ← MOD-02: Entidades globais
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   ├── Versatus.GestaoMaterial/    ← MOD-03
│   ├── Versatus.Faturamento/       ← MOD-04
│   ├── Versatus.GestaoFinanceira/  ← MOD-05
│   ├── Versatus.GestaoCompra/      ← MOD-06
│   ├── Versatus.GestaoTributo/     ← MOD-07
│   ├── Versatus.NFe/               ← MOD-08
│   ├── Versatus.GestaoRH/          ← MOD-09
│   ├── Versatus.GestaoContrato/    ← MOD-10
│   ├── Versatus.GestaoOS/          ← MOD-11
│   ├── Versatus.GestaoProducao/    ← MOD-12
│   └── Versatus.Shared/            ← Tipos compartilhados entre módulos
└── tests/
    ├── Versatus.AcessoGlobal.Tests/
    └── [um projeto de teste por módulo]
```

---

*Documento criado em: 2026-04-27*
