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
