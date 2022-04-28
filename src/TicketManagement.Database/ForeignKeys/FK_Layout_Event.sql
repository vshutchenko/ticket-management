ALTER TABLE dbo.[Event]
ADD CONSTRAINT FK_Layout_Event FOREIGN KEY (LayoutId)     
    REFERENCES dbo.Layout (Id)