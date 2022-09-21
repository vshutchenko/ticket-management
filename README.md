## Ticket management

This project represents an application for ticket management. The project has service-oriented architecture. Presentation layer is implemented as ASP .NET Core MVC application. The project contains Web APIs for event and venue management, purchase operations and authentication endpoint. All APIs perform calls to a DAL. Data Access Layer realizes repository pattern. DAL has two implementations - ADO.NET and Entity Framework. Application uses TAP pattern - most of operations are implemented in asynchronous way.

### Features

- Authentication and authorization. Application supports user roles with different abilities (Admin, User, Event Manager, Venue Manager). Current implementation provides next functionality for roles:

- User - has ability to see information about events and purchase seats on events. Each user can modify his profile information and see his purchase history.

- Event Manager - has ability to create, update and delete events.

- Venue Manager - has ability to create, update and delete venues, layouts, areas and seats.

- Localization. Supported languages: English, Russian, Belarusian.

- Logging. All components are logging incoming requests.

- API Documentation. All APIs have swagger documentation.

- Event import from JSON file.

### Testing

The solution contains unit and integration tests. To run integration tests you need to specify connection string in the [appsettings.json](/test/TicketManagement.IntegrationTests/appsettings.json) file.

  
To run solution in a debug mode you need to specify multiple startup projects in the solution properties.

### Login information

##### Event Manager:

- login: eventManager@gmail.com

- password: Password123#

##### Venue Manager:

- login: manager1@gmail.com

- password: Password123#

##### User:

- login: user1@gmail.com

- password: Password123#

## Third Party Event Editor

This project is an utility for the Ticket management application. It provides the possibility to manage and extract events from another vendor. The application uses JSON file as storage and supports CRUD operations fo events. JSON file with events can be downloaded and used for import in the main application.

## React UI Application

The new UI was provided for Event management and Profile management. The main MVC application uses feature flags to redirect all requests to a new UI.

## How to run
### Requirements
- React 18.2.0
- npm 8.15.0

### 1. Use start.bat file
- Publish [TicketManagement.Database](/src/TicketManagement.Database/) project and get connection string
- Change connection strings in appsettings.json files of projects which use database:
	- [TicketManagement.UserApi](/src/TicketManagement.UserApi/appsettings.json)
	- [TicketManagement.VenueApi](/src/TicketManagement.VenueApi/appsettings.json)
	- [TicketManagement.EventApi](/src/TicketManagement.EventApi/appsettings.json)
	- [TicketManagement.PurchaseApi](/src/TicketManagement.PurchaseApi/appsettings.json)
	
- Run [start.bat](/start.bat) file

### 2. Manually in Visual Studio
- Publish [TicketManagement.Database](/src/TicketManagement.Database/) project and get connection string.
- Change connection strings in appsettings.json files of projects which use database:
	- [TicketManagement.UserApi](/src/TicketManagement.UserApi/appsettings.json)
	- [TicketManagement.VenueApi](/src/TicketManagement.VenueApi/appsettings.json)
	- [TicketManagement.EventApi](/src/TicketManagement.EventApi/appsettings.json)
	- [TicketManagement.PurchaseApi](/src/TicketManagement.PurchaseApi/appsettings.json)

- Change startup projects in the solution properties or run projects manually one by one. Projects that should be started:
	- [TicketManagement.UserApi](/src/TicketManagement.UserApi)
	- [TicketManagement.VenueApi](/src/TicketManagement.VenueApi)
	- [TicketManagement.EventApi](/src/TicketManagement.EventApi)
	- [TicketManagement.PurchaseApi](/src/TicketManagement.PurchaseApi)
	- [TicketManagement.UI](/src/TicketManagement.UI)
	- [TicketManagement.WebApplication](/src/TicketManagement.WebApplication)
