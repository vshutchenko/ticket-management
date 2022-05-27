This project represents Data Access layer and Business logic layer of the Ticket management application.
Data Access layer allows to manipulate core entities using ADO.NET. Bussiness logic layer performs
validation and interact with Data Access layer.

Current implementation define CRUD operations for Event, Venue, Layout, Area and Seat entities; updating price for EventArea entity
and changing state for EventSeat entity.

The solution contains unit and integration tests. To run integration tests you need to specify connection string
in the appsettings.json file in the TicketManagement.IntegrationTests folder.

