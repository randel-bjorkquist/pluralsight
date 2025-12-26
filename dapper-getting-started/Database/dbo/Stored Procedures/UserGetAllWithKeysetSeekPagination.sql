--------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE UserGetAllWithKeysetSeekPagination
   @PageSize      AS INT          = 0       -- <= 0 means return all records (no pagination)
  ,@OrderByColumn AS NVARCHAR(50) = 'ID'    -- Allowed: ID, FirstName, LastName, Email, Company, Title
  ,@SortDirection AS CHAR(4)      = 'DESC'  -- 'ASC' or 'DESC'
  ,@AfterID       AS INT          = NULL    -- Anchor ID from previous page (NULL = first page)
  ,@AfterValue    AS SQL_VARIANT  = NULL    -- Anchor value of the sorted column from previous page

   -- OUTPUT parameters for next page
  ,@NextID    AS INT         OUTPUT
  ,@NextValue AS SQL_VARIANT OUTPUT
  ,@HasMore   AS BIT         OUTPUT
AS
BEGIN
  SET NOCOUNT ON;

  -- Normalize and validate inputs
  IF @SortDirection NOT IN ('ASC', 'DESC')
    SET @SortDirection = 'DESC';

  IF @OrderByColumn NOT IN ('ID', 'FirstName', 'LastName', 'Email', 'Deleted')
    SET @OrderByColumn = 'ID';

  DECLARE @ApplyPagination BIT = CASE WHEN @PageSize > 0 THEN 1 ELSE 0 END;

  -- Initialize OUTPUT parameters
  SET @NextID    = NULL;
  SET @NextValue = NULL;
  SET @HasMore   = 0;

  -- Table variable to hold results with ordering info
  DECLARE @Results TABLE (
     ID          INT
    ,FirstName   VARCHAR(50)
    ,LastName    VARCHAR(50)
    ,Email       VARCHAR(50)
    ,Deleted     BIT
    ,OrderValue  SQL_VARIANT
    ,RowNum      INT IDENTITY(1,1));  -- To preserve insertion order

  -- Insert the filtered/paginated data
  INSERT INTO @Results (ID, FirstName, LastName, Email, Deleted, OrderValue)
  SELECT 
     U.ID
    ,U.FirstName
    ,U.LastName
    ,U.Email
    ,U.Deleted
    ,CASE @OrderByColumn
          WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
          WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
          WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
          WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
          WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
      END AS OrderValue
  FROM
    [dbo].[User] AS U
  WHERE
    @ApplyPagination = 0  -- No pagination: return everything
    OR (@AfterID IS NULL) -- First page
    OR ( -- Keyset condition: continue from previous anchor  
        ( -- Primary sort column comparison
         ( @SortDirection = 'DESC' AND
           CAST(@AfterValue AS SQL_VARIANT) > CASE @OrderByColumn
                                                WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
                                                WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
                                                WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
                                                WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
                                                WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
                                              END )
         OR
         ( @SortDirection = 'ASC' AND 
           CAST(@AfterValue AS SQL_VARIANT) < CASE @OrderByColumn
                                                  WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
                                                  WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
                                                  WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
                                                  WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
                                                  WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
                                              END )
        )
        OR
        ( -- Same value in sort column ? use ID tie-breaker
          CAST(@AfterValue AS SQL_VARIANT) = CASE @OrderByColumn
                                                 WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
                                                 WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
                                                 WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
                                                 WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
                                                 WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
                                             END
          AND
          ((@SortDirection = 'DESC' AND U.ID < @AfterID)
           OR
           (@SortDirection = 'ASC'  AND U.ID > @AfterID))
        ))
  ORDER BY
     CASE WHEN @SortDirection = 'DESC' THEN
             CASE @OrderByColumn
                 WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
                 WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
                 WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
                 WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
                 WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
             END
         END DESC
    ,CASE WHEN @SortDirection = 'ASC' THEN
         CASE @OrderByColumn
             WHEN 'ID'        THEN CAST(U.ID        AS SQL_VARIANT)
             WHEN 'FirstName' THEN CAST(U.FirstName AS SQL_VARIANT)
             WHEN 'LastName'  THEN CAST(U.LastName  AS SQL_VARIANT)
             WHEN 'Email'     THEN CAST(U.Email     AS SQL_VARIANT)
             WHEN 'Deleted'   THEN CAST(U.Deleted   AS SQL_VARIANT)
         END
     END ASC
    ,CASE WHEN @SortDirection = 'DESC' THEN U.ID END DESC  -- Tie-breaker matches direction
    ,CASE WHEN @SortDirection = 'ASC'  THEN U.ID END ASC   -- Tie-breaker matches direction
  OFFSET 0 ROWS
  FETCH NEXT CASE WHEN @ApplyPagination = 1 THEN (@PageSize + 1) ELSE 2147483647 END ROWS ONLY;  -- Large number = effectively no limit

  -- Determine if there are more rows and set OUTPUT anchor values
  DECLARE @FetchedRows INT = @@ROWCOUNT;  -- Total rows fetched (up to PageSize + 1)

  -- Only apply "fetch one extra + trim" logic when pagination is active
  IF @ApplyPagination = 1
    BEGIN
      IF @FetchedRows > @PageSize
        BEGIN
          SET @HasMore = 1;
          DELETE FROM @Results WHERE RowNum = @FetchedRows;  -- Remove the extra row
          SET @FetchedRows = @PageSize;
        END
      ELSE
        BEGIN
          SET @HasMore = 0;
        END
    END
  ELSE
    BEGIN
      -- When returning all records (@ApplyPagination = 0), do NOT trim anything
      SET @HasMore = 0;
      -- @FetchedRows remains as-is (all rows)
    END


  -- Set next anchor only if we have rows to return
  IF @FetchedRows > 0
    BEGIN
      SELECT 
         @NextID    = ID
        ,@NextValue = OrderValue
      FROM 
        @Results
      WHERE 
        RowNum = @FetchedRows;  -- Last row among the ones we'll actually return
    END
  ELSE
    BEGIN
      -- No rows returned ? clear outputs
      SET @NextID    = NULL;
      SET @NextValue = NULL;
    END

  -- Return the actual data (without internal columns)
  SELECT 
     ID
    ,FirstName
    ,LastName
    ,Email
    ,Deleted
  FROM 
    @Results
  ORDER BY 
    RowNum;  -- Preserves original order
END;
GO