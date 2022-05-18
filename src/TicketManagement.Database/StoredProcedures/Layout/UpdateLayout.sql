CREATE PROCEDURE [dbo].[UpdateLayout]
	@layoutId int,
	@venueId int,
	@description nvarchar(120)
AS
	BEGIN TRANSACTION

	UPDATE Layout
	SET Description = @description, VenueId = @venueId
	WHERE Id = @layoutId

	COMMIT
