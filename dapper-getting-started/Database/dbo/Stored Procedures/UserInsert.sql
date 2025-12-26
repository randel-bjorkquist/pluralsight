----------------------------------------------------------------
CREATE PROCEDURE [dbo].[UserInsert]
   @FirstName AS NVARCHAR(50) = NULL
  ,@LastName  AS NVARCHAR(50) = NULL
  ,@Email     AS NVARCHAR(50) = NULL
  ,@ID        AS INT OUTPUT
AS
SET NOCOUNT ON;
  
INSERT INTO [dbo].[User] (
   FirstName
  ,LastName
  ,Email
  ,Deleted
)
VALUES (
   @FirstName
  ,@LastName
  ,@Email
  ,0
)
  
SET @ID = SCOPE_IDENTITY()
RETURN CASE WHEN @@ROWCOUNT = 1 THEN 0 ELSE 1 END;
----------------------------------------------------------------
