CREATE PROCEDURE [dbo].[InsertVenue]
	@description nvarchar(120),
	@address nvarchar(200),
	@phone nvarchar(30)
AS
	INSERT INTO Venue(Description, Address, Phone)
	VALUES(@description, @address, @phone)

	SELECT SCOPE_IDENTITY()
