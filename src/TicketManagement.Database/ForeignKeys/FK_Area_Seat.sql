﻿ALTER TABLE dbo.Seat
ADD CONSTRAINT FK_Area_Seat FOREIGN KEY (AreaId)     
    REFERENCES dbo.Area (Id)