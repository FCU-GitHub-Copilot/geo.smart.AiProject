CREATE TABLE [dbo].[Elmah] (
    [Seq]            INT             IDENTITY (1, 1) NOT NULL,
    [LogTime]        DATETIME        CONSTRAINT [DF_Elmah_LogTime] DEFAULT (getdate()) NOT NULL,
    [Message]        NVARCHAR (MAX)  NOT NULL,
    [StackTrace]     NVARCHAR (MAX)  NULL,
    [InnerException] NVARCHAR (MAX)  NULL,
    [Application]    NVARCHAR (60)   NOT NULL,
    [Host]           NVARCHAR (50)   NOT NULL,
    [ConnectionApp]  NVARCHAR (1000) NULL,
    [ClassName]      NVARCHAR (500)  NULL,
    [MethodName]     NVARCHAR (200)  NULL,
    [Type]           NVARCHAR (100)  NULL,
    [Source]         NVARCHAR (500)  NOT NULL,
    [RequestPath]    NVARCHAR (2000) NOT NULL,
    [QueryString]    NVARCHAR (2000) NULL,
    [UserAgent]      NVARCHAR (1000) NULL,
    [UserId]         NVARCHAR (128)  NULL,
    CONSTRAINT [PK_Elmah] PRIMARY KEY CLUSTERED ([Seq] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'例外攔截記錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'例外攔截記錄流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Seq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'紀錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'LogTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Message';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'詳細資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'StackTrace';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'內部例外錯誤資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'InnerException';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'應用程式名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Application';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主機資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Host';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'資料庫連線應用系統', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'ConnectionApp';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'例外類別名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'ClassName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'例外方法名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'MethodName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'例外類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤來源', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'Source';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'請求位置', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'RequestPath';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'請求參數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'QueryString';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者代理字串', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'UserAgent';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者 ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Elmah', @level2type = N'COLUMN', @level2name = N'UserId';

