CREATE TABLE [dbo].[RelProjectMcp] (
    [ProjectId]   UNIQUEIDENTIFIER NOT NULL,
    [McpServerId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RelProjectMcp] PRIMARY KEY CLUSTERED ([ProjectId] ASC, [McpServerId] ASC),
    CONSTRAINT [FK_RelProjectMcp_AiProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[AiProject] ([ProjectId]),
    CONSTRAINT [FK_RelProjectMcp_McpServer] FOREIGN KEY ([McpServerId]) REFERENCES [dbo].[McpServer] ([McpServerId])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MCP Server ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RelProjectMcp', @level2type = N'COLUMN', @level2name = N'McpServerId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案 ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RelProjectMcp', @level2type = N'COLUMN', @level2name = N'ProjectId';

