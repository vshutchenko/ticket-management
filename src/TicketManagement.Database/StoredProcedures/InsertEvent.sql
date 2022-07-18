CREATE PROCEDURE [dbo].[InsertEvent]
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@startDate datetime2,
	@endDate datetime2,
	@imageUrl nvarchar(max),
	@published bit,
	@eventId int out
AS
	BEGIN TRANSACTION

	DECLARE @Ids TABLE(AreaId int, EventAreaId int)

	INSERT INTO Event (Name, Description, LayoutId, StartDate, EndDate, ImageUrl, Published)
	VALUES (@name, @description, @layoutId, @startDate, @endDate, @imageUrl, @published)
	SET @eventId = SCOPE_IDENTITY()

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