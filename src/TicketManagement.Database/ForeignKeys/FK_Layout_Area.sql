ALTER TABLE dbo.Area
ADD CONSTRAINT FK_Layout_Area FOREIGN KEY (LayoutId)     
    REFERENCES dbo.Layout (Id)