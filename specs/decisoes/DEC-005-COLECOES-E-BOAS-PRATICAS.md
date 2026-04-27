# Decisão Técnica — DEC-005: Coleções Nativas e Boas Práticas de C#
## Documento: decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** ✅ Decisão tomada  
> **Impacto:** Todos os módulos — regra global obrigatória

---

## Decisão 1 — Coleções: Usar .NET Nativo, NÃO usar ListBase

### Problema com o legado

O legado usa `ListBase` — uma classe wrapper sobre `ArrayList` que existia para
compatibilidade com .NET Remoting (`MarshalByRefObject`) e o Gentle.NET.
No .NET Core isso não existe e não deve ser replicado.

### Regra

> **Nunca** criar classes `[Entidade]Lista` no novo sistema.  
> **Sempre** usar as interfaces e coleções nativas do .NET.

### Mapeamento: Legado → .NET Nativo

| Situação | Tipo a usar | Quando usar |
|---|---|---|
| Coleção imutável retornada por repositório/query | `IReadOnlyList<T>` | Listas que não devem ser modificadas fora da entidade |
| Coleção interna de um agregado (pode ser modificada) | `List<T>` privada + `IReadOnlyList<T>` público | Dentro de entidades de domínio |
| Parâmetro de entrada de um método | `IEnumerable<T>` | Quando só precisa iterar |
| Resultado de query com paginação | `IReadOnlyList<T>` + `int Total` (record PagedResult) | Consultas paginadas |
| Dicionário de lookup | `IReadOnlyDictionary<TKey, TValue>` | Caches e lookups |

### Padrão de coleção em entidades de domínio

```csharp
// ✅ CORRETO — encapsulamento com coleção nativa
public class DocumentoVenda
{
    // interna mutável
    private readonly List<DocumentoItemVenda> _itens = new();

    // exposição imutável
    public IReadOnlyList<DocumentoItemVenda> Itens => _itens.AsReadOnly();

    // método controlado para adicionar
    public void AdicionarItem(DocumentoItemVenda item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _itens.Add(item);
    }
}

// ❌ ERRADO — não criar classe Lista legada
public class DocumentoItemVendaLista : ListBase { ... }  // NÃO FAZER
```

### O que fazer com as classes `[Entidade]Lista` do legado

As classes `*Lista` do legado (`DocumentoItemVendaLista`, `ClienteLista`, etc.) serviam
apenas como coleções fortemente tipadas compatíveis com Remoting.

**Na migração:** NÃO criar equivalentes. Usar `IReadOnlyList<T>` e `List<T>` diretamente.

> Em .NET Core, use coleções genéricas nativas. `IReadOnlyList<T>` é a forma preferida para
> expor listas imutáveis e `List<T>` ou `IList<T>` podem ser usados internamente quando mutação
> é necessária. Não usar `IList` sem tipo genérico, `ArrayList` ou `ListBase`.

| Classe legada | Substituto no novo sistema |
|---|---|
| `ClienteLista` | `IReadOnlyList<Cliente>` |
| `DocumentoItemVendaLista` | `IReadOnlyList<DocumentoItemVenda>` dentro de `DocumentoVenda` |
| `FormaPagamentoFilialLista` | `IReadOnlyList<FormaPagamentoFilial>` |
| Qualquer `*Lista` | `IReadOnlyList<*>` ou `List<*>` conforme contexto |

---

## Decisão 2 — Boas Práticas de C# Obrigatórias

As regras abaixo são **obrigatórias** em todo código gerado pela IA ou escrito
manualmente no novo sistema Versatus.

### BP-001 — Records para DTOs e Value Objects

```csharp
// ✅ DTOs de request/response como records
public record CriarClienteCommand(
    string Nome,
    string Cpf,
    string Email
);

public record ClienteResponse(
    int IdCliente,
    string Nome,
    string Cpf
);

// ✅ Value Objects como records imutáveis
public record Cpf(string Valor)
{
    public static Cpf Criar(string valor)
    {
        if (!EhValido(valor)) throw new RegraDeNegocioException("CPF inválido");
        return new Cpf(valor);
    }
    private static bool EhValido(string valor) => /* validação */ true;
}
```

### BP-002 — Async/Await em toda operação de I/O

```csharp
// ✅ sempre async
public async Task<ClienteResponse> Handle(ObterClienteQuery query, CancellationToken ct)
{
    var cliente = await _repositorio.ObterPorIdAsync(query.IdCliente, ct);
    return ClienteResponse.From(cliente);
}

// ❌ nunca sincronizar em cima de async
var cliente = _repositorio.ObterPorIdAsync(id).Result; // PROIBIDO
```

### BP-003 — Injeção de dependência via construtor

```csharp
// ✅ construtor com interfaces
public class CriarClienteHandler
{
    private readonly IClienteRepositorio _repositorio;
    private readonly IContextoExecucao _contexto;

    public CriarClienteHandler(
        IClienteRepositorio repositorio,
        IContextoExecucao contexto)
    {
        _repositorio = repositorio;
        _contexto = contexto;
    }
}

// ❌ nunca new dentro de classe que não seja Factory
var repo = new ClienteRepositorio(); // PROIBIDO fora de testes/factories
```

### BP-004 — Nullable Reference Types habilitado

Todos os projetos devem ter `<Nullable>enable</Nullable>` no `.csproj`.

```csharp
// ✅ explicito sobre nullable
public string Nome { get; init; } = string.Empty;       // não-nullable por padrão
public string? Complemento { get; init; }               // nullable explícito
public IReadOnlyList<Telefone> Telefones { get; init; } = []; // collection expression
```

### BP-005 — ArgumentNullException e Guard Clauses no início

```csharp
public void AdicionarItem(DocumentoItemVenda item)
{
    // ✅ guard clauses no topo, antes de qualquer lógica
    ArgumentNullException.ThrowIfNull(item);
    if (item.Quantidade <= 0)
        throw new RegraDeNegocioException("Quantidade deve ser maior que zero");

    // lógica...
    _itens.Add(item);
}
```

### BP-006 — Expressões modernas de C# (C# 12+)

```csharp
// ✅ collection expressions
private readonly List<Telefone> _telefones = [];

// ✅ pattern matching em lugar de is/cast manual
if (entidade is EntidadeFisica fisica)
    return fisica.Cpf;

// ✅ switch expressions
var descricao = situacao switch {
    SituacaoDocumento.Rascunho   => "Rascunho",
    SituacaoDocumento.Confirmado => "Confirmado",
    SituacaoDocumento.Cancelado  => "Cancelado",
    _ => throw new ArgumentOutOfRangeException(nameof(situacao))
};

// ✅ primary constructors (C# 12) quando adequado
public class CalculoPrecoService(ITabelaPrecoRepositorio repositorio)
{
    public async Task<decimal> CalcularAsync(int idProduto) { ... }
}
```

### BP-007 — Enumerados em lugar de constantes mágicas

```csharp
// ❌ legado: constantes inteiras espalhadas
if (situacao == 1) { ... }  // o que é 1?

// ✅ novo: enum com nome descritivo
public enum SituacaoDocumentoVenda
{
    Rascunho = 1,
    Confirmado = 2,
    Faturado = 3,
    Cancelado = 9
}
```

> ⚠️ Preservar os valores inteiros do legado nos enums para manter compatibilidade
> com o banco de dados existente.

### BP-008 — Logs estruturados com ILogger<T>

```csharp
// ✅ log estruturado (não concatenação de string)
_logger.LogInformation(
    "Documento {IdDocumento} confirmado para filial {IdFilial}",
    documento.Id, contexto.IdFilial);

// ❌ nunca concatenar strings no log
_logger.LogInformation("Documento " + id + " confirmado"); // PROIBIDO
```

### BP-009 — Sem exceções genéricas

```csharp
// ❌ nunca lançar Exception genérica
throw new Exception("Algo deu errado");

// ✅ sempre usar exceções específicas da hierarquia do Versatus
throw new RegraDeNegocioException("Estoque insuficiente para o produto {0}", produto.Nome);
throw new EntidadeNaoEncontradaException(typeof(Cliente), idCliente);
```

### BP-010 — Validação esperada não é exceção

Validações de entrada e regras de negócio esperadas devem ser tratadas como fluxo de resultado,
com erros estruturados, em vez de lançar exceções para controle de fluxo.

```csharp
// Local: Versatus.Framework/Validation/
public record ValidationError(string Campo, string Mensagem);
public record ValidationResult(bool IsValid, IReadOnlyList<ValidationError> Errors)
{
    public static ValidationResult Ok() => new(true, []);
    public static ValidationResult Fail(params ValidationError[] errors) => new(false, errors);
}

public sealed record Result<T>(bool IsSuccess, T? Value, IReadOnlyList<ValidationError> Errors)
{
    public static Result<T> Ok(T value) => new(true, value, []);
    public static Result<T> Fail(params ValidationError[] errors) => new(false, default, errors);
}

// ✅ fluxo normal de validação
public async Task<Result<ClienteResponse>> Handle(CriarClienteCommand command, CancellationToken ct)
{
    var validation = _validator.Validate(command);
    if (!validation.IsValid)
        return Result<ClienteResponse>.Fail(validation.Errors.ToArray());

    var cliente = await _repositorio.AdicionarAsync(..., ct);
    return Result<ClienteResponse>.Ok(ClienteResponse.From(cliente));
}
```

> Use exceções do tipo `VersatusException` apenas para falhas inesperadas,
> invariantes violados ou erros de persistência que não fazem parte do fluxo de validação normal.

### BP-011 — Nenhum código comentado (// codigo legado)

Código comentado não é permitido no novo sistema.
Se uma funcionalidade não for migrada ainda, criar uma SPEC aberta — não comentar código.

---

## Resumo: O que a IA NUNCA deve fazer

| Proibição | Regra |
|---|---|
| Criar classe `[Entidade]Lista` | DEC-005, Decisão 1 |
| Usar `ArrayList` | DEC-005, Decisão 1 |
| Usar `IList` sem tipo genérico | DEC-005, Decisão 1 |
| Usar `.Result` ou `.Wait()` em async | BP-002 |
| Instanciar dependências com `new` dentro de serviços | BP-003 |
| Usar constante inteira no lugar de enum | BP-007 |
| Lançar `Exception` genérica | BP-009 |
| Usar validação estruturada em vez de exceções para fluxo esperado | BP-010 |
| Deixar código comentado | BP-010 |
| Concatenar strings em log | BP-008 |

---

## Resumo: O que a IA SEMPRE deve fazer

| Obrigação | Regra |
|---|---|
| Usar `IReadOnlyList<T>` para expor coleções | DEC-005, Decisão 1 |
| Usar `List<T>` privado dentro de agregados | DEC-005, Decisão 1 |
| Usar records para DTOs e Value Objects | BP-001 |
| Tornar todo I/O async com CancellationToken | BP-002 |
| Injetar dependências via construtor | BP-003 |
| Habilitar Nullable Reference Types | BP-004 |
| Guard clauses no início dos métodos | BP-005 |
| Usar expressões modernas de C# 12 | BP-006 |
| Usar validação estruturada em vez de exceções para fluxo esperado | BP-010 |
| Usar enums com valores preservados do legado | BP-007 |
| Log estruturado com ILogger<T> | BP-008 |
| Exceções específicas da hierarquia Versatus | BP-009 |

---

## Adição ao Prompt Padrão de Segurança

Adicionar ao final do prompt padrão (`03-REGRAS-ANTI-ALUCINACAO.md`):

```
BOAS PRÁTICAS OBRIGATÓRIAS (DEC-005):
- Nunca criar classe [Entidade]Lista — usar IReadOnlyList<T>
- Todos os métodos de I/O devem ser async com CancellationToken
- DTOs e Value Objects devem ser records
- Coleções internas de agregados: List<T> privado + IReadOnlyList<T> público
- Nullable Reference Types habilitado
- Enums para situações/tipos (preservar valores inteiros do banco)
- ILogger<T> com log estruturado
- Exceções da hierarquia VersatusException
- Validations esperadas retornam `Result`/`ValidationResult`, não exceções
```

---

*Decisão: 2026-04-27*
