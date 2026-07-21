# Prompt de Execução — MOD-05: Estrangulamento de Documento Financeiro (Legado)

> **Para a IA executora:** Sua tarefa aqui é **modificar os arquivos C# no projeto legado** (`projeto_tag_1906`) para substituir a lógica interna de consultas ao banco via Gentle.NET por chamadas HTTP para a nova API do .NET 8.
> Em conformidade com a **Lei nº 5 (Padrão de Estrangulamento do Legado)**, todos os DTOs e helpers de rede devem ser isolados no projeto compartilhado **`Servidor.Strangler`** sob namespaces modulares.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**

### Antes de começar:
```powershell
git checkout develop
git pull origin develop
git checkout -b feat/estrangulamento-documento-financeiro
```

### Ao finalizar a alteração (após testar a compilação do legado):
```powershell
git add projeto_tag_1906/servidor/objeto de negócio/gestao.financeira/Documento.cs
git add projeto_tag_1906/servidor/objeto de negócio/gestao.financeira/DocumentoFinanceiroBase.cs
git add projeto_tag_1906/servidor/framework/Servidor.Strangler/
git commit -m "feat(gestao-financeira): estrangula validações e parâmetros locais usando projeto Servidor.Strangler"
# NÃO faça merge — deixe a branch para o usuário revisar
```

---

## Contexto

Estamos aplicando o **Padrão de Estrangulamento** (Strangler Fig Pattern) para migrar a inteligência de negócios do sistema legado **Versatus** para o **.NET 8**.
Ao substituir as queries internas das entidades `Documento` e `DocumentoFinanceiroBase` por chamadas à API nova, começamos a desativar as tabelas auxiliares do banco antigo e a direcionar a verdade dos dados para o novo sistema.

---

## Arquivos a Modificar / Criar

### [CRIAR / MIGRAR no projeto Servidor.Strangler]
1. [FinancialStranglerHelper.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Common/FinancialStranglerHelper.cs) (Mover para a pasta `Common` do novo projeto `Servidor.Strangler`)
2. [EntidadeDto.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Gestao.Financeira/DTOs/EntidadeDto.cs)
3. [FornecedorDto.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Gestao.Financeira/DTOs/FornecedorDto.cs)
4. [ClienteDto.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Gestao.Financeira/DTOs/ClienteDto.cs)
5. [ParametroDto.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Gestao.Financeira/DTOs/ParametroDto.cs)

### [MODIFICAR nos objetos de negócio]
6. [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs)
7. [Documento.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/Documento.cs)

---

## Roteiro de Implementação

### Passo 1: Configurar o Projeto `Servidor.Strangler`
1. Crie um projeto Class Library .NET Framework 4.7 na pasta `projeto_tag_1906/servidor/framework/Servidor.Strangler/`.
2. Adicione a referência ao pacote NuGet `Newtonsoft.Json` (v13.0.1 ou compatível).
3. Mova o helper [FinancialStranglerHelper.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/FinancialStranglerHelper.cs) do projeto do financeiro para a pasta `Common/` deste novo projeto.
4. Ajuste o namespace do helper para: `Projeto.Servidor.Strangler.Common`.
5. Adicione uma **Project Reference** de `gestao.financeira` para `Servidor.Strangler`.

### Passo 2: Criar os DTOs Modulares no Novo Projeto
Crie os arquivos DTO dentro de `Servidor.Strangler/Gestao.Financeira/DTOs/` seguindo o namespace `Projeto.Servidor.Strangler.GestaoFinanceira.DTOs`:

* **`EntidadeDto.cs`**:
  ```csharp
  namespace Projeto.Servidor.Strangler.GestaoFinanceira.DTOs
  {
      public class EntidadeDto
      {
          public int IdEntidade { get; set; }
          public bool IsCliente { get; set; }
          public bool IsFornecedor { get; set; }
          public bool IsInstituicaoFinanceira { get; set; }
      }
  }
  ```
* **`FornecedorDto.cs`**:
  ```csharp
  namespace Projeto.Servidor.Strangler.GestaoFinanceira.DTOs
  {
      public class FornecedorDto
      {
          public int IdFornecedor { get; set; }
          public int? IdCondicaoPagamento { get; set; }
      }
  }
  ```
* **`ClienteDto.cs`**:
  ```csharp
  namespace Projeto.Servidor.Strangler.GestaoFinanceira.DTOs
  {
      public class ClienteDto
      {
          public int IdCliente { get; set; }
          public bool? ItemFinanceiroPadrao { get; set; }
          public int? IdCondicaoPagamento { get; set; }
      }
  }
  ```
* **`ParametroDto.cs`**:
  ```csharp
  namespace Projeto.Servidor.Strangler.GestaoFinanceira.DTOs
  {
      public class ParametroDto
      {
          public string Chave { get; set; }
          public string Valor { get; set; }
      }
  }
  ```

### Passo 3: Importar os Namespaces e Estrangular `ValidarEntidade`
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs).
* **Preparação:** Adicione os imports no topo:
  ```csharp
  using Projeto.Servidor.Strangler.Common;
  using Projeto.Servidor.Strangler.GestaoFinanceira.DTOs;
  ```
* **Lógica:** Substitua o método `ValidarEntidade()` original:
  ```csharp
  protected void ValidarEntidade()
  {
      if (this.IdEntidade == 0)
          return;

      string sTipo = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(PagarReceberTipo), this.PagarReceber, this.Ambiente);
      string msg = null;

      try
      {
          // Consome a API do .NET 8 via Helper compartilhado
          var entidade = FinancialStranglerHelper.GetJson<EntidadeDto>("entidade/" + this.IdEntidade);

          if (entidade == null)
          {
              throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, "Entidade informada não existe na API principal.");
          }

          if (this.PagarReceber == PagarReceberTipo.Pagar && !entidade.IsFornecedor)
          {
              msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Fornecedor'.", sTipo);
              throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
          }

          if (this.PagarReceber == PagarReceberTipo.Receber && !entidade.IsCliente)
          {
              msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Cliente'.", sTipo);
              throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
          }

          if (this.PagarReceber == PagarReceberTipo.MovimentoCartao && !entidade.IsInstituicaoFinanceira)
          {
              msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Instituição financeira'.", sTipo);
              throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
          }
      }
      catch (Exception e)
      {
          if (e is ObjetoNegocioException) throw;
          throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, "Erro ao validar entidade via API: " + e.Message);
      }
  }
  ```

### Passo 4: Estrangular `CarregarCondicaoPagamentoDefault`
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) no método `CarregarCondicaoPagamentoDefault()`.
* **Alteração requerida:** Substituir pela leitura HTTP:
  ```csharp
  private void CarregarCondicaoPagamentoDefault()
  {
      bool setPortador = true;

      try
      {
          if (this.IdEntidade == 0 || this.PagarReceber == PagarReceberTipo.MovimentoCartao)
              return;

          int? idCondicao = null;

          if (this.PagarReceber == PagarReceberTipo.Pagar)
          {
              var f = FinancialStranglerHelper.GetJson<FornecedorDto>("fornecedor/" + this.IdEntidade);
              if (f != null && f.IdCondicaoPagamento.HasValue)
              {
                  idCondicao = f.IdCondicaoPagamento;
              }
          }
          else
          {
              var ce = FinancialStranglerHelper.GetJson<ClienteDto>("cliente/" + this.IdEntidade);
              if (ce != null && ce.IdCondicaoPagamento.HasValue)
              {
                  idCondicao = ce.IdCondicaoPagamento;
              }
          }

          if (idCondicao.HasValue && idCondicao.Value > 0)
          {
              this.IdCondicaoPagamento = idCondicao.Value;
              setPortador = false;
          }
      }
      catch (Exception e)
      {
          setPortador = false;
          ControleErro.TratarErro(e, false, true);
      }
      finally
      {
          if (setPortador)
              this.IdPortador = GetPortadorDefault();
      }
  }
  ```

### Passo 5: Estrangular Parâmetros Globais e Portador Padrão
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) no método `GetPortadorDefault()`.
* **Alteração requerida:** Consumir parâmetros globais via `GET parametro/PortadorPadrao`:
  ```csharp
  protected int GetPortadorDefault()
  {
      int idPortadorDefault = 0;
      try
      {
          var param = FinancialStranglerHelper.GetJson<ParametroDto>("parametro/PortadorPadrao");
          if (param != null && !string.IsNullOrEmpty(param.Valor))
          {
              idPortadorDefault = Convert.ToInt32(param.Valor);
          }
      }
      catch
      {
          idPortadorDefault = 1; // ID padrão fallback
      }

      return idPortadorDefault;        
  }
  ```

### Passo 6: Estrangular `PermiteItemFinanceiroPadrao`
* **Local:** [Documento.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/Documento.cs).
* **Preparação:** Adicione os imports no topo:
  ```csharp
  using Projeto.Servidor.Strangler.Common;
  using Projeto.Servidor.Strangler.GestaoFinanceira.DTOs;
  ```
* **Alteração requerida:** Substituir o método `PermiteItemFinanceiroPadrao()`:
  ```csharp
  private bool PermiteItemFinanceiroPadrao()
  {
      if (this.PagarReceber != PagarReceberTipo.Receber || this.IdEntidade == 0)
          return false;

      try
      {
          var cliente = FinancialStranglerHelper.GetJson<ClienteDto>("cliente/" + this.IdEntidade);
          return cliente != null && cliente.ItemFinanceiroPadrao.HasValue && cliente.ItemFinanceiroPadrao.Value;
      }
      catch
      {
          return false;
      }
  }
  ```

---

## Verificação e Testes

Ao finalizar as alterações:
1. Compile a solução legada inteira, garantindo que o projeto `Servidor.Strangler` compile primeiro e seja referenciado sem avisos pelo `gestao.financeira`.
2. Certifique-se de que a API .NET 8 está ativa e responda de forma condizente aos endpoints chamados.
3. Submeta a branch para revisão do usuário após o build com sucesso.
