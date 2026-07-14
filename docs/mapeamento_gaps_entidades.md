# Mapeamento e Análise de Gaps - Cadastro de Entidades

Este documento registra o status atual de implementação (Campos, Tabelas e Endpoints) do cadastro de Entidades no **Versatus ERP**, comparando o formulário completo do Frontend (`IEntidadeForm`) com a estrutura de persistência real mapeada no Entity Framework do Backend (`AcessoGlobalDbContext`).

---

## 📊 Matriz de Cobertura (Front-End vs. Banco de Dados)

| Seção / Lista | Propriedade no Front | Tabela no Banco (EF) | Status da Integração | Ação Necessária |
| :--- | :--- | :--- | :--- | :--- |
| **Dados Principais** | Base Form Fields | `GloEntidade` | **100% Integrado** | Nenhuma |
| **Pessoa Física** | PF Form Fields | `GloEntidadeFisica` | **100% Integrado** | Nenhuma |
| **Pessoa Jurídica** | PJ Form Fields | `GloEntidadeJuridica` | **100% Integrado** | Nenhuma |
| **Endereços** | `enderecos` | `GloEntidadeEndereco` | **100% Integrado** | Nenhuma |
| **Contatos Adicionais**| `contatos` | *Não existe* | ⚠️ Apenas Mock no Front | Criar tabela + Relacionamento |
| **Telefones** | `telefones` | *Não existe* | ⚠️ Apenas Mock no Front | Criar tabela + Relacionamento |
| **CNAEs Complementares**| `cnaes` | *Não existe* | ⚠️ Apenas Mock no Front | Criar tabela + Relacionamento |

---

## 🏢 Status de Integração dos Papéis (Roles)

Quando um papel é ativado no cadastro, a entidade base é vinculada a uma tabela especializada de negócio. 

| Papel (Role) | Flag no Front | Tabela de Extensão (EF) | Status da Integração Base | Sub-Listas / Tabelas Filhas de Negócio |
| :--- | :--- | :--- | :--- | :--- |
| **Cliente** | `isCliente` | `GloCliente` | **Integrado** (Criar/Excluir) | 🔴 **Gaps:** Referências, Bens, Cartões, Parentes e Sócios (Apenas Mocks no Front) |
| **Fornecedor** | `isFornecedor` | `GloFornecedor` | **Integrado** (Criar/Excluir) | 🔴 **Gaps:** Contas Bancárias e Filiais Vinculadas (Apenas Mocks no Front) |
| **Funcionário** | `isFuncionario` | `GloFuncionario` | **Integrado** (Criar/Excluir) | 🔴 **Gaps:** Dependentes (Apenas Mocks no Front) |
| **Transportadora**| `isTransportadora`| `GloTransportadora` | **Integrado** (Criar/Excluir) | 🔴 **Gaps:** Veículos da Frota (Apenas Mocks no Front) |
| **Demais Papéis** | Flags de Filtro | Mapeadas na Entidade Base | **Integrado** | Nenhuma (Utilizam campos da Entidade Base) |

---

## 🔍 Detalhamento das Lacunas (Gaps) de Banco de Dados

Para viabilizar a gravação total das abas do formulário, as seguintes estruturas físicas precisam ser adicionadas ao banco de dados e mapeadas no Entity Framework:

### 1. Estruturas Gerais de Contato
* **Contatos (`IContato`):** Criar tabela `GloEntidadeContato` vinculada à `GloEntidade` (N para 1).
* **Telefones (`ITelefone`):** Criar tabela `GloEntidadeTelefone` vinculada à `GloEntidade` (N para 1).
* **CNAEs (`ICnae`):** Criar tabela `GloEntidadeCnae` (Tabela pivô ou relacionamento N para 1).

### 2. Estruturas de Negócio de Clientes (Abas do Cliente)
* **Referências (`ICliReferencia`):** Criar tabela `CliClienteReferencia` vinculada a `GloCliente`.
* **Bens (`ICliBem`):** Criar tabela `CliClienteBem` vinculada a `GloCliente`.
* **Cartões (`ICliCartao`):** Criar tabela `CliClienteCartao` vinculada a `GloCliente`.
* **Parentes (`ICliParente`):** Criar tabela `CliClienteParente` vinculada a `GloCliente`.
* **Sócios (`ICliSocio`):** Criar tabela `CliClienteSocio` vinculada a `GloCliente`.

### 3. Estruturas de Negócio de Fornecedores e Outros
* **Contas Bancárias (`IFornConta`):** Criar tabela `FornFornecedorConta` vinculada a `GloFornecedor`.
* **Dependentes do Funcionário (`IFuncDependente`):** Criar tabela `RhFuncionarioDependente` vinculada a `GloFuncionario`.
* **Veículos da Transportadora (`ITranspVeiculo`):** Criar tabela `TrpTransportadoraVeiculo` vinculada a `GloTransportadora`.

---

## 🗺️ Roadmap de Implementação Proposto (Passo a Passo)

### Plano de Ação por Fase
1. **Modelagem:** Criar as classes de Entidade C# na pasta `Domain/Entities/` do projeto `Versatus.AcessoGlobal`.
2. **Mapeamento (Fluent API):** Configurar chaves estrangeiras, nomes de tabelas legado e índices na pasta `Infrastructure/Mappings/`.
3. **Migração:** Executar `dotnet ef migrations add <Nome>` e `dotnet ef database update` para atualizar a base de dados.
4. **Sincronização no Serviço:** Atualizar os métodos de diff (como o `SincronizarEnderecos` existente) no `EntidadeService.cs` para gravar os novos dados em lote na mesma transação.
