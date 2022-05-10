CREATE PROCEDURE [dbo].[UpdateEvent]
	@eventId int,
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int
AS
	BEGIN TRANSACTION

	DELETE FROM EventArea
	WHERE EventId = @eventId

	UPDATE Event
	SET Name = @name, Description = @description, LayoutId = @layoutId

	INSERT INTO EventArea (EventId, Description, CoordX, CoordY, Price)
	SELECT @eventId, Description, CoordX, CoordY, 0
	FROM Area
	WHERE LayoutId = @layoutId

	INSERT INTO EventSeat (EventAreaId, Row, Number, State)
	SELECT AreaId, Row, Number, 0
	FROM Seat
	WHERE AreaId IN(
	SELECT Id FROM Area
	WHERE LayoutId = @layoutId)

	COMMIT

