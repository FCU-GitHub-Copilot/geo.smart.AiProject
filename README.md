# SMART AI Agent Hub 研發專案

## MCP Server

- 暫時先不支援 stdio 模式
## 資料庫設定

1. 新增使用者
```sql
USE [AiAgentHub]
GO

-- 建立開發 PG 帳號
CREATE USER [GEO\DEV_SMART_SC_PG]
GO
ALTER ROLE [db_owner] ADD MEMBER [GEO\DEV_SMART_SC_PG]
GO

-- CI 執行時使用的帳號
CREATE USER [GEO\ciworker]
GO
ALTER ROLE [db_owner] ADD MEMBER [GEO\ciworker]
GO

```

2. 建立 DEMO 測試站使用的帳號
```sql
USE [master]
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.server_principals WHERE name = 'AiAgentHubUser' )
BEGIN
    CREATE LOGIN AiAgentHubUser WITH PASSWORD = '*1qaz@WSX3edc*'
END

USE [AiAgentHub]
GO

CREATE USER AiAgentHubUser FOR LOGIN AiAgentHubUser
GO
ALTER ROLE [db_datareader] ADD MEMBER [AiAgentHubUser]
ALTER ROLE [db_datawriter] ADD MEMBER [AiAgentHubUser]
ALTER ROLE [db_executor] ADD MEMBER [AiAgentHubUser]
GO
```

3. 使用 SSMS 進行【匯出資料層應用程式】時，會因為 AD 帳號造成匯出失敗，
要先刪除使用者，匯出後再用步驟一加回來
```
DROP USER [GEO\DEV_SMART_SC_PG];
DROP USER [GEO\ciworker];
DROP LOGIN [GEO\DEV_SMART_SC_PG];
DROP LOGIN [GEO\ciworker];
```


## Kernel Memory

By default, the length of the embedding vector is 
1536 for text-embedding-3-small 
or 3072 for text-embedding-3-large

## Serilog [Syslog] 資料表

```sql
CREATE TABLE [dbo].[Syslog]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Message] NVARCHAR(MAX) NULL,
    [Level] NVARCHAR(128) NULL,
    [Timestamp] DATETIME2(7) NOT NULL,
    [Exception] NVARCHAR(MAX) NULL,
    [LogEvent] NVARCHAR(MAX) NULL,
    [User] NVARCHAR(200) NULL,
    [RequestPath] NVARCHAR(500) NULL,
    [RequestMethod] NVARCHAR(10) NULL,
    [StatusCode] INT NULL,
    [Application] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Syslog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 建議的索引
CREATE NONCLUSTERED INDEX [IX_Syslog_Timestamp] ON [dbo].[Syslog] ([Timestamp] DESC);
CREATE NONCLUSTERED INDEX [IX_Syslog_Level] ON [dbo].[Syslog] ([Level]) INCLUDE ([Timestamp], [Message]);
CREATE NONCLUSTERED INDEX [IX_Syslog_Application] ON [dbo].[Syslog] ([Application]) INCLUDE ([Timestamp], [Level]);
```
