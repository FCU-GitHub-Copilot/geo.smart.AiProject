CREATE TABLE [dbo].[McpServer] (
    [McpServerId]   UNIQUEIDENTIFIER CONSTRAINT [DF_McpServer_McpServerId] DEFAULT (newid()) NOT NULL,
    [UserId]        NVARCHAR (128)   NOT NULL,
    [Name]          NVARCHAR (100)   NOT NULL,
    [McpServerType] INT              CONSTRAINT [DF_McpServer_McpServerType] DEFAULT ((0)) NOT NULL,
    [SseUrl]        NVARCHAR (200)   NULL,
    [StdioCommand]  NVARCHAR (100)   NULL,
    [StdioArgs]     NVARCHAR (MAX)   NULL,
    [StdioEnv]      NVARCHAR (MAX)   NULL,
    [Tools]         NVARCHAR (MAX)   NOT NULL,
    [CreatedDate]   DATETIME         CONSTRAINT [DF_McpServer_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (128)   NOT NULL,
    [UpdatedDate]   DATETIME         CONSTRAINT [DF_McpServer_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]     NVARCHAR (128)   NOT NULL,
    [IsEnabled]     BIT              CONSTRAINT [DF_McpServer_IsEnabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_McpServer] PRIMARY KEY CLUSTERED ([McpServerId] ASC),
    CONSTRAINT [FK_McpServer_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);










GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MCP Server ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'McpServerId';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MCP 服務名稱，只能是英數字以及底線', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'Name';






GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MCP 伺服器通訊型態，0:Stdio,1:Sse', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'McpServerType';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'擁有者 UserId', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'stdio 環境變數，存 JSON object', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'StdioEnv';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'stdio 的指令', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'StdioCommand';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'stdio 指令參數，存 JSON array', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'StdioArgs';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'伺服器的 SSE 端點 URL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'SseUrl';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工具清單，存 JSON object，Name、Description', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'McpServer', @level2type = N'COLUMN', @level2name = N'Tools';

