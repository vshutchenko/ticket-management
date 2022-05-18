CREATE PROCEDURE [dbo].[InsertLayout]
	@venueId int,
	@description nvarchar(120),
	@layoutId int out
AS
	BEGIN TRANSACTION

	INSERT INTO Layout(Description, VenueId)
	VALUES(@description, @venueId)

	SET @layoutId = SCOPE_IDENTITY()

	COMMIT
