CREATE PROCEDURE [dbo].[UpdateVenue]
	@venueId int,
	@description nvarchar(120),
	@address nvarchar(200),
	@phone nvarchar(30)
AS
	BEGIN TRANSACTION

	UPDATE Venue
	SET Description = @description, Address = @address, Phone = @phone
	WHERE Id = @venueId

	COMMIT
