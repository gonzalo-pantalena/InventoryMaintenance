CREATE TABLE [dbo].[RefreshTokens] (
    [Id]            INT      IDENTITY (1, 1) NOT NULL,
    [StaffMemberId] INT      NOT NULL,
    [Token]         NVARCHAR (200) NOT NULL,
    [ExpiresAt]     DATETIME2 (7) NOT NULL,
    [CreatedAt]     DATETIME2 (7) NOT NULL,
    [RevokedAt]     DATETIME2 (7) NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefreshTokens_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [dbo].[StaffMembers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_StaffMemberId]
    ON [dbo].[RefreshTokens] ([StaffMemberId] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_Token]
    ON [dbo].[RefreshTokens] ([Token] ASC);
