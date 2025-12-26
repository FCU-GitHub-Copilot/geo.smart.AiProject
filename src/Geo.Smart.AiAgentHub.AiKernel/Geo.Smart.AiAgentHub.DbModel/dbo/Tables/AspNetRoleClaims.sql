CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoleClaims', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoleClaims', @level2type = N'COLUMN', @level2name = N'RoleId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'宣告類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoleClaims', @level2type = N'COLUMN', @level2name = N'ClaimType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'宣告值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetRoleClaims', @level2type = N'COLUMN', @level2name = N'ClaimValue';

