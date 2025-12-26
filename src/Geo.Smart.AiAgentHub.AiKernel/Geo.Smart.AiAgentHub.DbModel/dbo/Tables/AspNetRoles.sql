CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR (128) NOT NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoles';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoles', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoles', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名稱正規化', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoles', @level2type = N'COLUMN', @level2name = N'NormalizedName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Concurrency戳記', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoles', @level2type = N'COLUMN', @level2name = N'ConcurrencyStamp';

