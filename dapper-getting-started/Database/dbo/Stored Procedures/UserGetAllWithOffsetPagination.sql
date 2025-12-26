CREATE OR ALTER PROCEDURE dbo.UserGetAllWithOffsetPagination
   @PageNumber     AS INT           = 1
  ,@PageSize       AS INT           = 20
  ,@OrderByColumn  AS NVARCHAR(50)  = 'ID'
  ,@SortDirection  AS CHAR(4)       = 'ASC'
  ,@TotalRows      AS INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;

  -- Validate and normalize
  IF @PageNumber < 1 SET @PageNumber = 1;
  IF @PageSize   < 1 SET @PageSize   = 20;

  IF @SortDirection NOT IN ('ASC', 'DESC')
    SET @SortDirection = 'ASC';

  -- Whitelist columns to prevent injection
  IF @OrderByColumn NOT IN ('ID', 'FirstName', 'LastName', 'Email', 'Deleted')
    SET @OrderByColumn = 'ID';

  DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

  -- Build dynamic query
  DECLARE @SQL NVARCHAR(MAX) = N'
SELECT
  ID, FirstName, LastName, Email, Deleted
FROM
  [dbo].[User]
ORDER BY
  [' + @OrderByColumn + N'] ' + @SortDirection;

  -- Add stable tie-breaker only if not sorting by ID
  IF @OrderByColumn <> 'ID'
    SET @SQL += N', [ID] ' + @SortDirection;

  SET @SQL += N'
OFFSET @Offset ROWS 
FETCH NEXT @PageSize ROWS ONLY;';

  -- Execute
  EXEC sp_executesql @SQL, N'@Offset INT, @PageSize INT', @Offset = @Offset, @PageSize = @PageSize;

  -- Total count
  SELECT @TotalRows = COUNT(*) FROM [dbo].[User];
END
GO
