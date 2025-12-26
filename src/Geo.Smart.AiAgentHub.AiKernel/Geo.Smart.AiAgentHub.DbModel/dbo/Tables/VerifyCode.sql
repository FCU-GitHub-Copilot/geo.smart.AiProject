CREATE TABLE [dbo].[VerifyCode] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_EnforceChangeQoo_Id_1] DEFAULT (newid()) NOT NULL,
    [UserId]     NVARCHAR (128)   NOT NULL,
    [StartTime]  DATETIME         NOT NULL,
    [EndTime]    DATETIME         NOT NULL,
    [VerifyType] INT              NOT NULL,
    CONSTRAINT [PK_EnforceChangeQoo] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'強制變更密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'強制變更密碼id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者Id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效起日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode', @level2type = N'COLUMN', @level2name = N'StartTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效迄日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode', @level2type = N'COLUMN', @level2name = N'EndTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'驗證類型(1:信箱註冊、2:第一次登入、3:三個月強制變更密碼、4:忘記密碼)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VerifyCode', @level2type = N'COLUMN', @level2name = N'VerifyType';

