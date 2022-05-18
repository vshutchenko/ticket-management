CREATE PROCEDURE [dbo].[DeleteArea]
	@areaId int
AS
	DELETE FROM Area
	WHERE Id = @areaId
