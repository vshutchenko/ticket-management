CREATE PROCEDURE [dbo].[SelectEvents]
	@offset int,
	@limit int
AS
	SELECT * FROM Event
	ORDER BY Id
	OFFSET @offset ROWS
	FETCH NEXT @limit ROWS ONLY