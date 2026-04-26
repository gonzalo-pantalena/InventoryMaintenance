CREATE TABLE [dbo].[StaffMembers] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [TenantId]      INT            NOT NULL,
    [DepartmentId]  INT            NULL,
    [StaffRoleId]   INT            NOT NULL,
    [UserName]      NVARCHAR (100) NOT NULL,
    [Email]         NVARCHAR (256) NOT NULL,
    [FirstName]     NVARCHAR (100) NOT NULL,
    [LastName]      NVARCHAR (100) NOT NULL,
    [PasswordHash]  NVARCHAR (500) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StaffMembers_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]),
    CONSTRAINT [FK_StaffMembers_StaffRoles_StaffRoleId] FOREIGN KEY ([StaffRoleId]) REFERENCES [dbo].[StaffRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StaffMembers_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_StaffMembers_DepartmentId]
    ON [dbo].[StaffMembers] ([DepartmentId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_StaffMembers_StaffRoleId]
    ON [dbo].[StaffMembers] ([StaffRoleId] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_StaffMembers_TenantId_UserName]
    ON [dbo].[StaffMembers] ([TenantId] ASC, [UserName] ASC);
