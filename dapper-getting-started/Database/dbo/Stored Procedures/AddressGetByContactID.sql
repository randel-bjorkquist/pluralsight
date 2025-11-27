-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressGetByContactID]
	 @IDs				AS VARCHAR(MAX) = NULL
	,@separator AS NVARCHAR(50) = NULL
AS
SELECT
	 A.[ID]
	,A.[ContactID]
	,A.[AddressType]
	,A.[StreetAddress]
	,A.[City]
	,A.[StateID]
	,A.[PostalCode]
FROM 
	[dbo].[Address] AS A
	  INNER JOIN [dbo].[fn_CsvToInt](@IDs, @separator) AS IDs
			ON A.[ContactID] = IDs.[value];
-------------------------------------------------
