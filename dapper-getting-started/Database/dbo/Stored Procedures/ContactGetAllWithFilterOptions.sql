--------------------------------------------------------------
--DROP PROCEDURE IF EXISTS [dbo].[ContactGetAllWithFilterOptions];

--------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[ContactGetAllWithFilterOptions]
   @Filters         FilterCriteriaType  READONLY
  ,@AllowedColumns  AllowedColumnType   READONLY
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @Where NVARCHAR(MAX) = '';

  -- Whitelist allowed columns for security
--  DECLARE @AllowedColumns TABLE (ColumnName NVARCHAR(100) PRIMARY KEY);
--  INSERT INTO @AllowedColumns (ColumnName)
--  VALUES ('ID'), ('FirstName'), ('LastName'), ('Email'), ('Deleted');
  IF EXISTS (SELECT 1 FROM @Filters AS F
             WHERE F.ColumnName NOT IN (SELECT ColumnName FROM @AllowedColumns))
    BEGIN
      RAISERROR('Invlide Column Name in Filter',16, 1)
      RETURN;
    END

  -- Build the WHERE clause dynamically from the TVP
  SELECT @Where = @Where +
    CASE WHEN ROW_NUMBER() OVER (ORDER BY ParameterIndex) > 1 
         THEN ' ' + LogicalOperator + ' '
         ELSE ''
    END +
    CASE  WHEN ColumnName NOT IN (SELECT ColumnName FROM @AllowedColumns)
          THEN ''  -- Skip invalid columns silently (security)
          ELSE
              '(' + QUOTENAME(ColumnName) + ' ' +
              CASE Operator
                  WHEN 'Equals'              THEN '='
                  WHEN 'NotEquals'           THEN '<>'
                  WHEN 'GreaterThan'         THEN '>'
                  WHEN 'GreaterThanOrEqual'  THEN '>='
                  WHEN 'LessThan'            THEN '<'
                  WHEN 'LessThanOrEqual'     THEN '<='
                  WHEN 'Like'                THEN 'LIKE'
                  WHEN 'NotLike'             THEN 'NOT LIKE'
                  WHEN 'In'                  THEN 'IN'
                  WHEN 'NotIn'               THEN 'NOT IN'
                  ELSE '='  -- fallback (should not happen)
              END + ' ' + dbo.fn_SafeQuote(Value) + ')'
    END
  FROM @Filters
  ORDER BY ParameterIndex;

  -- Final dynamic SQL
  DECLARE @Sql NVARCHAR(MAX) = N'
      SELECT 
          ID, 
          FirstName, 
          LastName, 
          Email, 
          Company,
          Title
      FROM dbo.[Contact]
      ' + CASE WHEN @Where != '' THEN N'WHERE ' + @Where ELSE N'' END + N'
      ORDER BY ID';

  -- Execute the dynamic SQL
  -- Dapper automatically makes all @filter_n parameters available inside sp_executesql
  EXEC sp_executesql @Sql;
END
GO