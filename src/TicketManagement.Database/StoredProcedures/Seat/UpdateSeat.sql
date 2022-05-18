CREATE PROCEDURE [dbo].[UpdateSeat]
	@seatId int,
	@areaId int,
	@row int,
	@number int
AS
	BEGIN TRANSACTION

	UPDATE Seat
	SET AreaId = @areaId, Row = @row, Number = @number
	WHERE Id = @seatId

	COMMIT
