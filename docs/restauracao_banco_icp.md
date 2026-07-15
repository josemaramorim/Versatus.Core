# Guia de Restauração de Banco de Dados no SQL Server (ICP / Linux Docker)

Este documento descreve o procedimento passo a passo para restaurar um arquivo de backup do SQL Server (`.bak`) local no servidor de banco de dados rodando em contêiner Docker/Linux no Painel ICP.

---

## 📋 Pré-requisitos
* Um arquivo de backup válido (ex: `vedas_banco_ok.bak`).
* Acesso FTP/SFTP ou Gerenciador de Arquivos do Painel ICP para enviar o arquivo para o servidor VPS.
* Acesso SSH (ou console de terminal) ao servidor VPS onde o contêiner do SQL Server está rodando.
* Microsoft SQL Server Management Studio (SSMS) conectado ao banco remoto.

---

## 🚀 Passo a Passo

### Passo 1: Enviar o arquivo `.bak` para a máquina Linux
1. Utilizando seu cliente de FTP (como o FileZilla) ou SSH (SCP), envie o arquivo de backup (`vedas_banco_ok.bak`) para o servidor do ICP.
2. Normalmente, o arquivo será salvo no diretório raiz do seu usuário (ex: `/root/` ou `/home/seu-usuario/`).

---

### Passo 2: Copiar o arquivo `.bak` para dentro do contêiner SQL Server
Como o SQL Server roda isolado dentro de um contêiner Docker, ele não consegue ver os arquivos na pasta do seu usuário do Linux diretamente. Precisamos copiar o arquivo para dentro dele.

1. No terminal do servidor VPS Linux, descubra o nome do contêiner rodando o SQL Server:
   ```bash
   docker ps
   ```
   *Anote o nome do contêiner da coluna **NAMES** (ex: `mssql-server`).*

2. Copie o arquivo `.bak` para a pasta interna de dados do contêiner (onde o SQL Server tem permissão de leitura):
   ```bash
   docker cp vedas_banco_ok.bak <NOME_DO_CONTAINER>:/var/opt/mssql/data/vedas_banco_ok.bak
   ```
   *(Substitua `<NOME_DO_CONTAINER>` pelo nome anotado no passo anterior).*

---

### Passo 3: Identificar os Nomes Lógicos do Backup
Os arquivos dentro do backup possuem nomes lógicos internos. Precisamos identificá-los para realizar o mapeamento correto dos caminhos de arquivos do Windows para as pastas do Linux.

1. Conecte-se ao SQL Server do ICP pelo SSMS no seu computador.
2. Abra uma nova janela de consulta (**New Query**) e execute:
   ```sql
   RESTORE FILELISTONLY FROM DISK = '/var/opt/mssql/data/vedas_banco_ok.bak';
   ```
3. Anote os valores exibidos na coluna **LogicalName** para o arquivo de dados (tipo `D`) e o arquivo de log (tipo `L`). 
   * *Exemplo obtido para o backup `vedas_banco_ok.bak`:*
     * Arquivo de Dados (D): `apresentacao`
     * Arquivo de Log (L): `apresentacao_log`

---

### Passo 4: Executar o Comando de Restauração (Restore)
Com os nomes lógicos mapeados, execute o script abaixo para realizar a restauração completa. 

> ⚠️ **Atenção:** O script força a desconexão de usuários ativos (como a WebAPI conectada) definindo o banco como `SINGLE_USER` temporariamente. Isso evita o erro de "Banco de dados em uso".

```sql
USE master;
GO

-- 1. Força a desconexão de todos os usuários do banco 'versatus'
ALTER DATABASE versatus SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- 2. Executa a restauração mapeando os nomes lógicos internos do backup
RESTORE DATABASE versatus
FROM DISK = '/var/opt/mssql/data/vedas_banco_ok.bak'
WITH 
  MOVE 'apresentacao' TO '/var/opt/mssql/data/versatus.mdf',
  MOVE 'apresentacao_log' TO '/var/opt/mssql/data/versatus_log.ldf',
  REPLACE;
GO

-- 3. Devolve o banco ao modo multi-usuário (acesso normal)
ALTER DATABASE versatus SET MULTI_USER;
GO
```

---

## 🛠️ Resolução de Erros Comuns

### Erro 1: CREATE FILE encountered operating system error 3 (Caminho não encontrado)
* **Causa:** Ocorre ao tentar usar o assistente visual de criação ou restauração do SSMS no Windows. O SSMS tenta concatenar caminhos do Windows usando barra invertida (`\`) no Linux (ex: `/var/opt/mssql/data\versatus.mdf`).
* **Solução:** Evite a interface visual do SSMS. Sempre realize a criação e restauração usando comandos SQL diretos (Scripts de Query) conforme ensinado acima.

### Erro 2: Msg 3102 - RESTORE cannot process database porque está em uso
* **Causa:** Há conexões abertas no banco que você quer sobrescrever (sua própria janela de query está usando o banco ou a WebAPI do sistema está conectada nele).
* **Solução:** Execute `USE master;` no início da query e use o comando `ALTER DATABASE versatus SET SINGLE_USER WITH ROLLBACK IMMEDIATE;` para derrubar as sessões ativas antes de rodar o `RESTORE`.
