-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressDelete]
	 @IDs				AS NVARCHAR(MAX)	= NULL
	,@separator AS NVARCHAR(50)		= NULL
AS
BEGIN
	DELETE A FROM [Address] AS A 
		INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs 
			ON A.ID = IDs.[value]
END;
-------------------------------------------------
