CREATE PROCEDURE [dbo].[InsertEvent]
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@startDate datetime2,
	@endDate datetime2,
	@eventId int out
AS
	BEGIN TRANSACTION

	INSERT INTO Event (Name, Description, LayoutId, StartDate, EndDate)
	VALUES (@name, @description, @layoutId, @startDate, @endDate)
	SET @eventId = SCOPE_IDENTITY()

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