CREATE TABLE [dbo].[Organization] (
    [OrgId]       UNIQUEIDENTIFIER CONSTRAINT [DF_Organization_OrgId] DEFAULT (newid()) NOT NULL,
    [Name]        NVARCHAR (100)   NOT NULL,
    [NameShort]   NVARCHAR (50)    NULL,
    [Upper]       UNIQUEIDENTIFIER NULL,
    [CreatedBy]   NVARCHAR (128)   NOT NULL,
    [CreatedDate] DATETIME         CONSTRAINT [DF__Organizat__Creat__0AF29B96] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]   NVARCHAR (128)   NOT NULL,
    [UpdatedDate] DATETIME         CONSTRAINT [DF__Organizat__Updat__0BE6BFCF] DEFAULT (getdate()) NOT NULL,
    [IsEnabled]   BIT              CONSTRAINT [DF__Organizat__IsEna__09FE775D] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ORGANIZATION] PRIMARY KEY CLUSTERED ([OrgId] ASC),
    CONSTRAINT [FK_Organization_Organization] FOREIGN KEY ([Upper]) REFERENCES [dbo].[Organization] ([OrgId])
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'組織單位編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'OrgId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'組織單位名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'組織簡稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'NameShort';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上層組織', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'Upper';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Organization', @level2type = N'COLUMN', @level2name = N'IsEnabled';

