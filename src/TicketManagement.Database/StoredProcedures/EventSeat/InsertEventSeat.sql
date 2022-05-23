CREATE PROCEDURE [dbo].[InsertEventSeat]
	@eventAreaId int,
	@row int,
	@number int,
	@state int,
	@eventSeatId int out
AS
	INSERT INTO EventSeat(EventAreaId, Row, Number, State)
	VALUES(@eventAreaId, @row, @number, @state)

	SELECT SCOPE_IDENTITY()
