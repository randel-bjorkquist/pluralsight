-------------------------------------------------
CREATE PROCEDURE [dbo].[ContactDelete]
	@ID INT
AS
BEGIN
	DELETE FROM Contact WHERE ID = @ID;
END;
