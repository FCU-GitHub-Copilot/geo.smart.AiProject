CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR (128) NOT NULL,
    [ProviderKey]         NVARCHAR (128) NOT NULL,
    [UserId]              NVARCHAR (128) NOT NULL,
    [ProviderDisplayName] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者登入紀錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserLogins';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'第三方登入提供者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserLogins', @level2type = N'COLUMN', @level2name = N'LoginProvider';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提供者KEY', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserLogins', @level2type = N'COLUMN', @level2name = N'ProviderKey';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserLogins', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提供者名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserLogins', @level2type = N'COLUMN', @level2name = N'ProviderDisplayName';

