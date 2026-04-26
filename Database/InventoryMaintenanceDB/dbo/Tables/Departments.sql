CREATE TABLE [dbo].[Departments] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [TenantId] INT            NOT NULL,
    [Name]     NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Departments_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_Departments_TenantId]
    ON [dbo].[Departments] ([TenantId] ASC);
