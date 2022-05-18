CREATE PROCEDURE [dbo].[DeleteLayout]
	@layoutId int
AS
	DELETE FROM Layout
	WHERE Id = @layoutId
