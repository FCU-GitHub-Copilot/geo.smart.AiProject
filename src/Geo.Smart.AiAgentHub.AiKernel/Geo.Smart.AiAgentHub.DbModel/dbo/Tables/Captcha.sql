CREATE TABLE [dbo].[Captcha] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Captcha_Id] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (6)      NOT NULL,
    [CreatedBy]   NVARCHAR (128)   NOT NULL,
    [CreatedDate] DATETIME         CONSTRAINT [DF_Captcha_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]   NVARCHAR (128)   NOT NULL,
    [UpdatedDate] DATETIME         CONSTRAINT [DF_Captcha_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [IsEnabled]   BIT              CONSTRAINT [DF_Captcha_IsEnabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Captcha] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Captcha', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Captcha ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Captcha Code', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'Code';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'CreatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'UpdatedBy';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'UpdatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Captcha', @level2type = N'COLUMN', @level2name = N'IsEnabled';

