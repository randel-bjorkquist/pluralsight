-------------------------------------------------
CREATE procedure [dbo].[ContactGetByID]
	@ID INT
AS
BEGIN
	SELECT
		 [ID]
		,[FirstName]
		,[LastName]
		,[Company]
		,[Title]
		,[Email]
	FROM
		[dbo].[Contact]
	WHERE
		ID = @ID;
  -----------------------------------------------
	SELECT 
		 ID
		,ContactID
		,AddressType
		,StreetAddress
		,City
		,StateID
		,PostalCode
	FROM 
		[dbo].[Address]
	WHERE 
		ContactID = @ID;
END
-------------------------------------------------