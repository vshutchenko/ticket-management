CREATE TABLE [dbo].[PurchasedSeat]
(
	[Id] int identity primary key,
    [PurchaseId] INT NOT NULL, 
    [EventSeatId] INT NOT NULL, 
    CONSTRAINT [FK_PurchasedSeat_Purchase] FOREIGN KEY ([PurchaseId]) REFERENCES [Purchase]([Id]), 
    CONSTRAINT [FK_PurchasedSeat_EventSeat] FOREIGN KEY ([EventSeatId]) REFERENCES [EventSeat]([Id])
)
