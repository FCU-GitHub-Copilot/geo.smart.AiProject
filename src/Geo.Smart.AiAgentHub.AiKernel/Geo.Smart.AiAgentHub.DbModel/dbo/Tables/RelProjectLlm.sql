CREATE TABLE [dbo].[RelProjectLlm] (
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [LlmId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RelProjectLlm] PRIMARY KEY CLUSTERED ([ProjectId] ASC, [LlmId] ASC),
    CONSTRAINT [FK_RelProjectLlm_AiProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[AiProject] ([ProjectId]),
    CONSTRAINT [FK_RelProjectLlm_LlmInfo] FOREIGN KEY ([LlmId]) REFERENCES [dbo].[LlmInfo] ([LlmId])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RelProjectLlm', @level2type = N'COLUMN', @level2name = N'LlmId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案 ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RelProjectLlm', @level2type = N'COLUMN', @level2name = N'ProjectId';

