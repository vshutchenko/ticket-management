CREATE PROCEDURE [dbo].[UpdateEventArea]
	@eventAreaId int,
	@eventId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@price decimal(18)
AS
	UPDATE EventArea
	SET EventId = @eventId, Description = @description, CoordX = @coordX, CoordY = @coordY, Price = @price
	WHERE Id = @eventAreaId