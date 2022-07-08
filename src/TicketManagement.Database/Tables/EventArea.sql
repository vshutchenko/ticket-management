CREATE TABLE [dbo].[EventArea]
(
	[Id] int identity primary key,
	[EventId] int NOT NULL,
	[Description] nvarchar(200) NOT NULL,
	[CoordX] int NOT NULL,
	[CoordY] int NOT NULL,
	[Price] decimal(16, 2) NOT NULL
)
