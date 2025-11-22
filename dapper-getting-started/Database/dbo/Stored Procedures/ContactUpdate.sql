-------------------------------------------------
CREATE PROCEDURE [dbo].[ContactUpdate]
	 @ID     	  INT OUTPUT
  ,@FirstName	VARCHAR(50)
	,@LastName	VARCHAR(50)
	,@Company		VARCHAR(50)
	,@Title			VARCHAR(50)
	,@Email			VARCHAR(50)
AS
BEGIN
	UPDATE Contact
	   SET FirstName = @FirstName
			  ,LastName  = @LastName
			  ,Company   = @Company
			  ,Title     = @Title
			  ,Email     = @Email
	 WHERE ID        = @ID

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [dbo].[Contact](
			 [FirstName]
			,[LastName]
			,[Company]
			,[Title]
      ,[Email])
		VALUES(
			 @FirstName
			,@LastName
			,@Company
			,@Title
			,@Email);

		SET @ID = cast(scope_identity() as int)
	END;
END;
