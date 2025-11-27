-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressGetAll]
AS
BEGIN
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
END
-------------------------------------------------
