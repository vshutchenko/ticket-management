CREATE PROCEDURE [dbo].[InsertEventSeat]
	@eventAreaId int,
	@row int,
	@number int,
	@state int,
	@eventSeatId int out
AS
	BEGIN TRANSACTION

	INSERT INTO EventSeat(EventAreaId, Row, Number, State)
	VALUES(@eventAreaId, @row, @number, @state)

	SET @eventSeatId = SCOPE_IDENTITY()

	COMMIT
