CREATE TABLE [dbo].[EquipmentApplications] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [TenantId]           INT            NOT NULL,
    [EquipmentTypeId]    INT            NOT NULL,
    [Name]               NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_EquipmentApplications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentApplications_EquipmentTypes_EquipmentTypeId] FOREIGN KEY ([EquipmentTypeId]) REFERENCES [dbo].[EquipmentTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EquipmentApplications_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_EquipmentApplications_EquipmentTypeId]
    ON [dbo].[EquipmentApplications] ([EquipmentTypeId] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentApplications_TenantId_EquipmentTypeId_Name]
    ON [dbo].[EquipmentApplications] ([TenantId] ASC, [EquipmentTypeId] ASC, [Name] ASC);
