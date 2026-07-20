# Guia de Mapeamento de Enums e Valores Legados

Este documento serve como referência técnica para os agentes converterem os valores legados de Enums e chaves do banco de dados antigo para os valores e estruturas correspondentes do novo sistema.

---

## 1. Mapeamento de Tipo de Pessoa (Entidade)

O banco de dados legado do ERP Versatus usa IDs numéricos específicos para diferenciar tipos de pessoa física e jurídica que divergem dos componentes normais de Select no frontend.

### Mapeamento Técnico:
- **Pessoa Física:**
  - **API/Banco:** `2` (Pessoa Física)
  - **React Form:** `1` (Física)
- **Pessoa Jurídica:**
  - **API/Banco:** `3` (Pessoa Jurídica)
  - **React Form:** `2` (Jurídica)

### Código de Conversão (Frontend):

```ts
// Em mapBackendToForm:
tipoPessoa: backend.tipoPessoa === 2 ? 1 : backend.tipoPessoa === 3 ? 2 : 1

// Em mapFormToBackend:
tipoPessoa: Number(form.tipoPessoa) === 1 ? 2 : Number(form.tipoPessoa) === 2 ? 3 : 2
```

---

## 2. Enums Comuns no Domínio

Muitas vezes, a API retorna inteiros correspondentes aos enums declarados em `Enums.cs`. O frontend deve mapear esses inteiros para as strings ou números correspondentes nas seleções de formulário.

### A. Sexo / Gênero
Mapeado pelo enum `SexoTipo`:
- `1` = Masculino
- `2` = Feminino
- `3` = Outro

### B. Estado Civil
Mapeado pelo enum `EstadoCivilTipo`:
- `1` = Solteiro
- `2` = Casado
- `3` = Divorciado
- `4` = Viúvo
- `5` = Separado
- `6` = União Estável

### C. Contribuinte ICMS
Mapeado pelo enum `IndicadorContribuinteICMS`:
- `1` = Contribuinte (Sim no frontend)
- `2` = Contribuinte Isento (Isento no frontend)
- `9` = Não Contribuinte (Não no frontend)

> **Regra do Backend:** Se for selecionado `ContribuinteIsento` (`2`), o campo de Inscrição Estadual (`InscricaoEstadual`) deve ser preenchido como `"ISENTO"` automaticamente ao salvar.

### D. Regime Tributário
Mapeado pelo enum `RegimeTributarioTipo`:
- `1` = Simples Nacional
- `2` = Simples Nacional - Excesso de Sublimite
- `3` = Regime Normal

### E. Enquadramento da Empresa
Mapeado pelo enum `EnquadramentoTipo`:
- `1` = MEI
- `2` = ME
- `3` = EPP
- `4` = Demais

---

## 3. Diretriz Geral de Mapeamento Bidirecional

Sempre que criar um arquivo `[Nome]CadastroConfig.tsx`, avalie se as propriedades da API condizem com as tipagens e interfaces do formulário. 
- Use o `mapBackendToForm` para converter de tipos complexos legados/aninhados (ex: `pessoaFisica.cpf`) para campos planos do formulário (ex: `cpf`).
- Use o `mapFormToBackend` para construir o DTO de salvamento estruturado (ex: aninhar `cpf` dentro de `pessoaFisica: { cpf }` se a API assim exigir).
- Nunca faça essa transformação de mapeamento dentro do componente React principal ou do hook de estado da lista para manter a arquitetura limpa (OOP).
