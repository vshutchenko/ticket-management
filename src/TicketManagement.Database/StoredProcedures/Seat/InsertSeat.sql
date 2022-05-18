CREATE PROCEDURE [dbo].[InsertSeat]
	@areaId int,
	@row int,
	@number int,
	@seatId int out
AS
	BEGIN TRANSACTION

	INSERT INTO Seat(AreaId, Row, Number)
	VALUES(@areaId, @row, @number)

	SET @seatId = SCOPE_IDENTITY()

	COMMIT
