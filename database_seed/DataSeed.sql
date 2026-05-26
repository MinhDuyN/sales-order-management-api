
-- Test Accounts:
--   Admin    : admin@erp.com    / admin@erp
--   Staff    : staff@erp.com    / staff@erp
--   Customer : customer@erp.com / customer@erp

USE [master]
GO

IF DB_ID(N'BackendLearningDB') IS NOT NULL
BEGIN
    ALTER DATABASE [BackendLearningDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [BackendLearningDB];
END
GO

CREATE DATABASE [BackendLearningDB];
GO
USE [BackendLearningDB];
GO

-- ================================================================
-- TABLES
-- ================================================================

CREATE TABLE [Roles] (
    [Id]       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE [Users] (
    [Id]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]         NVARCHAR(100) NOT NULL,
    [Email]        NVARCHAR(150) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NULL,
    [RoleId]       INT NOT NULL,
    [IsActive]     BIT NOT NULL DEFAULT 1,
    [CreatedDate]  DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
);
GO

CREATE TABLE [Categories] (
    [Id]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL UNIQUE,
    [CreatedDate]  DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE [Products] (
    [Id]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProductName]   NVARCHAR(150) NOT NULL,
    [Price]         DECIMAL(18,2) NOT NULL,
    [StockQuantity] INT NOT NULL DEFAULT 0,
    [CategoryId]    INT NOT NULL,
    [IsActive]      BIT NOT NULL DEFAULT 1,
    [CreatedDate]   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id])
);
GO

CREATE TABLE [Orders] (
    [Id]          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId]      INT NOT NULL,
    [Status]      NVARCHAR(50) NOT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Orders_Users FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);
GO

CREATE TABLE [OrderItems] (
    [Id]        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrderId]   INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity]  INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [LineTotal] DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders   FOREIGN KEY ([OrderId])   REFERENCES [Orders]([Id]),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
);
GO

CREATE TABLE [Payments] (
    [Id]          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrderId]     INT NOT NULL UNIQUE,
    [Amount]      DECIMAL(18,2) NOT NULL,
    [Status]      NVARCHAR(50) NOT NULL,
    [Method]      NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id])
);
GO

CREATE TABLE [RefreshTokens] (
    [Id]          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId]      INT NOT NULL,
    [Token]       NVARCHAR(500) NOT NULL,
    [ExpiresAt]   DATETIME2 NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);
GO

-- ROLES
INSERT INTO [Roles] ([RoleName]) VALUES ('Admin'), ('Staff'), ('Customer');
GO

-- USERS
-- Password: admin@erp / staff@erp / customer@erp (BCrypt hash)
SET IDENTITY_INSERT [Users] ON;

INSERT INTO [Users] ([Id],[Name],[Email],[PasswordHash],[RoleId],[IsActive],[CreatedDate]) VALUES
(1, N'Admin',      N'admin@erp.com',     N'$2b$11$KX/WvjwT3QLGYfWoZW6BZOPesGBKjpdBgwr0ROjpKcmcIafB/GJLe', 1, 1, '2026-01-01'),
(2, N'Staff',      N'staff@erp.com',     N'$2b$11$ydesDoqEWgBA9KeURxNx4OpoEDizGqdxmOVFHsL1Kv7jVM8jFC2i2', 2, 1, '2026-01-01'),
(3, N'Customer A', N'customer@erp.com',  N'$2b$11$0yrJlKMSwFMT0WWXYXESx.vc9IkQP2Shh2apz2n32BsEOphd3ltXW', 3, 1, '2026-01-15'),
(4, N'Customer B', N'customer2@erp.com', N'$2b$11$Sw0n768/e.vBLpxoWQDslewtWSUW9UK5OKeR3em7RVOX83oHB.zAm', 3, 1, '2026-02-01');

SET IDENTITY_INSERT [Users] OFF;
GO

-- CATEGORIES
SET IDENTITY_INSERT [Categories] ON;

INSERT INTO [Categories] ([Id],[CategoryName],[CreatedDate]) VALUES
(1, N'Laptop',      '2026-01-01'),
(2, N'Điện thoại',  '2026-01-01'),
(3, N'Phụ kiện',    '2026-01-01'),
(4, N'Màn hình',    '2026-01-01');

SET IDENTITY_INSERT [Categories] OFF;
GO

-- PRODUCTS
SET IDENTITY_INSERT [Products] ON;

INSERT INTO [Products] ([Id],[ProductName],[Price],[StockQuantity],[CategoryId],[IsActive],[CreatedDate]) VALUES
-- Laptop (CategoryId=1)
(1,  N'MacBook Air M2',             28000000, 20, 1, 1, '2026-01-01'),
(2,  N'Dell XPS 13',                25000000, 15, 1, 1, '2026-01-01'),
(3,  N'Asus ZenBook 14',            18000000, 30, 1, 1, '2026-01-01'),
-- Điện thoại (CategoryId=2)
(4,  N'iPhone 15 Pro',              30000000, 50, 2, 1, '2026-01-01'),
(5,  N'Samsung Galaxy S24',         22000000, 40, 2, 1, '2026-01-01'),
(6,  N'Xiaomi 14',                  15000000, 35, 2, 1, '2026-01-01'),
-- Phụ kiện (CategoryId=3)
(7,  N'Chuột Logitech MX Master 3',  2500000, 60, 3, 1, '2026-01-01'),
(8,  N'Bàn phím Keychron K2',        2200000, 45, 3, 1, '2026-01-01'),
(9,  N'Tai nghe Sony WH-1000XM5',    8000000, 25, 3, 1, '2026-01-01'),
-- Màn hình (CategoryId=4)
(10, N'LG UltraWide 34"',           12000000, 10, 4, 1, '2026-01-01'),
(11, N'Dell U2723D 27"',            10500000, 12, 4, 1, '2026-01-01');

SET IDENTITY_INSERT [Products] OFF;
GO

-- ORDERS + ORDER ITEMS + PAYMENTS
SET IDENTITY_INSERT [Orders] ON;

INSERT INTO [Orders] ([Id],[UserId],[Status],[TotalAmount],[CreatedDate]) VALUES
-- Customer A (UserId=3)
(1, 3, 'Shipped',   35000000, '2026-01-10'),   -- iPhone + Chuột x2
(2, 3, 'Confirmed', 28000000, '2026-02-15'),   -- MacBook
(3, 3, 'Shipped',   30000000, '2026-03-05'),   -- iPhone x1
(4, 3, 'Cancelled', 22000000, '2026-04-01'),   -- Samsung (cancelled)
-- Customer B (UserId=4)
(5, 4, 'Shipped',   41400000, '2026-01-20'),   -- Dell + Sony + Keyboard x2
(6, 4, 'Shipped',   22000000, '2026-02-28'),   -- Samsung
(7, 4, 'Shipped',   15000000, '2026-03-18'),   -- Xiaomi
(8, 4, 'Pending',   15000000, '2026-05-20');   -- Xiaomi (pending, dùng để test active-users)

SET IDENTITY_INSERT [Orders] OFF;
GO

SET IDENTITY_INSERT [OrderItems] ON;

INSERT INTO [OrderItems] ([Id],[OrderId],[ProductId],[Quantity],[UnitPrice],[LineTotal]) VALUES
-- Order 1: iPhone(1) + Chuột(2) = 30M + 5M = 35M
(1,  1, 4, 1, 30000000, 30000000),
(2,  1, 7, 2,  2500000,  5000000),
-- Order 2: MacBook(1) = 28M
(3,  2, 1, 1, 28000000, 28000000),
-- Order 3: iPhone(1) = 30M
(4,  3, 4, 1, 30000000, 30000000),
-- Order 4: Samsung(1) = 22M [Cancelled]
(5,  4, 5, 1, 22000000, 22000000),
-- Order 5: Dell(1) + Sony(1) + Keyboard(2) = 25M + 8M + 4.4M = 37.4M → 41.4M
(6,  5, 2, 1, 25000000, 25000000),
(7,  5, 9, 1,  8000000,  8000000),
(8,  5, 8, 2,  2200000,  4400000),
-- Order 6: Samsung(1) = 22M
(9,  6, 5, 1, 22000000, 22000000),
-- Order 7: Xiaomi(1) = 15M
(10, 7, 6, 1, 15000000, 15000000),
-- Order 8: Xiaomi(1) = 15M [Pending]
(11, 8, 6, 1, 15000000, 15000000);

SET IDENTITY_INSERT [OrderItems] OFF;
GO

-- Payments chỉ cho các Order đã Shipped
SET IDENTITY_INSERT [Payments] ON;

INSERT INTO [Payments] ([Id],[OrderId],[Amount],[Status],[Method],[CreatedDate]) VALUES
(1, 1, 35000000, 'Paid', 'BankTransfer', '2026-01-10'),
(2, 5, 41400000, 'Paid', 'CreditCard',   '2026-01-20'),
(3, 3, 30000000, 'Paid', 'BankTransfer', '2026-03-05'),
(4, 6, 22000000, 'Paid', 'Cash',         '2026-02-28'),
(5, 7, 15000000, 'Paid', 'BankTransfer', '2026-03-18');

SET IDENTITY_INSERT [Payments] OFF;
GO

USE [master]
GO