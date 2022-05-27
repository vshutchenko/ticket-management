CREATE PROCEDURE [dbo].[InsertLayout]
	@venueId int,
	@description nvarchar(120)
AS
	INSERT INTO Layout(Description, VenueId)
	VALUES(@description, @venueId)

	SELECT SCOPE_IDENTITY()
