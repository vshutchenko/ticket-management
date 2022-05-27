CREATE PROCEDURE [dbo].[InsertSeat]
	@areaId int,
	@row int,
	@number int
AS
	INSERT INTO Seat(AreaId, Row, Number)
	VALUES(@areaId, @row, @number)

	SELECT SCOPE_IDENTITY()
