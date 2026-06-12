# Prompt de Execução — MOD-05: Estrangulamento de Documento Financeiro (Legado)

> **Para a IA executora:** Sua tarefa aqui é **modificar os arquivos C# no projeto legado** (`projeto_tag_1906`) para substituir a lógica interna de consultas ao banco via Gentle.NET por chamadas HTTP para a nova API do .NET 8.
> Utilize o [FinancialStranglerHelper.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/FinancialStranglerHelper.cs) já criado e configurado para realizar as requisições.

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
git commit -m "feat(gestao-financeira): estrangula validações e parâmetros locais via chamadas à API .NET 8"
# NÃO faça merge — deixe a branch para o usuário revisar
```

---

## Contexto

Estamos aplicando o **Padrão de Estrangulamento** (Strangler Fig Pattern) para migrar a inteligência de negócios do sistema legado **Versatus** para o **.NET 8**.
Ao substituir as queries internas das entidades `Documento` e `DocumentoFinanceiroBase` por chamadas à API nova, começamos a desativar as tabelas auxiliares do banco antigo e a direcionar a verdade dos dados para o novo sistema.

---

## Arquivos a Modificar

1. [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs)
2. [Documento.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/Documento.cs)

---

## Roteiro de Implementação

### Passo 1: Adicionar os DTOs de Desserialização
No final do arquivo [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) (dentro do namespace `Projeto.Servidor.ObjetosNegocio.GestaoFinanceira`), declare os DTOs para mapear as respostas do JSON retornado pelo .NET 8:

```csharp
public class EntidadeDto
{
    public int IdEntidade { get; set; }
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public bool IsInstituicaoFinanceira { get; set; }
}

public class FornecedorDto
{
    public int IdFornecedor { get; set; }
    public int? IdCondicaoPagamento { get; set; }
}

public class ClienteDto
{
    public int IdCliente { get; set; }
    public bool? ItemFinanceiroPadrao { get; set; }
    public int? IdCondicaoPagamento { get; set; }
}

public class ParametroDto
{
    public string Chave { get; set; }
    public string Valor { get; set; }
}
```

### Passo 2: Estrangular `ValidarEntidade`
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) no método `ValidarEntidade()`.
* **Código Legado:**
  ```csharp
  if (this.PagarReceber == PagarReceberTipo.Pagar)
  {
      if (!this.Entidade.Fornecedor)
      {
          msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Fornecedor'.", sTipo);
          throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg); 
      }
  }
  ```
* **Alteração requerida:** Substitua a verificação por uma chamada HTTP utilizando `FinancialStranglerHelper`:
  ```csharp
  protected void ValidarEntidade()
  {
      if (this.IdEntidade == 0)
          return;

      string sTipo = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(PagarReceberTipo), this.PagarReceber, this.Ambiente);
      string msg = null;

      try
      {
          // Chamada para a API nova .NET 8
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

### Passo 3: Estrangular `CarregarCondicaoPagamentoDefault`
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) no método `CarregarCondicaoPagamentoDefault()`.
* **Código Legado:** Utilizava `Factory.Retornar(typeof(IFornecedor), ...)` e `Factory.Retornar(typeof(IClienteEmpresa), ...)`.
* **Alteração requerida:** Mudar para chamadas HTTP correspondentes:
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

### Passo 4: Estrangular Parâmetros Globais e Portador Padrão
* **Local:** [DocumentoFinanceiroBase.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/DocumentoFinanceiroBase.cs) no método `GetPortadorDefault()`.
* **Código Legado:**
  ```csharp
  int idPortadorDefault = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.PortadorPadrao, this.Ambiente).ValorInt;
  ```
* **Alteração requerida:** Substituir pela chamada do endpoint de parâmetros da nova API:
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
          // Fallback caso falhe a API
          idPortadorDefault = 1; // ID padrão de portador
      }

      if (this.IdEntidade == 0)
          return idPortadorDefault;

      if (this.PagarReceber == PagarReceberTipo.Receber)
      {
          try
          {
              var cli = FinancialStranglerHelper.GetJson<ClienteDto>("cliente/" + this.IdEntidade);
              // Se houver portador específico mapeado no DTO, pode-se extrair
          }
          catch
          {
              // Ignora e usa padrão
          }
      }

      return idPortadorDefault;        
  }
  ```

### Passo 5: Estrangular `PermiteItemFinanceiroPadrao`
* **Local:** [Documento.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/projeto_tag_1906/servidor/objeto%20de%20neg%C3%B3cio/gestao.financeira/Documento.cs) no método `PermiteItemFinanceiroPadrao()`.
* **Código Legado:**
  ```csharp
  IClienteConsulta cc = (IClienteConsulta)Factory.Retornar(typeof(IClienteConsulta), this.Ambiente, this.IdEntidade);
  return cc.ItemFinanceiroPadrao;
  ```
* **Alteração requerida:** Substituir pelo consumo da API de Clientes:
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
1. Abra a solução legada ou execute o build do projeto `gestao.financeira` usando o MSBuild para garantir que compila sem erros.
2. Certifique-se de que a API do .NET 8 esteja rodando em `http://localhost:5000` (ou na porta correspondente) e teste a abertura de um documento financeiro no legado para validar que as chamadas HTTP respondem perfeitamente.
3. Se todas as validações de compilação passarem, submeta a branch para revisão.
