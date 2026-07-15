# Guia de Deploy no Painel ICP (iContainer)

Este guia documenta o passo a passo para a publicação da API (Back-end .NET 10) e do Frontend (React + Vite) no Painel ICP via integração com o GitHub, incluindo as variáveis de ambiente necessárias e seus respectivos valores padrões.

---

## 📌 Sumário
1. [Configurações Prévias no GitHub](#1-configurações-prévias-no-github)
2. [Deploy da API (Back-end)](#2-deploy-da-api-back-end)
   * [Variáveis de Ambiente Necessárias (API)](#variáveis-de-ambiente-necessárias-api)
3. [Deploy do Frontend (React + Vite)](#3-deploy-do-frontend-react--vite)
   * [Variáveis de Ambiente do Frontend (Opcional)](#variáveis-de-ambiente-do-frontend-opcional)
4. [Como Testar e Validar a API (Swagger)](#4-como-testar-e-validar-a-api-swagger)

---

## 🔑 1. Configurações Prévias no GitHub
Antes de iniciar no painel ICP, crie um **Fine-grained Personal Access Token** no GitHub (Settings -> Developer Settings -> Personal access tokens -> Fine-grained tokens) com as seguintes permissões para o repositório `josemaramorim/Versatus.Core`:
* **Repository permissions -> Webhooks:** `Read and Write`
* **Repository permissions -> Contents:** `Read and Write`

Cadastre este Token no Painel ICP em **Configurações -> Aba Integrações -> Adicionar credencial**.

---

## 🖥️ 2. Deploy da API (Back-end)

Crie uma aplicação Standalone no painel ICP (**Aplicações -> Standalone**):

* **Nome da Aplicação:** `versatus-api`
* **Tecnologia (Aplicação):** `DotNet`
* **Versão do .NET:** `10.0`
* **Domínio Principal:** O subdomínio da API (ex: `api.seudominio.com`)
* **Origem dos Arquivos:** `GitHub`
* **Credencial GitHub:** Selecione a credencial criada no painel.
* **Repositório:** `josemaramorim/Versatus.Core`
* **Branch:** `develop`
* **Pasta do Projeto:** `src/Versatus.WebAPI` *(Certifique-se de incluir o **I** no final, pois o nome é WebAPI e não WebAP)*
* **Comando de Build:** `dotnet publish src/Versatus.WebAPI/Versatus.WebAPI.csproj -c Release -o /app` *(se o painel solicitar)*
* **Script de Execução:** `dotnet /app/Versatus.WebAPI.dll` *(ou `dotnet Versatus.WebAPI.dll`, caso a saída não seja mapeada para /app)*
* **Porta da Aplicação / Porta Externa:** `8080` (Porta padrão do .NET Core em contêineres) ou `5105` (se desejar manter a porta de desenvolvimento local).

### 🌐 Variáveis de Ambiente Necessárias (API)

Defina as seguintes chaves de configuração na seção de variáveis de ambiente do painel ICP:

| Nome da Variável | Valor Padrão (Local Fallback) | Descrição |
| :--- | :--- | :--- |
| `CONNECTIONSTRINGS__DEFAULTCONNECTION` | `Server=localhost\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;` | String de conexão para o banco de dados SQL Server remoto no iContainer. |
| `SECURITY__STRANGLERAPIKEY` | `SUA_CHAVE_SUPER_SECRETA_DE_INTEGRACAO_2026` | API Key de segurança para autenticação das integrações de microsserviços. |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Define o ambiente de execução da aplicação ASP.NET Core (ex: `Production` ou `Development`). |
| `ASPNETCORE_URLS` | `http://+:8080` | Define a porta e host em que o Kestrel escutará (caso queira trocar para a porta `5105`, defina como `http://+:5105`). |

> 💡 **Dica de Preenchimento para a `CONNECTIONSTRINGS__DEFAULTCONNECTION` no ICP:**
> * **Opção Recomendada (Conexão Interna Docker):**
>   `Server=sql-server-2022;Database=versatus;User Id=sa;Password=V#v070804S;TrustServerCertificate=True;`
> * **Opção Alternativa (IP Externo da VPS):**
>   `Server=IP_DO_SEU_SERVIDOR_VPS;Database=versatus;User Id=sa;Password=V#v070804S;TrustServerCertificate=True;`
> * ⚠️ *Atenção à senha: no ICP a senha é configurada com **S** maiúsculo (`V#v070804S`), diferentemente da senha local.*

---

## 🎨 3. Deploy do Frontend (React + Vite)

Crie uma aplicação Standalone no painel ICP (**Aplicações -> Standalone**):

* **Nome da Aplicação:** `versatus-frontend`
* **Tecnologia (Aplicação):** `NodeJS` (Versão `20` ou superior)
* **Domínio Principal:** O domínio de acesso dos usuários (ex: `app.seudominio.com`)
* **Origem dos Arquivos:** `GitHub`
* **Credencial GitHub:** Selecione a credencial criada no painel.
* **Repositório:** `josemaramorim/Versatus.Core`
* **Branch:** `develop`
* **Pasta do Projeto:** `src/Versatus.Frontend`
* **Comando de Build:**
  ```bash
  npm install && npm run build
  ```
* **Script de Execução:**
  ```bash
  npx serve -s dist -l 3000
  ```
* **Porta da Aplicação / Porta Externa:** `3000`

### 🌐 Variáveis de Ambiente do Frontend (Opcional)
* Em ambientes baseados em contêineres, as chamadas para a API (ex: `/api/entidade`) são comumente redirecionadas através do Proxy do próprio servidor web (Nginx/Apache) configurado no painel ICP.
* Mapeie a rota `/api` do domínio do Frontend (`app.seudominio.com/api`) para apontar diretamente para o endereço da API (`https://api.seudominio.com/api`).

---

## 🔍 4. Como Testar e Validar a API (Swagger)
Para verificar se a API foi publicada com sucesso e está respondendo:

1. **Acesso ao Swagger:**
   * Abra seu navegador no endereço: `https://api.seudominio.com/swagger` *(certifique-se de digitar com a letra **r** no final)*.
   * A página da documentação interativa deverá carregar exibindo os endpoints.
2. **Comportamento Esperado de Bloqueio (Erro 401):**
   * Se tentar acessar um endpoint de dados diretamente (ex: `https://api.seudominio.com/api/entidade/paginado`), a API retornará a mensagem em formato de texto limpo UTF-8:
     > `Acesso não autorizado via API Key.`
   * Isto confirma que o contêiner está rodando e a segurança da API está ativa.

