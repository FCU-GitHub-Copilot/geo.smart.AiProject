CREATE TABLE [dbo].[UserToken] (
    [TokenId]      UNIQUEIDENTIFIER NOT NULL,
    [UserId]       NVARCHAR (100)   NOT NULL,
    [UserName]     NVARCHAR (256)   NOT NULL,
    [AccessToken]  VARCHAR (1000)   NOT NULL,
    [ReFreshToken] VARCHAR (32)     NOT NULL,
    [ExpiredDate]  DATETIME         NOT NULL,
    [CreatedDate]  DATETIME         CONSTRAINT [DF_UserToken_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [IsEnabled]    BIT              CONSTRAINT [DF_UserToken_IsEnabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_UserToken] PRIMARY KEY CLUSTERED ([TokenId] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者 Token 紀錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者 Token 紀錄 ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'TokenId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'UserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Access Token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'AccessToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Refresh Token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'ReFreshToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Refresh Token 到期日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'ExpiredDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'每次 Refresh 後需要撤銷', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserToken', @level2type = N'COLUMN', @level2name = N'IsEnabled';

