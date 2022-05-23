CREATE PROCEDURE [dbo].[InsertArea]
	@layoutId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int
AS
	INSERT INTO Area(LayoutId, Description, CoordX, CoordY)
	VALUES(@layoutId, @description, @coordX, @coordY)
	SELECT SCOPE_IDENTITY()
