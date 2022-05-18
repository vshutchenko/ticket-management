CREATE PROCEDURE [dbo].[DeleteEventArea]
	@eventAreaId int
AS
	DELETE FROM EventArea
	WHERE Id = @eventAreaId
