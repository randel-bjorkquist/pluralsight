-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressUpdate]
	 @ID            INT OUTPUT
	,@ContactID     INT
	,@AddressType   VARCHAR(10)
	,@StreetAddress VARCHAR(50)
	,@City          VARCHAR(50)
	,@StateID       INT
	,@PostalCode    VARCHAR(20)
AS
BEGIN
	UPDATE Address
		 SET ContactID     = @ContactID
				,AddressType   = @AddressType
				,StreetAddress = @StreetAddress
				,City          = @City
				,StateID       = @StateID
				,PostalCode    = @PostalCode
	 WHERE ID            = @ID

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO Address(
			 ContactID
			,AddressType
			,StreetAddress
			,City
			,StateID
			,PostalCode)
		VALUES(
			 @ContactID
			,@AddressType
			,@StreetAddress
			,@City
			,@StateID
			,@PostalCode);
		
		SET @ID = cast(scope_identity() as int)
	END;
END;
