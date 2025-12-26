CREATE TABLE [dbo].[UserHistory] (
    [HistorySeq]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]          NVARCHAR (100) NOT NULL,
    [LoginId]         NVARCHAR (100) NULL,
    [UserHistoryType] INT            CONSTRAINT [DF_UserHistory_UserHistoryType] DEFAULT ((0)) NOT NULL,
    [HistoryTypeName] NVARCHAR (10)  NOT NULL,
    [RequestTime]     DATETIME       CONSTRAINT [DF_UserHistory_RequestTime] DEFAULT (getdate()) NOT NULL,
    [RequestResult]   BIT            CONSTRAINT [DF_UserHistory_RequestResult] DEFAULT ((0)) NOT NULL,
    [Message]         NVARCHAR (MAX) NULL,
    [Ip]              NVARCHAR (50)  CONSTRAINT [DF_UserHistory_Ip] DEFAULT (N'-') NOT NULL,
    CONSTRAINT [PK_UserHistory] PRIMARY KEY CLUSTERED ([HistorySeq] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者帳號修改歷程', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'歷程流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'HistorySeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入者或操作者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'LoginId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者資料異動類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'UserHistoryType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'資料異動類型名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'HistoryTypeName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'RequestTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作執行結果是否成功', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'RequestResult';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作結果額外資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'Message';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory', @level2type = N'COLUMN', @level2name = N'Ip';

