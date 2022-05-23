CREATE PROCEDURE [dbo].[UpdateArea]
	@areaId int,
	@layoutId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int
AS
	UPDATE Area
	SET LayoutId = @layoutId, Description = @description, CoordX = @coordX, CoordY = @coordY
	WHERE Id = @areaId
