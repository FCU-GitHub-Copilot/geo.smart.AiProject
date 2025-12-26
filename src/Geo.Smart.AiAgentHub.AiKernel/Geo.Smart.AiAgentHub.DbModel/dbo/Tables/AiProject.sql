CREATE TABLE [dbo].[AiProject] (
    [ProjectId]    UNIQUEIDENTIFIER CONSTRAINT [DF_AiProject_ProjectId] DEFAULT (newid()) NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [Description]  NVARCHAR (500)   NULL,
    [UserId]       NVARCHAR (128)   NOT NULL,
    [SystemPrompt] NVARCHAR (MAX)   NOT NULL,
    [CreatedDate]  DATETIME         CONSTRAINT [DF_AiProject_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    NVARCHAR (128)   NOT NULL,
    [UpdatedDate]  DATETIME         CONSTRAINT [DF_AiProject_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]    NVARCHAR (128)   NOT NULL,
    [IsEnabled]    BIT              CONSTRAINT [DF_AiProject_IsEnabled] DEFAULT ((1)) NOT NULL,
    [Temperature]  FLOAT (53)       NULL,
    [TopP]         FLOAT (53)       NULL,
    [TopK]         INT              NULL,
    [MaxTokens]    INT              NULL,
    CONSTRAINT [PK_AiProject] PRIMARY KEY CLUSTERED ([ProjectId] ASC),
    CONSTRAINT [FK_AiProject_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案擁有者 UserId', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'UserId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'Description';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案 ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'ProjectId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'系統提示詞', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'SystemPrompt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'控制 LLM 文本生成的機率篩選器，範圍 0.1 到 2 之間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'TopP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM 只會從機率最高的 k 個 Tokens 中進行選擇', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'TopK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'溫度，控制 LLM 的創造力，範圍 0 到 2 之間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'Temperature';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最大的 token 數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AiProject', @level2type = N'COLUMN', @level2name = N'MaxTokens';

