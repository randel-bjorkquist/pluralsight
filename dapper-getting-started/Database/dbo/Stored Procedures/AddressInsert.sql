-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressInsert]
	 @ID     				INT OUTPUT
	,@ContactID			INT
	,@AddressType	  VARCHAR(10)
	,@StreetAddress	VARCHAR(50)
	,@City					VARCHAR(50)
	,@StateID				INT
	,@PostalCode		VARCHAR(20)
AS
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
		
	SET @ID = CAST(SCOPE_IDENTITY() AS INT)
END;
-------------------------------------------------
