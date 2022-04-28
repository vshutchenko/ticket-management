--- Venue
insert into dbo.Venue
	values 
		('First venue', 'First venue address', '123 45 678 90 12')

--- Layout
insert into dbo.Layout
	values 
		(1, 'First layout'),
		(1, 'Second layout')

--- Area
insert into dbo.Area
	values 
		(1, 'First area of first layout', 1, 1),
		(1, 'Second area of first layout', 1, 1),
		(2, 'First area of second layout', 4, 4)

--- Seat
insert into dbo.Seat
	values
		(1, 1, 1),
		(1, 1, 2),
		(1, 1, 3),
		(1, 2, 2),
		(2, 1, 1),
		(1, 2, 1)
