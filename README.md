## Ticket management

This project represents an application for ticket management. The project has service-oriented architecture. Presentation layer is implemented as ASP .NET Core MVC application. The project contains Web APIs for event and venue management, purchase operations and authentication endpoint. All APIs perform calls to a DAL. Data Access Layer realizes repository pattern. DAL has two imlementations - ADO.NET and Entity Framework. Application uses TAP pattern - most of operations are implemented in asynchronous way.

### Features
- Authentification and authorization. Application supports user roles with different abilities (Admin, User, Event Manager, Venue Manager). Current implementation provides next functionality for roles:
    - User - has ability to see information about events and purchase seats on events. Each user can modify his profile information and see his purchase history. 
    - Event Manager - has ability to create, update and delete events.
    - Venue Manager - has ability to create, update and delete venues, layouts, areas and seats.
- Localization. Supported languages: English, Russian, Belarussian.
- Logging. All components are logging incoming requests.
- API Documentation. All APIs have swagger documentation.
- Event import from JSON file.
### Testing
The solution contains unit and integration tests. To run integration tests you need to specify connection string in the appsettings.json file in the TicketManagement.IntegrationTests folder.

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
