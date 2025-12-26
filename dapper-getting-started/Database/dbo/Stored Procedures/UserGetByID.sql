-------------------------------------------------
CREATE PROCEDURE [dbo].[UserGetByID]
	 @IDs				AS VARCHAR(MAX) = NULL
	,@separator AS NVARCHAR(50) = NULL
AS
SELECT
	 U.[ID]
	,U.[FirstName]
	,U.[LastName]
	,U.[Email]
	,U.[Deleted]
FROM 
	[dbo].[User] AS U
	  INNER JOIN [dbo].[fn_CsvToInt](@IDs, @separator) AS IDs
			ON U.[ID] = IDs.[value];
-------------------------------------------------
