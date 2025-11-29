DECLARE @separator AS NVARCHAR(5)		= ','
DECLARE @IDs			 AS NVARCHAR(MAX) = '1,3,5,7'

SELECT C.* 
FROM [Contact] AS C 
	INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs
		ON C.ID = IDs.[value]


--DECLARE @separator AS NVARCHAR(5)		= ','
--DECLARE @IDs			 AS NVARCHAR(MAX) = '1,3,5,7'

--DELETE A FROM [Address] AS A
--INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs
--	ON A.ID = IDs.[value];


--DELETE C FROM [Contact] AS C WHERE C.ID > 6
--DELETE A FROM [Address] AS A WHERE A.ID > 7


--DROP TABLE dbo.[Address]
--DROP TABLE dbo.[State]
--DROP TABLE dbo.[Contact]


--TRUNCATE TABLE dbo.[Contact]
--TRUNCATE TABLE dbo.[Address]
--TRUNCATE TABLE dbo.[State]
