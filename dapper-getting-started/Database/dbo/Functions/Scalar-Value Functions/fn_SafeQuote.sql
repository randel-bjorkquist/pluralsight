CREATE OR ALTER FUNCTION [dbo].[fn_SafeQuote](@Value SQL_VARIANT)
  RETURNS NVARCHAR(MAX)
AS
BEGIN
  IF @Value IS NULL 
    RETURN 'NULL';

  DECLARE @Type        NVARCHAR(128) = CAST(SQL_VARIANT_PROPERTY(@Value, 'BaseType') AS NVARCHAR(128));
  DECLARE @StringValue NVARCHAR(MAX);

  SET @StringValue = CASE @Type
      WHEN 'int'              THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS INT))
      WHEN 'bigint'           THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS BIGINT))
      WHEN 'smallint'         THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS SMALLINT))
      WHEN 'tinyint'          THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS TINYINT))
      WHEN 'bit'              THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS BIT))
      WHEN 'decimal'          THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS DECIMAL(38,10)))
      WHEN 'numeric'          THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS NUMERIC(38,10)))
      WHEN 'float'            THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS FLOAT))
      WHEN 'real'             THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS REAL))
      WHEN 'money'            THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS MONEY))
      WHEN 'date'             THEN CONVERT(NVARCHAR(30)  ,CAST(@Value AS DATE), 120)
      WHEN 'datetime'         THEN CONVERT(NVARCHAR(30)  ,CAST(@Value AS DATETIME), 120)
      WHEN 'datetime2'        THEN CONVERT(NVARCHAR(30)  ,CAST(@Value AS DATETIME2), 120)
      WHEN 'smalldatetime'    THEN CONVERT(NVARCHAR(30)  ,CAST(@Value AS SMALLDATETIME), 120)
      WHEN 'time'             THEN CONVERT(NVARCHAR(30)  ,CAST(@Value AS TIME), 14)
      WHEN 'uniqueidentifier' THEN CONVERT(NVARCHAR(MAX) ,CAST(@Value AS UNIQUEIDENTIFIER))
      ELSE                         CONVERT(NVARCHAR(MAX) ,@Value)
  END;

  RETURN CASE 
    WHEN @Type IN ('int','bigint','smallint','tinyint','bit','decimal','numeric','float','real','money')
      THEN @StringValue
    WHEN @Type IN ('date','datetime','datetime2','smalldatetime','time')
      THEN '''' + @StringValue + ''''
    ELSE '''' + REPLACE(@StringValue, '''', '''''') + ''''
  END;
END
GO