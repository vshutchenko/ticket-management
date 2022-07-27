﻿CREATE PROCEDURE [dbo].[InsertEvent]
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

	INSERT INTO [Event] (Name, Description, LayoutId, StartDate, EndDate, ImageUrl, Published)
	VALUES (@name, @description, @layoutId, @startDate, @endDate, @imageUrl, @published)

	SET @eventId = SCOPE_IDENTITY();

	INSERT INTO [EventArea] ([EventId], [Description], [CoordX], [CoordY], [Price])
		SELECT e.[Id], a.[Description], a.[CoordX], a.[CoordY], 0 FROM [Event] as e 
		INNER JOIN [Area] as a ON a.[LayoutId] = e.[LayoutId]
		WHERE e.[Id] = @eventId;

	INSERT INTO [EventSeat] ([EventAreaId], [Row], [Number], [State])
		SELECT ea.[Id] as EventAreaId, s.[Row], s.[Number], 0 as State FROM [Event] as e
		INNER JOIN [Area] as a ON a.[LayoutId] = e.[LayoutId]
		INNER JOIN [EventArea] as ea ON ea.[EventId] = e.[Id] AND a.Description = ea.Description
		INNER JOIN [Seat] as s ON s.[AreaId] = a.[Id]
		WHERE e.[Id] = @eventId;

	COMMIT

	RETURN