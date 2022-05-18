CREATE PROCEDURE [dbo].[DeleteEventSeat]
	@eventSeatId int
AS
	DELETE FROM EventSeat
	WHERE Id = @eventSeatId
