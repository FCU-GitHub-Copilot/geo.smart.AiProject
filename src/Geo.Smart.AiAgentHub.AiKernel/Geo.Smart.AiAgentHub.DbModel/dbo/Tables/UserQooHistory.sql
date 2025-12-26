CREATE TABLE [dbo].[UserQooHistory] (
    [HistorySeq]  INT            IDENTITY (1, 1) NOT NULL,
    [UserId]      NVARCHAR (100) NOT NULL,
    [LoginId]     NVARCHAR (100) NOT NULL,
    [CreatedDate] DATETIME       CONSTRAINT [DF_UserQooHistory_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [QooHash]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_UserQooHistory] PRIMARY KEY CLUSTERED ([HistorySeq] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者密碼變更歷程', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'歷程流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory', @level2type = N'COLUMN', @level2name = N'HistorySeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入者或操作者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory', @level2type = N'COLUMN', @level2name = N'LoginId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'加密後的密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserQooHistory', @level2type = N'COLUMN', @level2name = N'QooHash';

