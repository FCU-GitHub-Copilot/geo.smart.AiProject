CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [OrgId]                UNIQUEIDENTIFIER   NULL,
    [CreatedDate]          DATETIME           CONSTRAINT [DF_AspNetUsers_CreatedDate_1] DEFAULT (getdate()) NOT NULL,
    [IsEnabled]            BIT                CONSTRAINT [DF_AspNetUsers_IsEnabled_1] DEFAULT ((1)) NOT NULL,
    [FullName]             NVARCHAR (50)      NOT NULL,
    [RegisterVerifyCode]   VARCHAR (10)       NULL,
    [IsRegisterVerify]     BIT                CONSTRAINT [DF_AspNetUsers_IsRegisterVerify_1] DEFAULT ((0)) NOT NULL,
    [IsForgotPwd]          BIT                CONSTRAINT [DF_AspNetUsers_IsForgotPwd_1] DEFAULT ((0)) NOT NULL,
    [ForgotPwdVerifyCode]  VARCHAR (10)       NULL,
    [LastChangeQoo]        DATETIME           CONSTRAINT [DF_AspNetUsers_LastChangeQoo_1] DEFAULT (getdate()) NOT NULL,
    [MustChangeQoo]        BIT                CONSTRAINT [DF_AspNetUsers_MustChangeQoo_1] DEFAULT ((1)) NOT NULL,
    [Gender]               BIT                CONSTRAINT [DF_AspNetUsers_Gender_1] DEFAULT ((1)) NOT NULL,
    [Birthday]             DATE               NULL,
    [JobTitle]             NVARCHAR (50)      NULL,
    [Tel]                  NVARCHAR (100)     NULL,
    [LastLogin]            DATETIME           NULL,
    [LoginTimes]           INT                CONSTRAINT [DF_AspNetUsers_LoginTimes] DEFAULT ((0)) NOT NULL,
    [UseOtp]               BIT                CONSTRAINT [DF_AspNetUsers_UseOtp] DEFAULT ((0)) NOT NULL,
    [OtpSecret]            NVARCHAR (500)     NULL,
    [TelExt]               NVARCHAR (100)     NULL,
    [LoginType]            INT                CONSTRAINT [DF_AspNetUsers_LoginType] DEFAULT ((0)) NOT NULL,
    [IsDelete]             BIT                CONSTRAINT [DF_AspNetUsers_IsDelete] DEFAULT ((0)) NOT NULL,
    [DistinguishedName]    NVARCHAR (100)     COLLATE Chinese_Taiwan_Stroke_CI_AS NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_Organization] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId])
);








GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號，不可有中文', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'UserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號正規化', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'NormalizedUserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'email', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'Email';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Email 正規化', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'NormalizedEmail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'email 確認', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'EmailConfirmed';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'加密後的密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'PasswordHash';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'安全戳記', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'SecurityStamp';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Concurrency戳記', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'ConcurrencyStamp';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行動電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'PhoneNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行動電話確認', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'PhoneNumberConfirmed';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'雙因素認證啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'TwoFactorEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'鎖定結束日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LockoutEnd';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否鎖定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LockoutEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'失敗鎖定次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'AccessFailedCount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'組織機關ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'OrgId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'IsEnabled';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'中文姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'FullName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'APP註冊驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'RegisterVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'註冊驗證完成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'IsRegisterVerify';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否執行重設密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'IsForgotPwd';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'忘記密碼驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'ForgotPwdVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次修改密碼時間(Null:代表一進入就要強制更新，有時間大於3個月也是要強制更新)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LastChangeQoo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'必須先修改密碼才能進入系統', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'MustChangeQoo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'性別 1:男, 0:女', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'Gender';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'Birthday';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'職稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'JobTitle';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'Tel';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次登入時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LastLogin';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LoginTimes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'啟用 OTP 登入', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'UseOtp';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'OTP 驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'OtpSecret';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電話分機', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'TelExt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入方式:0=帳密登入、1=府內單登', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'LoginType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號是否刪除', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'IsDelete';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LDAP 專有名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'AspNetUsers', @level2type = N'COLUMN', @level2name = N'DistinguishedName';

