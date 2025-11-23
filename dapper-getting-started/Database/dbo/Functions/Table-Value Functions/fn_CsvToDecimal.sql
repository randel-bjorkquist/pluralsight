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
