CREATE PROCEDURE [dbo].[ContactSave]
	 @ID     	  INT OUTPUT
	,@FirstName	VARCHAR(50)
	,@LastName	VARCHAR(50)
	,@Company	  VARCHAR(50)
	,@Title		  VARCHAR(50)
	,@Email		  VARCHAR(50)
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
		INSERT INTO Contact(
			 FirstName
      ,LastName
      ,Company
      ,Title
      ,Email
		)
		VALUES(
			 @FirstName
			,@LastName
			,@Company
			,@Title
			,@Email
		);

		SET @ID = CAST(SCOPE_IDENTITY() AS INT)
	END;
END;