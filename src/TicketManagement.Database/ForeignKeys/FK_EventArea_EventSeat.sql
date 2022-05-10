ALTER TABLE dbo.EventSeat
ADD CONSTRAINT FK_EventArea_EventSeat FOREIGN KEY ([EventAreaId])     
    REFERENCES dbo.EventArea (Id)
    ON DELETE CASCADE