CREATE TABLE [dbo].[Purchase]
(
	[Id] int identity primary key, 
    [UserId] NVARCHAR(450) NOT NULL, 
    [Price] DECIMAL NOT NULL, 
    [EventId] INT NOT NULL, 
    CONSTRAINT [FK_Purchase_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_Purchase_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id])
)
