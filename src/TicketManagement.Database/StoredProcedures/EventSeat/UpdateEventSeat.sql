CREATE PROCEDURE [dbo].[UpdateEventSeat]
	@eventSeatId int,
	@eventAreaId int,
	@row int,
	@number int,
	@state int
AS
	BEGIN TRANSACTION

	UPDATE EventSeat
	SET EventAreaId = @eventAreaId, Row = @row, Number = @number, State = @state
	WHERE Id = @eventSeatId

	COMMIT
