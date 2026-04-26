-- Creates the application database. Schema comes from EF migrations (dotnet ef update) and/or the SSDT project.
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'InventoryMaintenance')
    CREATE DATABASE [InventoryMaintenance] COLLATE SQL_Latin1_General_CP1_CI_AS;
GO
