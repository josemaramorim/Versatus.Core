# Decisão Técnica — DEC-002: Substituição do .NET Remoting
## Documento: decisoes/DEC-002-REMOTING.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** ✅ Decisão tomada

---

## Problema

O sistema legado usa **.NET Remoting** para comunicação entre cliente (WinForms) e servidor.
O .NET Remoting:
- Foi **removido completamente** no .NET Core (não existe no .NET 5+)
- Baseia-se em `MarshalByRefObject` e `BinaryFormatter` (ambos removidos/inseguros)
- Expõe objetos de negócio diretamente na rede (acoplamento total)
- Não funciona com firewalls, proxies ou HTTPS moderno

## Como funciona hoje (legado)

```
Cliente WinForms
  → IDocumentoVenda (interface Remoting)
    → [TCP Canal] → Servidor
      → DocumentoVenda : ObjectBase : MarshalByRefObject
```

O cliente recebe uma **referência proxy** do objeto servidor.
Quando o cliente chama `documentoVenda.Confirmar()`, a chamada vai pela rede
e executa no servidor. O cliente nunca tem o objeto real — só o proxy.

## Decisão: API REST com ASP.NET Core (Minimal API ou Controllers)

**Justificativa:**
1. Padrão de mercado consolidado
2. Funciona com qualquer cliente (WinForms, Web, Mobile)
3. HTTPS nativo
4. Testável com Swagger/OpenAPI
5. Suporte a autenticação JWT

## Como cada peça do Remoting é substituída

| Conceito Remoting | Substituto REST |
|---|---|
| `IDocumentoVenda` (interface Remoting) | DTO de Request/Response |
| `ObjectBase : MarshalByRefObject` | Classe de domínio pura (sem herança) |
| Proxy transparente (cliente chama método remoto) | Cliente faz HTTP request |
| `Transacao` passada pelo ambiente | Gerenciada internamente no Handler |
| `IAmbiente` viajando objeto a objeto | `IContextoExecucao` via DI (de JWT) |
| Canal TCP do Remoting | HTTP/HTTPS |
| `SponsorManager` (lifetime do objeto) | Gerenciado pelo DI Container |
| `BizServerManager` (host do servidor) | `WebApplication` do ASP.NET Core |
| `CanalServidor` (abertura do canal) | Não necessário — HTTP é stateless |

## Regra sobre Interfaces

As interfaces do tipo `IDocumentoVenda`, `ICliente` etc. existiam exclusivamente
para o Remoting funcionar de forma transparente.

**No novo sistema:**
- Interface → DTO de entrada (Command/Query)
- Interface → DTO de saída (Response)
- Classe de negócio → Entidade de domínio pura

```
LEGADO:
  IDocumentoVenda → proxy → DocumentoVenda (servidor)

NOVO:
  POST /documentos-venda → IncluirDocumentoVendaCommand → DocumentoVendaResponse
  PUT  /documentos-venda/{id}/confirmar → ConfirmarDocumentoVendaCommand → DocumentoVendaResponse
```

## Padrão de Nomeação de Endpoints

```
GET    /[modulo]/[entidade]           → ListarXxx
GET    /[modulo]/[entidade]/{id}      → ObterXxx
POST   /[modulo]/[entidade]           → CriarXxx
PUT    /[modulo]/[entidade]/{id}      → AlterarXxx
DELETE /[modulo]/[entidade]/{id}      → ExcluirXxx
POST   /[modulo]/[entidade]/{id}/[acao] → Operação de negócio (ex: confirmar, cancelar)
```

### Exemplos reais

```
GET    /faturamento/documentos-venda/{id}
POST   /faturamento/documentos-venda
PUT    /faturamento/documentos-venda/{id}/confirmar
PUT    /faturamento/documentos-venda/{id}/cancelar
GET    /acesso-global/clientes
POST   /acesso-global/clientes
```

## Compatibilidade com Cliente WinForms Legado

Durante o estrangulamento, o cliente WinForms legado ainda usa Remoting.
**Estratégia:** Manter o servidor legado rodando para o cliente antigo enquanto
o novo servidor REST é desenvolvido. Feature flags no cliente decidem qual usar.

> Uma alternativa futura: criar um **Adapter Remoting → REST** que permite ao
> cliente WinForms legado chamar o novo servidor via HTTP sem modificações.
> Avaliar conforme necessidade.

---

*Decisão: 2026-04-27*
