CREATE PROCEDURE [dbo].[InsertArea]
	@layoutId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@areaId int out
AS
	BEGIN TRANSACTION

	INSERT INTO Area(LayoutId, Description, CoordX, CoordY)
	VALUES(@layoutId, @description, @coordX, @coordY)
	SET @areaId = SCOPE_IDENTITY()

	COMMIT
