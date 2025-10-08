IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'StockDamage')
BEGIN
    EXEC('CREATE DATABASE StockDamage');
END
GO

USE StockDamage;
GO

IF OBJECT_ID('dbo.Godown', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Godown
    (
        GodownNo INT NOT NULL PRIMARY KEY,
        GodownName NVARCHAR(100) NOT NULL,
        AutoSlNo INT IDENTITY(1,1) NOT NULL
    );

    INSERT INTO dbo.Godown (GodownNo, GodownName)
    VALUES (1, 'Central Warehouse'),
           (2, 'Secondary Warehouse');
END
GO

IF OBJECT_ID('dbo.SubItemCode', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubItemCode
    (
        AutoSlNo INT IDENTITY(1,1) PRIMARY KEY,
        SubItemCode NVARCHAR(50) NOT NULL UNIQUE,
        SubItemName NVARCHAR(150) NOT NULL,
        Unit NVARCHAR(50) NOT NULL,
        Weight DECIMAL(18,3) NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    INSERT INTO dbo.SubItemCode (SubItemCode, SubItemName, Unit, Weight)
    VALUES ('ITM-001', 'Damaged Monitor', 'PCS', 5.5),
           ('ITM-002', 'Broken Keyboard', 'PCS', 0.6);
END
GO

IF OBJECT_ID('dbo.Stock', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Stock
    (
        SubItemCode NVARCHAR(50) NOT NULL PRIMARY KEY,
        Stock DECIMAL(18,2) NOT NULL DEFAULT 0
    );

    INSERT INTO dbo.Stock (SubItemCode, Stock)
    VALUES ('ITM-001', 10),
           ('ITM-002', 20);
END
GO

IF OBJECT_ID('dbo.Currency', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Currency
    (
        CurrencyName NVARCHAR(50) NOT NULL PRIMARY KEY,
        ConversionRate DECIMAL(18,4) NOT NULL
    );

    INSERT INTO dbo.Currency (CurrencyName, ConversionRate)
    VALUES ('BDT', 1.00),
           ('USD', 109.50),
           ('EUR', 117.30);
END
GO

IF OBJECT_ID('dbo.Employee', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employee
    (
        EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeName NVARCHAR(120) NOT NULL
    );

    INSERT INTO dbo.Employee (EmployeeName)
    VALUES ('Ariful Islam'),
           ('Nusrat Jahan'),
           ('Karim Uddin');
END
GO

IF OBJECT_ID('dbo.StockDamage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.StockDamage
    (
        StockDamageId INT IDENTITY(1,1) PRIMARY KEY,
        EntryDate DATE NOT NULL,
        VoucherNo NVARCHAR(50) NULL,
        DrAccountHead NVARCHAR(100) NOT NULL,
        GodownNo INT NOT NULL,
        WarehouseName NVARCHAR(100) NOT NULL,
        SubItemCode NVARCHAR(50) NOT NULL,
        SubItemName NVARCHAR(150) NOT NULL,
        Unit NVARCHAR(50) NOT NULL,
        Stock DECIMAL(18,2) NOT NULL,
        BatchNo NVARCHAR(50) NOT NULL DEFAULT 'NA',
        CurrencyName NVARCHAR(50) NOT NULL,
        CurrencyRate DECIMAL(18,4) NOT NULL,
        Quantity DECIMAL(18,2) NOT NULL,
        Rate DECIMAL(18,2) NOT NULL,
        AmountIn DECIMAL(18,2) NOT NULL,
        EmployeeId INT NOT NULL,
        Comments NVARCHAR(500) NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_StockDamage_Godown FOREIGN KEY (GodownNo) REFERENCES dbo.Godown(GodownNo),
        CONSTRAINT FK_StockDamage_SubItem FOREIGN KEY (SubItemCode) REFERENCES dbo.SubItemCode(SubItemCode),
        CONSTRAINT FK_StockDamage_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(EmployeeId)
    );
END
GO

IF OBJECT_ID('dbo.SP_StockDamage_Save', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.SP_StockDamage_Save;
END
GO

CREATE PROCEDURE dbo.SP_StockDamage_Save
    @Date DATE,
    @VoucherNo NVARCHAR(50) = NULL,
    @DrAccountHead NVARCHAR(100),
    @GodownNo INT,
    @WarehouseName NVARCHAR(100),
    @SubItemCode NVARCHAR(50),
    @SubItemName NVARCHAR(150),
    @Unit NVARCHAR(50),
    @Stock DECIMAL(18,2),
    @BatchNo NVARCHAR(50),
    @CurrencyName NVARCHAR(50),
    @CurrencyRate DECIMAL(18,4),
    @Quantity DECIMAL(18,2),
    @Rate DECIMAL(18,2),
    @AmountIn DECIMAL(18,2),
    @EmployeeId INT,
    @Comments NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.StockDamage
    (
        EntryDate,
        VoucherNo,
        DrAccountHead,
        GodownNo,
        WarehouseName,
        SubItemCode,
        SubItemName,
        Unit,
        Stock,
        BatchNo,
        CurrencyName,
        CurrencyRate,
        Quantity,
        Rate,
        AmountIn,
        EmployeeId,
        Comments
    )
    VALUES
    (
        @Date,
        @VoucherNo,
        @DrAccountHead,
        @GodownNo,
        @WarehouseName,
        @SubItemCode,
        @SubItemName,
        @Unit,
        @Stock,
        @BatchNo,
        @CurrencyName,
        @CurrencyRate,
        @Quantity,
        @Rate,
        @AmountIn,
        @EmployeeId,
        @Comments
    );
END
GO
