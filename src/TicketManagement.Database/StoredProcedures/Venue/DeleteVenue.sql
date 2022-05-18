CREATE PROCEDURE [dbo].[DeleteVenue]
	@venueId int
AS
	DELETE FROM Venue
	WHERE Id = @venueId
