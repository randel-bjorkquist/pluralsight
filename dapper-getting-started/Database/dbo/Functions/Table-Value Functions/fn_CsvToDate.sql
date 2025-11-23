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
