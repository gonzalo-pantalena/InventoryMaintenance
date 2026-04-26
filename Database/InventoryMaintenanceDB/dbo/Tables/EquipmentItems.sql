CREATE TABLE [dbo].[EquipmentItems] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [TenantId]                INT            NOT NULL,
    [DepartmentId]            INT            NOT NULL,
    [EquipmentTypeId]         INT            NOT NULL,
    [EquipmentApplicationId]  INT            NOT NULL,
    [Name]                    NVARCHAR (200) NOT NULL,
    [Brand]                   NVARCHAR (100) NOT NULL,
    [Model]                   NVARCHAR (100) NOT NULL,
    [Year]                    INT            NOT NULL,
    [SerialNumber]            NVARCHAR (100) NOT NULL,
    [EcriCode]                NVARCHAR (100) NOT NULL,
    [CommissionedOn]          DATE           NOT NULL,
    [DecommissionedOn]        DATE           NULL,
    [PublicId]                UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EquipmentItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentItems_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EquipmentItems_EquipmentApplications_EquipmentApplicationId] FOREIGN KEY ([EquipmentApplicationId]) REFERENCES [dbo].[EquipmentApplications] ([Id]),
    CONSTRAINT [FK_EquipmentItems_EquipmentTypes_EquipmentTypeId] FOREIGN KEY ([EquipmentTypeId]) REFERENCES [dbo].[EquipmentTypes] ([Id]),
    CONSTRAINT [FK_EquipmentItems_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_EquipmentItems_DepartmentId]
    ON [dbo].[EquipmentItems] ([DepartmentId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_EquipmentItems_EquipmentApplicationId]
    ON [dbo].[EquipmentItems] ([EquipmentApplicationId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_EquipmentItems_EquipmentTypeId]
    ON [dbo].[EquipmentItems] ([EquipmentTypeId] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentItems_PublicId]
    ON [dbo].[EquipmentItems] ([PublicId] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentItems_TenantId_SerialNumber]
    ON [dbo].[EquipmentItems] ([TenantId] ASC, [SerialNumber] ASC);
