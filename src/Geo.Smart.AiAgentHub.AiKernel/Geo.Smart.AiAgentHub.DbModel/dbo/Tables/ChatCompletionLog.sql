CREATE TABLE [dbo].[ChatCompletionLog] (
    [LogSeq]          INT                IDENTITY (1, 1) NOT NULL,
    [LogId]           VARCHAR (50)       NULL,
    [Metadata]        TEXT               NULL,
    [CreatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_ChatCompletionLog_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [PromptToken]     BIGINT             NULL,
    [CompletionToken] BIGINT             NULL,
    [TotalToken]      BIGINT             NULL,
    CONSTRAINT [PK_ChatCompletionLog] PRIMARY KEY CLUSTERED ([LogSeq] ASC)
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'紀錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'記錄所有的 METADATA 內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'Metadata';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'AI 回應的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'LogId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聊天紀錄流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'LogSeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'總 Token 數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'TotalToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Prompt Token 數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'PromptToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Completion Token 數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatCompletionLog', @level2type = N'COLUMN', @level2name = N'CompletionToken';

