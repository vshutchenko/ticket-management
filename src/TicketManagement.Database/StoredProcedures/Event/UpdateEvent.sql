﻿CREATE PROCEDURE [dbo].[UpdateEvent]
	@eventId int,
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@startDate datetime2,
	@endDate datetime2
AS
	BEGIN TRANSACTION

	DELETE FROM EventArea
	WHERE EventId = @eventId

	UPDATE Event
	SET Name = @name, Description = @description, LayoutId = @layoutId
	WHERE Id = @eventId

	DECLARE @areaId int
	DECLARE @areaDescription nvarchar(200)
	DECLARE @coordX int
	DECLARE @coordY int
	DECLARE @eventAreaId int

	DECLARE AREA_CURSOR CURSOR 
	LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT Id, Description, CoordX, CoordY
	FROM Area
	WHERE LayoutId = @layoutId

	OPEN AREA_CURSOR
	FETCH NEXT FROM AREA_CURSOR INTO @areaId, @areaDescription, @coordX, @coordY
	WHILE @@FETCH_STATUS = 0
	BEGIN 
    
		INSERT INTO EventArea (EventId, Description, CoordX, CoordY, Price)
		VALUES (@eventId, @areaDescription, @coordX, @coordY, 0)
	
		SET @eventAreaId = SCOPE_IDENTITY()

		INSERT INTO EventSeat (EventAreaId, Row, Number, State)
		SELECT @eventAreaId, Row, Number, 0
		FROM Seat
		WHERE AreaId = @areaId

		FETCH NEXT FROM AREA_CURSOR INTO @areaId, @areaDescription, @coordX, @coordY

	END

	CLOSE AREA_CURSOR
	DEALLOCATE AREA_CURSOR

	COMMIT

