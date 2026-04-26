CREATE TABLE [dbo].[MaintenanceRecords] (
    [Id]                    INT      IDENTITY (1, 1) NOT NULL,
    [TenantId]              INT      NOT NULL,
    [EquipmentId]           INT      NOT NULL,
    [MaintenanceTypeId]     INT      NOT NULL,
    [RequestedAt]           DATETIME2 (7) NOT NULL,
    [CompletedAt]           DATETIME2 (7) NULL,
    [RequestedByStaffId]    INT      NOT NULL,
    CONSTRAINT [PK_MaintenanceRecords] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaintenanceRecords_EquipmentItems_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [dbo].[EquipmentItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MaintenanceRecords_MaintenanceTypes_MaintenanceTypeId] FOREIGN KEY ([MaintenanceTypeId]) REFERENCES [dbo].[MaintenanceTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MaintenanceRecords_StaffMembers_RequestedByStaffId] FOREIGN KEY ([RequestedByStaffId]) REFERENCES [dbo].[StaffMembers] ([Id]),
    CONSTRAINT [FK_MaintenanceRecords_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_MaintenanceRecords_EquipmentId]
    ON [dbo].[MaintenanceRecords] ([EquipmentId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_MaintenanceRecords_MaintenanceTypeId]
    ON [dbo].[MaintenanceRecords] ([MaintenanceTypeId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_MaintenanceRecords_RequestedByStaffId]
    ON [dbo].[MaintenanceRecords] ([RequestedByStaffId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_MaintenanceRecords_TenantId]
    ON [dbo].[MaintenanceRecords] ([TenantId] ASC);
