-- ============================================================
--  Employee Management System — SQL Server Schema
--  Use this if you want to switch from SQLite to SQL Server
-- ============================================================

-- 1. Create database (run once as sa / sysadmin)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EMS_DB')
    CREATE DATABASE EMS_DB;
GO

USE EMS_DB;
GO

-- 2. Users table
IF OBJECT_ID('Users', 'U') IS NULL
CREATE TABLE Users (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50)  NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role     NVARCHAR(20)  NOT NULL DEFAULT 'admin'
);
GO

-- 3. Employees table
IF OBJECT_ID('Employees', 'U') IS NULL
CREATE TABLE Employees (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(100),
    Phone       NVARCHAR(20),
    Department  NVARCHAR(60),
    Designation NVARCHAR(80),
    Salary      DECIMAL(18,2) DEFAULT 0,
    Gender      NVARCHAR(10),
    JoiningDate DATE,
    Status      NVARCHAR(20)  DEFAULT 'Active',
    Address     NVARCHAR(250),
    Notes       NVARCHAR(500)
);
GO

-- 4. Default admin seed
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
    INSERT INTO Users (Username, Password, Role) VALUES ('admin', 'admin@123', 'admin');
GO

-- ============================================================
--  To switch the app to SQL Server:
--  1. Run this script against your SQL Server instance.
--  2. In Db.cs, replace Microsoft.Data.Sqlite with
--     Microsoft.Data.SqlClient (NuGet).
--  3. Replace SqliteConnection / SqliteCommand with
--     SqlConnection / SqlCommand.
--  4. Update ConnStr to:
--     "Server=(localdb)\\MSSQLLocalDB;Database=EMS_DB;Integrated Security=true"
--     or your own connection string.
-- ============================================================
