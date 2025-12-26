-- =============================================
-- Author:		Joe
-- Create date: 2020-05-27
-- Description:	利用 UserId 取得使用者名稱，常用於顯示 CreatedBy、UpdatedBy
-- =============================================
CREATE FUNCTION [dbo].[FnUserFullName]
(
	@key NVARCHAR(128)
)

RETURNS NVARCHAR(200)
AS
BEGIN
	DECLARE @UserFullName NVARCHAR(200) = '';

	SELECT @UserFullName = FullName
	FROM [dbo].[AspNetUsers]
	WHERE [Id] = @key;
	RETURN @UserFullName

END