CREATE TABLE [dbo].[MaintenanceNotes] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [MaintenanceRecordId]  INT            NOT NULL,
    [Body]                 NVARCHAR (4000) NOT NULL,
    [CreatedAt]            DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_MaintenanceNotes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaintenanceNotes_MaintenanceRecords_MaintenanceRecordId] FOREIGN KEY ([MaintenanceRecordId]) REFERENCES [dbo].[MaintenanceRecords] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_MaintenanceNotes_MaintenanceRecordId]
    ON [dbo].[MaintenanceNotes] ([MaintenanceRecordId] ASC);
