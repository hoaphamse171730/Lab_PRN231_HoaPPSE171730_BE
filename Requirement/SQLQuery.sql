-- 1) Drop & recreate the demo database
USE master;
GO

IF DB_ID(N'orchid_shop') IS NOT NULL
BEGIN
    ALTER DATABASE [orchid_shop] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [orchid_shop];
END
GO

CREATE DATABASE [orchid_shop]
    COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

USE [orchid_shop];
GO


-- 2) Clean up any stray tables (drop in FK order)
IF OBJECT_ID(N'dbo.order_details','U') IS NOT NULL DROP TABLE dbo.order_details;
IF OBJECT_ID(N'dbo.orders','U')        IS NOT NULL DROP TABLE dbo.orders;
IF OBJECT_ID(N'dbo.orchids','U')       IS NOT NULL DROP TABLE dbo.orchids;
IF OBJECT_ID(N'dbo.categories','U')    IS NOT NULL DROP TABLE dbo.categories;
IF OBJECT_ID(N'dbo.accounts','U')      IS NOT NULL DROP TABLE dbo.accounts;
IF OBJECT_ID(N'dbo.roles','U')         IS NOT NULL DROP TABLE dbo.roles;
GO


-- 3) Create tables

CREATE TABLE dbo.roles
(
    role_id   INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE dbo.accounts
(
    account_id   INT IDENTITY(1,1) PRIMARY KEY,
    account_name NVARCHAR(100) NOT NULL,
    email        NVARCHAR(100) NOT NULL UNIQUE,
    password     NVARCHAR(255) NOT NULL,
    role_id      INT NOT NULL
        CONSTRAINT FK_accounts_roles
        REFERENCES dbo.roles(role_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

CREATE TABLE dbo.categories
(
    category_id   INT IDENTITY(1,1) PRIMARY KEY,
    category_name NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE dbo.orchids
(
    orchid_id           INT IDENTITY(1,1) PRIMARY KEY,
    is_natural          BIT            NOT NULL 
        CONSTRAINT DF_orchids_is_natural DEFAULT(1),
    orchid_description  NVARCHAR(MAX)  NULL,
    orchid_name         NVARCHAR(100)  NOT NULL,
    orchid_url          NVARCHAR(255)  NULL,
    price               DECIMAL(10,2)  NOT NULL 
        CONSTRAINT DF_orchids_price DEFAULT(0.00),
    category_id         INT NULL
        CONSTRAINT FK_orchids_categories
        REFERENCES dbo.categories(category_id)
        ON DELETE SET NULL
        ON UPDATE NO ACTION
);
GO

CREATE TABLE dbo.orders
(
    id            INT IDENTITY(1,1) PRIMARY KEY,
    account_id    INT            NOT NULL
        CONSTRAINT FK_orders_accounts
        REFERENCES dbo.accounts(account_id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    order_date    DATETIME2      NOT NULL 
        CONSTRAINT DF_orders_order_date DEFAULT(SYSDATETIME()),
    order_status  VARCHAR(20)    NOT NULL 
        CONSTRAINT DF_orders_status DEFAULT('pending'),
    total_amount  DECIMAL(10,2)  NOT NULL 
        CONSTRAINT DF_orders_total_amount DEFAULT(0.00),
    CONSTRAINT CHK_orders_status 
        CHECK(order_status IN ('pending','processing','completed','cancelled'))
);
GO

CREATE TABLE dbo.order_details
(
    id        INT IDENTITY(1,1) PRIMARY KEY,
    order_id  INT            NOT NULL
        CONSTRAINT FK_order_details_orders
        REFERENCES dbo.orders(id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    orchid_id INT            NOT NULL
        CONSTRAINT FK_order_details_orchids
        REFERENCES dbo.orchids(orchid_id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    price     DECIMAL(10,2)  NOT NULL,
    quantity  INT            NOT NULL 
        CONSTRAINT DF_order_details_quantity DEFAULT(1)
);
GO


-- 4) Insert sample data

INSERT INTO dbo.roles(role_name) VALUES
    ('Admin'),
    ('Customer');
GO

INSERT INTO dbo.accounts(account_name, email, password, role_id) VALUES
    ('Alice Nguyen', 'alice@example.com', 'alicepass', 1),
    ('Bob Tran',     'bob@example.com',   'bobpass',   2),
    ('Carol Le',     'carol@example.com', 'carolpass', 2);
GO

INSERT INTO dbo.categories(category_name) VALUES
    ('Cattleya'),
    ('Phalaenopsis'),
    ('Dendrobium');
GO

INSERT INTO dbo.orchids(is_natural, orchid_description, orchid_name, orchid_url, price, category_id) VALUES
    (1, 'Large lavender Cattleya with ruffled petals', 'Lavender Cattleya', 'https://example.com/img/cattleya1.jpg', 25.00, 1),
    (0, 'Pink Phalaenopsis hybrid, perfect for beginners', 'Phala Pink',          'https://example.com/img/phala1.jpg',   30.00, 2),
    (1, 'Slim-stemmed white Dendrobium, very fragrant','White Dendro',     'https://example.com/img/den1.jpg',     20.00, 3);
GO

INSERT INTO dbo.orders(account_id, order_date, order_status, total_amount) VALUES
    (2, '2025-07-04T10:15:00', 'completed', 55.00),
    (3, '2025-07-05T14:30:00', 'processing', 20.00);
GO

INSERT INTO dbo.order_details(order_id, orchid_id, price, quantity) VALUES
    (1, 1, 25.00, 1),
    (1, 2, 30.00, 1),
    (2, 3, 20.00, 1);
GO
