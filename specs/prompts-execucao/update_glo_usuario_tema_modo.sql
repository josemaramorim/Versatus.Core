-- Script SQL para adicionar a coluna TemaModo na tabela GloUsuario (SQL Server 2008+)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[GloUsuario]') 
      AND name = N'TemaModo'
)
BEGIN
    ALTER TABLE [dbo].[GloUsuario] 
    ADD [TemaModo] VARCHAR(10) NULL;

    PRINT 'Coluna TemaModo adicionada com sucesso na tabela GloUsuario.';
END
ELSE
BEGIN
    PRINT 'Coluna TemaModo já existe na tabela GloUsuario.';
END
GO
