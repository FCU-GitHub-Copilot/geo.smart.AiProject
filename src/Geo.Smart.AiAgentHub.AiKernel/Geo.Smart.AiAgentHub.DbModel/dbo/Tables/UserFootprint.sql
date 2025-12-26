CREATE TABLE [dbo].[UserFootprint] (
    [Seq]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]      NVARCHAR (128) NOT NULL,
    [Auth]        NVARCHAR (100) NOT NULL,
    [LogType]     NVARCHAR (10)  CONSTRAINT [DF_UserFootprint_LogType] DEFAULT (N'API') NOT NULL,
    [Url]         NVARCHAR (500) NOT NULL,
    [HttpVerb]    NVARCHAR (10)  CONSTRAINT [DF_UserFootprint_HttpVerb] DEFAULT (N'GET') NOT NULL,
    [QueryString] NVARCHAR (MAX) NOT NULL,
    [PostBody]    NVARCHAR (MAX) NOT NULL,
    [UserAgent]   NVARCHAR (MAX) NOT NULL,
    [Ip]          NVARCHAR (50)  NOT NULL,
    [RequestTime] DATETIME       CONSTRAINT [DF_UserFootprint_RequestTime] DEFAULT (getdate()) NOT NULL,
    [Controller]  NVARCHAR (50)  NULL,
    [Action]      NVARCHAR (50)  NULL,
    [PageName]    NVARCHAR (100) NULL,
    [Browser]     NVARCHAR (200) NULL,
    [Os]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_UserFootprint] PRIMARY KEY CLUSTERED ([Seq] ASC),
    CONSTRAINT [FK_UserFootprint_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者足跡，自動記錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者足跡 SEQ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Seq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Auth';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作紀錄類型：API、MVC、SQL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'LogType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作路徑，可與 swagger json 配對', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Url';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'HTTP動詞', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'HttpVerb';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'GET 查詢參數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'QueryString';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'POST 資料內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'PostBody';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者代理資訊：作業系統、瀏覽器', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'UserAgent';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Ip';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'RequestTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Controller 名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Controller';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Action 名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Action';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'頁面名稱 / 操作項目', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'PageName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'瀏覽器類型、版本', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Browser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'作業平台', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserFootprint', @level2type = N'COLUMN', @level2name = N'Os';

