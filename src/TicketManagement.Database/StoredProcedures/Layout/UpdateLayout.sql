CREATE PROCEDURE [dbo].[UpdateLayout]
	@layoutId int,
	@venueId int,
	@description nvarchar(120)
AS
	UPDATE Layout
	SET Description = @description, VenueId = @venueId
	WHERE Id = @layoutId
