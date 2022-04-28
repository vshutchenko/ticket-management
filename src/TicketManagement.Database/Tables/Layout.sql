CREATE TABLE [dbo].[Layout]
(
	[Id] int identity primary key,
	[VenueId] int NOT NULL,
	[Description] nvarchar(120) NOT NULL,
)
