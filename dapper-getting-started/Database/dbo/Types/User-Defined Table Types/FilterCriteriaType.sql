--------------------------------------------------------------
DROP TYPE IF EXISTS dbo.FilterCriteriaType;

--------------------------------------------------------------
CREATE TYPE dbo.FilterCriteriaType AS TABLE
(
   ColumnName       NVARCHAR(100) --PRIMARY KEY   -- The column name (required)
  ,Operator         NVARCHAR(20)
  ,LogicalOperator  NVARCHAR(10)  DEFAULT 'AND'
  ,[Value]          SQL_VARIANT
  ,ParameterIndex   INT           PRIMARY KEY   
);