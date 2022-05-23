CREATE PROCEDURE [dbo].[UpdateSeat]
	@seatId int,
	@areaId int,
	@row int,
	@number int
AS
	UPDATE Seat
	SET AreaId = @areaId, Row = @row, Number = @number
	WHERE Id = @seatId
