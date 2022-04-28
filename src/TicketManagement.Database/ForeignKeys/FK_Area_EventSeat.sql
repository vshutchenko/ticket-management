ALTER TABLE dbo.EventSeat
ADD CONSTRAINT FK_Area_EventSeat FOREIGN KEY ([EventAreaId])     
    REFERENCES dbo.EventArea (Id)