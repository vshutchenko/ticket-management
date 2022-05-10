CREATE PROCEDURE [dbo].[SelectEvent]
	@eventId int
AS
	SELECT * FROM Event
	WHERE Id = @eventId