CREATE TABLE [dbo].[ChatMessage] (
    [MessageId]    UNIQUEIDENTIFIER CONSTRAINT [DF_ChatMessage_MessageId] DEFAULT (newid()) NOT NULL,
    [RoomId]       UNIQUEIDENTIFIER NOT NULL,
    [Role]         NVARCHAR (20)    NOT NULL,
    [Content]      NVARCHAR (MAX)   NOT NULL,
    [SentAt]       DATETIME         CONSTRAINT [DF__ChatMessa__SentA__08D548FA] DEFAULT (getdate()) NOT NULL,
    [LlmServiceId] NVARCHAR (100)   NOT NULL,
    [LogId]        VARCHAR (50)     NULL,
    [Tokens]       BIGINT           NULL,
    [CreatedDate]  DATETIME         CONSTRAINT [DF_ChatMessage_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    NVARCHAR (128)   NOT NULL,
    [UpdatedDate]  DATETIME         CONSTRAINT [DF_ChatMessage_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]    NVARCHAR (128)   NOT NULL,
    [IsEnabled]    BIT              CONSTRAINT [DF_ChatMessage_IsEnabled] DEFAULT ((1)) NOT NULL,
    [ToolSelected] NVARCHAR (MAX)   CONSTRAINT [DF_ChatMessage_ToolSelected] DEFAULT ('{}') NULL,
    CONSTRAINT [PK_ChatMessage] PRIMARY KEY CLUSTERED ([MessageId] ASC),
    CONSTRAINT [FK_ChatMessage_ChatRoom] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[ChatRoom] ([RoomId])
);














GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息主鍵', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'MessageId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聊天室主鍵', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'RoomId';


GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息發送時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'SentAt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LLM 服務識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'LlmServiceId';


GO



GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發送者角色（user/system/assistant）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'Role';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'Content';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'總 Token 數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'Tokens';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'AI 回應的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'LogId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提問使用時者選取的工具', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatMessage', @level2type = N'COLUMN', @level2name = N'ToolSelected';

