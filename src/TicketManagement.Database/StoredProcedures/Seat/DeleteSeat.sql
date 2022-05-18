CREATE PROCEDURE [dbo].[DeleteSeat]
	@seatId int
AS
	DELETE FROM Seat
	WHERE Id = @seatId
