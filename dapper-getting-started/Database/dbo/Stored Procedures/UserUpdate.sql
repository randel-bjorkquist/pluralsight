--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[UserUpdate]
	 @ID     	  AS INT				 = NULL
  ,@FirstName	AS VARCHAR(50) = NULL
	,@LastName	AS VARCHAR(50) = NULL
	,@Email			AS VARCHAR(50) = NULL
	,@Deleted   AS BIT				 = NULL
AS

IF @ID IS NULL
  RETURN -1   -- Invalid @ID (NULL)

UPDATE [dbo].[User]
   SET FirstName = @FirstName
      ,LastName  = @LastName
      ,Email     = @Email
      ,Deleted   = @Deleted
	WHERE ID       = @ID

RETURN CASE WHEN @@ROWCOUNT = 1 THEN 0 ELSE 1 END;
--------------------------------------------------------------------------
