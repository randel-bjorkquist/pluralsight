-------------------------------------------------
CREATE PROCEDURE [dbo].[AddressDelete]
	@ID INT
AS
BEGIN
	DELETE FROM Address WHERE ID = @ID;
END;
