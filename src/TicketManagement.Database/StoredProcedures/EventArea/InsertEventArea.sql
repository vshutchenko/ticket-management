CREATE PROCEDURE [dbo].[InsertEventArea]
	@eventId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@price decimal(18),
	@areaId int out
AS
	BEGIN TRANSACTION

	INSERT INTO EventArea(EventId, Description, CoordX, CoordY, Price)
	VALUES(@eventId, @description, @coordX, @coordY, @price)
	SET @areaId = SCOPE_IDENTITY()

	COMMIT
