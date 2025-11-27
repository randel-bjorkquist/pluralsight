-------------------------------------------------
CREATE PROCEDURE [dbo].[ContactGetAll]
AS
BEGIN
	SELECT
		 C.[ID]
		,C.[FirstName]
		,C.[LastName]
		,C.[Company]
		,C.[Title]
		,C.[Email]
	FROM
		[dbo].[Contact] AS C
END
-------------------------------------------------
