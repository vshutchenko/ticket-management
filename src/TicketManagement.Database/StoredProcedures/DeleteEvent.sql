CREATE PROCEDURE [dbo].[DeleteEvent]
	@eventId int
AS
	DELETE FROM Event
	WHERE Id = @eventId
