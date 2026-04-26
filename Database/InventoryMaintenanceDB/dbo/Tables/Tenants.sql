CREATE TABLE [dbo].[Tenants] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC)
);
