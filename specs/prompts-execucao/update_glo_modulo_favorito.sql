IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'PrefixoRota')
    ALTER TABLE GloModulo ADD PrefixoRota VARCHAR(100) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'CorHex')
    ALTER TABLE GloModulo ADD CorHex VARCHAR(7) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'IconeMui')
    ALTER TABLE GloModulo ADD IconeMui VARCHAR(50) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloRotina') AND name = 'RotaWeb')
    ALTER TABLE GloRotina ADD RotaWeb VARCHAR(100) NULL;
GO

UPDATE GloModulo SET PrefixoRota='/acesso-global',    CorHex='#637381', IconeMui='ManageAccounts'     WHERE IdGloModulo = 1;
UPDATE GloModulo SET PrefixoRota='/financeiro',       CorHex='#10B981', IconeMui='AccountBalance'     WHERE IdGloModulo = 2;
UPDATE GloModulo SET PrefixoRota='/faturamento',      CorHex='#2065D1', IconeMui='ReceiptLong'        WHERE IdGloModulo = 3;
UPDATE GloModulo SET PrefixoRota='/material',         CorHex='#8B5CF6', IconeMui='Inventory2'         WHERE IdGloModulo = 4;
UPDATE GloModulo SET PrefixoRota='/compras',          CorHex='#06B6D4', IconeMui='ShoppingCart'       WHERE IdGloModulo = 5;
UPDATE GloModulo SET PrefixoRota='/tributos',         CorHex='#EF4444', IconeMui='Gavel'              WHERE IdGloModulo = 6;
UPDATE GloModulo SET PrefixoRota='/rh',               CorHex='#F59E0B', IconeMui='People'             WHERE IdGloModulo = 7;
UPDATE GloModulo SET PrefixoRota='/os',               CorHex='#F97316', IconeMui='Build'              WHERE IdGloModulo = 8;
UPDATE GloModulo SET PrefixoRota='/ativo-fixo',       CorHex='#64748B', IconeMui='Apartment'          WHERE IdGloModulo = 9;
UPDATE GloModulo SET PrefixoRota='/logistica',        CorHex='#0EA5E9', IconeMui='LocalShipping'      WHERE IdGloModulo = 10;
UPDATE GloModulo SET PrefixoRota='/mrp',              CorHex='#7C3AED', IconeMui='PrecisionManufacturing' WHERE IdGloModulo = 11;
UPDATE GloModulo SET PrefixoRota='/contrato',         CorHex='#DB2777', IconeMui='Description'        WHERE IdGloModulo = 12;
UPDATE GloModulo SET PrefixoRota='/frota',            CorHex='#16A34A', IconeMui='LocalShipping'      WHERE IdGloModulo = 13;
UPDATE GloModulo SET PrefixoRota='/contabil',         CorHex='#0891B2', IconeMui='BarChart'           WHERE IdGloModulo = 14;
UPDATE GloModulo SET PrefixoRota='/versatus',         CorHex='#1E293B', IconeMui='AdminPanelSettings' WHERE IdGloModulo = 15;
UPDATE GloModulo SET PrefixoRota='/obra',             CorHex='#92400E', IconeMui='Engineering'        WHERE IdGloModulo = 16;
UPDATE GloModulo SET PrefixoRota='/pdv',              CorHex='#DC2626', IconeMui='PointOfSale'        WHERE IdGloModulo = 17;
UPDATE GloModulo SET PrefixoRota='/garagem',          CorHex='#15803D', IconeMui='DirectionsCar'      WHERE IdGloModulo = 18;
UPDATE GloModulo SET PrefixoRota='/small',            CorHex='#4338CA', IconeMui='Storefront'         WHERE IdGloModulo = 19;
UPDATE GloModulo SET PrefixoRota='/pesagem',          CorHex='#B45309', IconeMui='Scale'              WHERE IdGloModulo = 20;
UPDATE GloModulo SET PrefixoRota='/locacao',          CorHex='#0E7490', IconeMui='MeetingRoom'        WHERE IdGloModulo = 21;
UPDATE GloModulo SET PrefixoRota='/ecommerce',        CorHex='#BE185D', IconeMui='ShoppingBag'        WHERE IdGloModulo = 22;
UPDATE GloModulo SET PrefixoRota='/educacional',      CorHex='#1D4ED8', IconeMui='School'             WHERE IdGloModulo = 23;
UPDATE GloModulo SET PrefixoRota='/transporte',       CorHex='#065F46', IconeMui='DirectionsBus'      WHERE IdGloModulo = 24;
UPDATE GloModulo SET PrefixoRota='/producao',         CorHex='#7C2D12', IconeMui='Factory'            WHERE IdGloModulo = 25;
UPDATE GloModulo SET PrefixoRota='/mdfe',             CorHex='#374151', IconeMui='Article'            WHERE IdGloModulo = 26;
UPDATE GloModulo SET PrefixoRota='/armazem',          CorHex='#78350F', IconeMui='Warehouse'          WHERE IdGloModulo = 27;
UPDATE GloModulo SET PrefixoRota='/nfse',             CorHex='#0369A1', IconeMui='Receipt'            WHERE IdGloModulo = 28;
UPDATE GloModulo SET PrefixoRota='/epay',             CorHex='#065F46', IconeMui='Payment'            WHERE IdGloModulo = 29;
GO

-- Rotas migradas no módulo Acesso Global
UPDATE GloRotina SET RotaWeb = 'entidade'           WHERE IdGloRotina = 23;
UPDATE GloRotina SET RotaWeb = 'condicao-pagamento' WHERE IdGloRotina = 24;
UPDATE GloRotina SET RotaWeb = 'parametro'          WHERE IdGloRotina = 25;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GloFavorito')
BEGIN
    CREATE TABLE GloFavorito (
        IdGloFavorito INT IDENTITY(1,1) PRIMARY KEY,
        IdUsuario     INT NOT NULL DEFAULT 1,
        IdGloRotina   INT NOT NULL,
        Ordem         INT NOT NULL DEFAULT 0,
        CONSTRAINT FK_GloFavorito_GloRotina FOREIGN KEY (IdGloRotina) REFERENCES GloRotina(IdGloRotina)
    );
    CREATE INDEX IX_GloFavorito_Usuario ON GloFavorito(IdUsuario);
END
GO
