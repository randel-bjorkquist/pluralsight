CREATE OR ALTER PROCEDURE dbo.ContactGetAllWithOffsetPagination
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
  IF @OrderByColumn NOT IN ('ID', 'FirstName', 'LastName', 'Email', 'Company', 'Title')
    SET @OrderByColumn = 'ID';

  DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

  -- Build dynamic query
  DECLARE @SQL NVARCHAR(MAX) = N'
SELECT
  ID, FirstName, LastName, Email, Company, Title 
FROM
  dbo.Contact 
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
  SELECT @TotalRows = COUNT(*) FROM dbo.Contact;
END
GO

/*** COMMENTED OUT: R&D CODE *********************************************************************************************

CREATE OR ALTER PROCEDURE dbo.ContactGetAllWithOffsetPagination
   @PageNumber     AS INT           = 1     -- 1-based
  ,@PageSize       AS INT           = 20
  ,@OrderByColumn  AS NVARCHAR(50)  = 'ID'  -- Allowed columns
  ,@SortDirection  AS CHAR(4)       = 'ASC' -- 'ASC' or 'DESC'
  ,@TotalRows      AS INT OUTPUT            -- Optional: total matching rows
AS
BEGIN
  SET NOCOUNT ON;

  -- Validate and normalize
  IF @PageNumber < 1 SET @PageNumber = 1;
  IF @PageSize   < 1 SET @PageSize   = 20;

  IF @SortDirection NOT IN ('ASC', 'DESC')
    SET @SortDirection = 'ASC';

  IF @OrderByColumn NOT IN ('ID', 'FirstName', 'LastName', 'Email', 'Company', 'Title')
    SET @OrderByColumn = 'ID';

  DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

  -- Dynamic ORDER BY using CASE expressions
  SELECT 
     ID, FirstName, LastName, Email, Company, Title
  FROM 
    dbo.Contact
  ORDER BY
    CASE WHEN @SortDirection = 'ASC' THEN CASE @OrderByColumn
                                              WHEN 'ID'        THEN ID
                                              WHEN 'FirstName' THEN FirstName
                                              WHEN 'LastName'  THEN LastName
                                              WHEN 'Email'     THEN Email
                                              WHEN 'Company'   THEN Company
                                              WHEN 'Title'     THEN Title
                                          END
    END ASC,
    CASE WHEN @SortDirection = 'DESC' THEN CASE @OrderByColumn
                                               WHEN 'ID'        THEN ID
                                               WHEN 'FirstName' THEN FirstName
                                               WHEN 'LastName'  THEN LastName
                                               WHEN 'Email'     THEN Email
                                               WHEN 'Company'   THEN Company
                                               WHEN 'Title'     THEN Title
                                           END
    END DESC,
    ID ASC  -- Stable tie-breaker (always ASC to avoid randomness)
  OFFSET @Offset ROWS
  FETCH NEXT @PageSize ROWS ONLY;

  -- Return total count for UI (e.g., "Showing 21-40 of 157")
  SELECT @TotalRows = COUNT(*) FROM dbo.Contact;
END

** COMMENTED OUT: R&D CODE ******************************************************************************************** */
