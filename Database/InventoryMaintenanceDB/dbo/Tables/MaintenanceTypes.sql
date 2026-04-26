CREATE TABLE [dbo].[MaintenanceTypes] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_MaintenanceTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);
