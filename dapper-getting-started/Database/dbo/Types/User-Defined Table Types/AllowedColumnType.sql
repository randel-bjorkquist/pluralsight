--------------------------------------------------------------
DROP TYPE IF EXISTS dbo.AllowedColumnType;

--------------------------------------------------------------
CREATE TYPE dbo.AllowedColumnType AS TABLE
(
   ColumnName NVARCHAR(100) PRIMARY KEY   -- The column name (required)
  ,IsNullable BIT           DEFAULT 0     -- Optional: whether NULL checks are allowed (useful for IS NULL / IS NOT NULL)
  ,DataType   NVARCHAR(50)  NULL          -- Optional: expected SQL data type (e.g., 'int', 'nvarchar', 'datetime') for future validation or casting
);
