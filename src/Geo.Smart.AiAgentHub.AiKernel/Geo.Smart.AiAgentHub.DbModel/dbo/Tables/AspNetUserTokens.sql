CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR (128) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [Name]          NVARCHAR (128) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserTokens', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入提供者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserTokens', @level2type = N'COLUMN', @level2name = N'LoginProvider';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserTokens', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUserTokens', @level2type = N'COLUMN', @level2name = N'Value';

