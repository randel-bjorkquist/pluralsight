-------------------------------------------------
CREATE PROCEDURE [dbo].[UserGetAll]
AS
BEGIN
	SELECT
		 U.[ID]
		,U.[FirstName]
		,U.[LastName]
		,U.[Email]
		,U.[Deleted]
	FROM
		[dbo].[User] AS U
END
-------------------------------------------------
