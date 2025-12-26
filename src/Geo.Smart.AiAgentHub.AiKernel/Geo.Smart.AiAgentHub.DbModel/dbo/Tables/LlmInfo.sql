CREATE TABLE [dbo].[LlmInfo] (
    [LlmId]          UNIQUEIDENTIFIER CONSTRAINT [DF_LlmInfo_LlmId] DEFAULT (newid()) NOT NULL,
    [UserId]         NVARCHAR (128)   NOT NULL,
    [ServiceId]      NVARCHAR (100)   NOT NULL,
    [ModelId]        NVARCHAR (100)   NOT NULL,
    [LlmSourceType]  INT              CONSTRAINT [DF_LlmInfo_LlmSourceType] DEFAULT ((0)) NOT NULL,
    [ApiKey]         NVARCHAR (MAX)   NULL,
    [Endpoint]       NVARCHAR (500)   NULL,
    [DeploymentName] NVARCHAR (100)   NULL,
    [Description]    NVARCHAR (500)   NULL,
    [CreatedDate]    DATETIME         CONSTRAINT [DF_LlmInfo_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]      NVARCHAR (128)   NOT NULL,
    [UpdatedDate]    DATETIME         CONSTRAINT [DF_LlmInfo_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]      NVARCHAR (128)   NOT NULL,
    [IsEnabled]      BIT              CONSTRAINT [DF_LlmInfo_IsEnabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_LlmInfo] PRIMARY KEY CLUSTERED ([LlmId] ASC),
    CONSTRAINT [FK_LlmInfo_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);












GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'LlmId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模型管理名稱、服務識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'ServiceId';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM 模型名稱，gpt-4o', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'ModelId';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM 來源類型，0:OpenAi,1:AzureOpenAi,2:Ollama,3:Gemini,4:Afs', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'LlmSourceType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'API 金鑰', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'ApiKey';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'端點網址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'Endpoint';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部署名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'DeploymentName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'Description';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'擁有者 UserId', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LlmInfo', @level2type = N'COLUMN', @level2name = N'UserId';


GO





GO





GO





GO
