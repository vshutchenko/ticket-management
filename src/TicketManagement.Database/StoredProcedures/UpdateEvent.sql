CREATE PROCEDURE [dbo].[UpdateEvent]
	@eventId int,
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@startDate datetime2,
	@endDate datetime2,
	@imageUrl nvarchar(max),
	@published bit
AS
	BEGIN TRANSACTION

	DELETE FROM EventArea
	WHERE EventId = @eventId

	UPDATE Event
	SET Name = @name, Description = @description, LayoutId = @layoutId, StartDate = @startDate, EndDate = @endDate, ImageUrl = @imageUrl, Published = @published
	WHERE Id = @eventId

	DECLARE @Ids TABLE(AreaId int, EventAreaId int)

	MERGE INTO EventArea
	USING
	(
		SELECT Id, Description, CoordX, CoordY
		FROM Area
		WHERE LayoutId = @layoutId
	) As Src
	ON 1 = 0
	WHEN NOT MATCHED BY TARGET THEN
	INSERT (EventId, Description, CoordX, CoordY, Price)
	VALUES (@eventId, Src.Description, Src.CoordX, Src.CoordY, 0)
	OUTPUT Src.Id AS AreaId, Inserted.Id AS EventAreaId
	INTO @Ids(AreaId, EventAreaId);

	INSERT INTO EventSeat (EventAreaId, Row, Number, State)
		SELECT a.EventAreaId, s.Row, s.Number, 0
		FROM Seat as s
		INNER JOIN @Ids as a
		ON s.AreaId = a.AreaId

	COMMIT