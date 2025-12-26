CREATE TABLE [dbo].[Photo] (
    [PhotoId]     UNIQUEIDENTIFIER CONSTRAINT [DF_Photo_PhotoId] DEFAULT (newid()) NOT NULL,
    [FileName]    NVARCHAR (250)   NOT NULL,
    [Extension]   NVARCHAR (10)    NOT NULL,
    [Size]        BIGINT           NOT NULL,
    [ContentType] NVARCHAR (250)   NOT NULL,
    [Note]        NVARCHAR (250)   NULL,
    [Lat]         FLOAT (53)       NULL,
    [Lon]         FLOAT (53)       NULL,
    [PhotoSeq]    INT              IDENTITY (1, 1) NOT NULL,
    [CreatedBy]   NVARCHAR (128)   NOT NULL,
    [CreatedDate] DATETIME         CONSTRAINT [DF_Photo_CreatedDate_1] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]   NVARCHAR (128)   NOT NULL,
    [UpdatedDate] DATETIME         CONSTRAINT [DF_Photo_UpdatedDate_1] DEFAULT (getdate()) NOT NULL,
    [IsEnabled]   BIT              CONSTRAINT [DF_Photo_IsEnabled_1] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Photo] PRIMARY KEY NONCLUSTERED ([PhotoId] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_Photo_Seq]
    ON [dbo].[Photo]([PhotoId] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'照片唯一ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'PhotoId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'圖片名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'FileName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'副檔名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'Extension';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案大小', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'Size';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'ContentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'Note';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'Lat';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'經度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'Lon';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'PhotoSeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Photo', @level2type = N'COLUMN', @level2name = N'IsEnabled';

