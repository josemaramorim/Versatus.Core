---
name: legacy-validation-audit
description: Audita e extrai 100% das validações e regras de negócio legadas (incluindo herança de classes pai backend/frontend, eventos de UI, domínio e banco) gerando a Matriz RTV (Rastreabilidade Total de Validações). Use sempre ao mapear formulários legados para garantir segurança matemática de conversão.
---

# Skill: Legacy Validation Audit (Auditoria e Extração de Validações Legadas)

## 📌 Objetivo
Garantir que **100% das validações e regras de negócio legadas** — sejam elas de interface, domínio, regras de estado, cruzamento de dados ou integridade relacional — sejam identificadas, mapeadas e transferidas sem perda para a nova arquitetura .NET 10 (Backend) e React (Frontend).

---

## 🔍 As 4 Camadas de Varredura

Ao analisar um formulário e entidade legados, esta skill impõe a varredura em 4 camadas distintas:

```
                          ┌────────────────────────────────────────────────────────┐
  1. UI / Formulário      │ Eventos _Validating, _Leave, _SelectedIndexChanged,    │
                          │ ErrorProvider, MessageBox, Habilitar/Desabilitar      │
                          └────────────────────────────────────────────────────────┘
                                                     │
                          ┌────────────────────────────────────────────────────────┐
  2. Domínio / Negócio    │ Métodos Validar(), ValidarInclusao(), ValidarExclusao(),│
                          │ Property Setters, regras de estado/status             │
                          └────────────────────────────────────────────────────────┘
                                                     │
                          ┌────────────────────────────────────────────────────────┐
  3. Helpers Globais      │ Validadores.cs, CalculosTributarios.cs, CNPJUtil.cs     │
                          └────────────────────────────────────────────────────────┘
                                                     │
                          ┌────────────────────────────────────────────────────────┐
  4. Banco de Dados       │ NOT NULL, CHECK Constraints, Triggers, Indexes Unicos   │
                          └────────────────────────────────────────────────────────┘
```

---

## 🌲 1. Resolução Recursiva de Herança (Inheritance Tree Scan)

**Regra Crítica:** Nenhuma entidade ou formulário legado deve ser analisado isoladamente se possuir classe pai.

1. **Backend:**
   - Identificar a declaração da classe: `public class Ent[Nome] : [ClassePai]`
   - Se `[ClassePai]` não for `object` ou `EntidadeBase` pura, abrir e ler integralmente o arquivo da classe pai.
   - Varrer métodos `override` e chamadas `base.Validar()`.

2. **Frontend Legado:**
   - Identificar a declaração do Form: `public class F[Nome] : [FormPai]`
   - Se `[FormPai]` possuir validações de tela ou manipuladores de eventos base (`ValidarCamposBase()`), incluir no escopo de auditoria.

---

## 🔎 2. Varredura por Palavras-Chave de Validação (Grep Audit)

Procurar exaustivamente nos arquivos legados (classe de negócio + formulário + classes pai) pelos seguintes termos chaves:

- `Validar`
- `Validating`
- `MessageBox`
- `ErrorProvider`
- `throw`
- `IsNullOrEmpty`
- `Trim() == ""`
- `if (`
- `Cancel = true`
- `SetError`
- `BeforePost`
- `OnValidate`

---

## 📋 3. A Matriz RTV (Rastreabilidade Total de Validações)

Toda análise deve produzir obrigatoriamente a **Matriz RTV**, que será incorporada na Spec Funcional (`docs/spec_f[nome].md`):

| ID | Origem Legada (Arquivo:Linha) | Camada / Classe | Regra / Condição Legada | Mensagem Legada Exata | Destino Backend (.NET Result<T>) | Destino Frontend (Zod + MUI) |
|---|---|---|---|---|---|---|
| **VAL-01** | `FCliente.cs:tbCPF_Validating` | UI / Filho | Digito verificador de CPF se `TipoPessoa == 'F'` | *"CPF digitado é inválido."* | `Domain/Services/ValidadorCpf` | `zod.refine(validaCPF)` |
| **VAL-02** | `EntPessoa.cs:Validar()` | Domínio / Pai | UF deve pertencer aos estados do Brasil | *"UF inválida."* | `if (!UfUtil.EhValida(dto.Uf))` | `zod.string().length(2)` |
| **VAL-03** | `EntCliente.cs:ValidarInclusao()` | Domínio / Filho | Se `IsIsento == false`, `IE` obrigatório | *"Informe a I.E. ou marque Isento."* | `if (!dto.IsIsento && string.IsNullOrEmpty(dto.IE))` | `zod.superRefine()` + `required` prop |
| **VAL-04** | `EntCliente.cs:ValidarExclusao()` | Domínio / Estado | Bloquear se cliente possui títulos pendentes | *"Cliente possui títulos financeiros pendentes."* | `if (await _repo.PossuiTitulos(id))` | *N/A (Apenas backend)* |

---

## 🛡️ 4. Dupla Trava e Cobertura por Testes Unitários (TDD)

1. **Frontend (Zod + MUI):**
   - Validação visual imediata. Todo campo obrigatório **DEVE** conter a prop `required` no MUI para exibir o asterisco vermelho.
   - O Schema Zod deve reproduzir as mensagens legadas exatas para manter familiaridade do usuário.

2. **Backend (.NET 10 Result<T>):**
   - Fonte da Verdade. Toda validação da Matriz RTV deve retornar `Result.Failure(new ValidationError(...))` (NUNCA lançar exceções).

3. **Testes Unitários de Validação (TDD):**
   - Para cada linha `VAL-xx` da Matriz RTV, a suíte de testes C# em `Versatus.Tests` deve possuir um método de teste com a anotação `[Fact]`.
   - A provação da suíte de testes unitários fornece a **segurança matemática** de que nenhuma regra do legado foi omitida.
