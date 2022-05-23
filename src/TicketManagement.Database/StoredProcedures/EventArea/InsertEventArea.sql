CREATE PROCEDURE [dbo].[InsertEventArea]
	@eventId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@price decimal(18)
AS
	INSERT INTO EventArea(EventId, Description, CoordX, CoordY, Price)
	VALUES(@eventId, @description, @coordX, @coordY, @price)
	SELECT SCOPE_IDENTITY()
