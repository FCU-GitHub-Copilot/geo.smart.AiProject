CREATE TABLE [dbo].[ChatRoom] (
    [RoomId]       UNIQUEIDENTIFIER CONSTRAINT [DF_ChatRoom_RoomId] DEFAULT (newid()) NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [History]      TEXT             NOT NULL,
    [CreatedDate]  DATETIME         CONSTRAINT [DF_ChatRoom_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    NVARCHAR (128)   NOT NULL,
    [UpdatedDate]  DATETIME         CONSTRAINT [DF_ChatRoom_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]    NVARCHAR (128)   NOT NULL,
    [IsEnabled]    BIT              CONSTRAINT [DF_ChatRoom_IsEnabled] DEFAULT ((1)) NOT NULL,
    [LlmServiceId] NVARCHAR (100)   NOT NULL,
    [ToolSelected] NVARCHAR (MAX)   CONSTRAINT [DF_ChatRoom_ToolSelected] DEFAULT ('{}') NOT NULL,
    CONSTRAINT [PK_ChatRoom] PRIMARY KEY CLUSTERED ([RoomId] ASC)
);












GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聊天室主鍵', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'RoomId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聊天室名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'Name';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聊天紀錄，ChatHistory序列化', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'History';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次提問使用者選取的工具', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'ToolSelected';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次提問的 LLM 服務識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ChatRoom', @level2type = N'COLUMN', @level2name = N'LlmServiceId';

