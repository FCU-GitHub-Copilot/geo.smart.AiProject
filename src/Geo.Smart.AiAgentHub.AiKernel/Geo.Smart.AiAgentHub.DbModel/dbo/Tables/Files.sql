CREATE TABLE [dbo].[Files] (
    [FileId]      UNIQUEIDENTIFIER CONSTRAINT [DF_Files_FileId] DEFAULT (newid()) NOT NULL,
    [FileName]    NVARCHAR (250)   NOT NULL,
    [Extension]   NVARCHAR (10)    NOT NULL,
    [Size]        BIGINT           NOT NULL,
    [ContentType] NVARCHAR (250)   NOT NULL,
    [Note]        NVARCHAR (250)   NULL,
    [FileSeq]     INT              IDENTITY (1, 1) NOT NULL,
    [CreatedDate] DATETIME         CONSTRAINT [DF_Files_CreatedDate_1] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]   NVARCHAR (128)   NOT NULL,
    [UpdatedDate] DATETIME         CONSTRAINT [DF_Files_UpdatedDate_1] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]   NVARCHAR (128)   NOT NULL,
    [IsEnabled]   BIT              CONSTRAINT [DF_Files_IsEnabled_1] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY NONCLUSTERED ([FileId] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_Files_Seq]
    ON [dbo].[Files]([FileId] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案唯一Id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'FileId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'FileName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'副檔名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'Extension';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案大小', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'Size';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'ContentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'Note';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'FileSeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Files', @level2type = N'COLUMN', @level2name = N'IsEnabled';

