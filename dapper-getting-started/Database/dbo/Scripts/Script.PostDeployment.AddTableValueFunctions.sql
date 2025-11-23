/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
CREATE FUNCTION [dbo].[fn_CsvToBigInt]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS BIGINT) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> ''
           AND TRY_CAST(TRIM(SS.[value]) AS BIGINT) IS NOT NULL );
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToDate]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS DATE) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> '' 
           AND TRY_CAST(TRIM(SS.[value]) AS DATE) IS NOT NULL);
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToDateTime]
(
    @csv_string VARCHAR(MAX),
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS DATETIME) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> ''
           AND TRY_CAST(TRIM(SS.[value]) AS DATETIME) IS NOT NULL );
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToDecimal]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS DECIMAL(18, 6)) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> ''
           AND TRY_CAST(TRIM(SS.[value]) AS DECIMAL(18, 6)) IS NOT NULL );
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToGuid]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS UNIQUEIDENTIFIER) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> '' 
           AND TRY_CAST(TRIM(SS.[value]) AS UNIQUEIDENTIFIER) IS NOT NULL);
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToInt]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS INT) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> ''
           AND TRY_CAST(TRIM(SS.[value]) AS INT) IS NOT NULL );
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToNumeric]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRY_CAST(TRIM(SS.[value]) AS DECIMAL(38, 18)) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> ''
           AND TRY_CAST(TRIM(SS.[value]) AS DECIMAL(38, 18)) IS NOT NULL );
GO

--------------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[fn_CsvToVarchar]
(
    @csv_string VARCHAR(MAX), 
    @delimiter  NVARCHAR(1) = ','
)
  RETURNS TABLE
AS
RETURN( SELECT TRIM(SS.[value]) AS [value]
          FROM STRING_SPLIT(@csv_string, @delimiter) AS SS
         WHERE TRIM(SS.value) <> '' );
GO

--------------------------------------------------------------------------------------
