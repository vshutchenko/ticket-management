CREATE PROCEDURE [dbo].[UpdateEvent]
	@eventId int,
	@name nvarchar(120),
	@description nvarchar(max),
	@startDate datetime2,
	@endDate datetime2,
	@imageUrl nvarchar(max),
	@published bit
AS
	UPDATE Event
	SET Name = @name, Description = @description, StartDate = @startDate, EndDate = @endDate, ImageUrl = @imageUrl, Published = @published
	WHERE Id = @eventId