--------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[UserDelete]
  @ID AS INT = NULL
AS
SET NOCOUNT ON;

IF @ID IS NULL
  RETURN -1   -- Invalid @ID (NULL)

UPDATE [dbo].[User]
   SET Deleted = 1
 WHERE ID = @ID AND Deleted = 0

RETURN CASE WHEN @@ROWCOUNT = 1 THEN 0 ELSE 1 END;
GO
