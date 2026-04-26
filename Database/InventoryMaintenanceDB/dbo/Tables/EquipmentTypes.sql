CREATE TABLE [dbo].[EquipmentTypes] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [TenantId] INT            NOT NULL,
    [Name]     NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_EquipmentTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentTypes_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id]) ON DELETE CASCADE
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentTypes_TenantId_Name]
    ON [dbo].[EquipmentTypes] ([TenantId] ASC, [Name] ASC);
