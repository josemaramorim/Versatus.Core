# Guia Prático: Como Escrever Bons Testes (Writing Good Tests)

> **Carregue esta referência quando:** Estiver escrevendo ou alterando testes, adicionando mocks, ou criando métodos auxiliares/fixtures de teste.

---

## Visão Geral

Um teste existe para capturar uma falha ou quebra específica de comportamento em produção. Dois princípios regem tudo aqui:

```
1. Todo teste nomeia a quebra/falha que ele captura
2. Todo teste exercita a implementação real (evitando mocks excessivos)
3. Toda expectativa é derivada de forma independente do código sob teste
```

O TDD estrito produz ambos naturalmente: um teste escrito primeiro e visto falhar contra o código real já provou que pode falhar, e só ganha um *mock* quando a dependência real provar ser externa, lenta ou com efeitos colaterais externos (ex: envio de e-mails, gateways de pagamento).

---

## Princípio 1: Nomeie a Quebra (Name the Break)

Antes de escrever o corpo do teste, responda: **qual alteração no código de produção faria este teste falhar — e essa alteração é um bug ou uma decisão de negócio?**

Um teste ganha seu lugar ao capturar um ramal de código incorreto, um argumento errado, um valor limite (boundary case) ou um contrato quebrado.

### Expectativas Derivadas de Forma Independente
Use literais ou fixtures verificadas manualmente. Nunca use os mesmos métodos auxiliares do código de produção para calcular o resultado esperado no teste:

#### ❌ Ruim (Asserção Espelho — reusa a mesma lógica para calcular ambos os lados):
```csharp
var esperado = CalculadoraTaxa.Calcular(100.0m);
Assert.Equal(esperado, service.Calcular(100.0m));
```

#### ✅ Bom (Literal Derivado de Forma Independente):
```csharp
// 100 * 0.15 = 15.00
Assert.Equal(15.00m, service.Calcular(100.0m));
```

### Evite "Detectores de Mudança" Superficiais
Se apenas uma decisão de refatoração alterar seu teste — como mudar o nome de uma variável privada ou o valor de uma constante —, o teste falhará em redesenhos limpos e dormirá durante bugs reais.
- ❌ Não teste: `Assert.Equal(5, Constantes.MAX_TENTATIVAS);`
- ✅ Teste o comportamento: "Uma chamada com falha é tentada 5 vezes e a 6ª tentativa nunca acontece."

### Teste o Seu Código, Não o Framework
Teste o contrato que seu código faz nas suas fronteiras — o endpoint registrado, a query emitida, o payload produzido. Mecânicas internas da Microsoft/EF Core cabem aos mantenedores do EF Core testar.

---

## Função de Checagem (Gate Function)

```
ANTES de escrever o corpo do teste:
  1. Nomeie a mudança no código de produção que faria este teste falhar.
     - Não consegue nomear uma? → Redesenhe em torno de um comportamento observável.
     - "O texto fonte mudou"   → Execute o artefato e asserte seus efeitos reais.
     - Apenas decisões internas → Teste o comportamento visível que depende da decisão.

  2. Confirme que o valor esperado foi derivado SEM utilizar a lógica do código testado.
     - Se reutilizou a lógica ou helpers da classe sob teste:
       Substitua por um valor literal ou fixture checada manualmente.
```

---

## Princípio 2: Exercite a Coisa Real (Exercise the Real Thing)

Mocks devem ser a exceção, não a regra:
- Use banco em memória ou SQLite para testes de repositório e infraestrutura.
- Mocks só ganham espaço em APIs de terceiros (ex: gateways de pagamento, APIs externas, serviços de e-mail).
- **Mocks não ganham asserções:** Assertar que um mock foi chamado prova apenas que o mock está presente; asserte o resultado observável produzido pelo componente real.
