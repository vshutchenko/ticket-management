--- Venue
insert into dbo.Venue
	values 
		('First venue', 'First venue address', '123 45 678 90 12'),
		('Second venue', 'Second venue address', '213 345 34 3434')

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

--- Event
insert into dbo.Event
	values
		('First event', 'First event description', 1, '2023-01-01 10:00:00', '2023-01-01 15:00:00', 'url', 1)


--- EventArea
insert into dbo.EventArea
	values 
		(1, 'Event area of first event', 1, 1, 15)

--- EvenSeat
insert into dbo.EventSeat
	values
		(1, 1, 1, 1),
		(1, 1, 2, 1),
		(1, 1, 3, 1),
		(1, 2, 2, 0),
		(1, 2, 1, 0)
