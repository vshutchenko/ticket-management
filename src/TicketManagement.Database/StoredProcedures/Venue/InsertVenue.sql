CREATE PROCEDURE [dbo].[InsertVenue]
	@description nvarchar(120),
	@address nvarchar(200),
	@phone nvarchar(30),
	@venueId int out
AS
	BEGIN TRANSACTION

	INSERT INTO Venue(Description, Address, Phone)
	VALUES(@description, @address, @phone)

	SET @venueId = SCOPE_IDENTITY()

	COMMIT
