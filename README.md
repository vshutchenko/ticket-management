## Ticket management

This project represents an application for ticket management. The project has three layerd architecture. Presentation layer is implemented as ASP .NET Core MVC application. Bussiness Layer represents a set of services which provides validation  and performs calls to a DAL. Data Access Layer realizes repository pattern. DAL has two imlementations - ADO.NET and Entity Framework. Application uses TAP pattern - most of operations are implemented in asynchronous way.

### Features
- Authentification and authorization. Application supports user roles with different abilities (Admin, User, Event Manager, Venue Manager). Current implementation provides next functionality for roles:
    - User - has ability to see information about events and purchase seats on events. Each user can modify his profile information and see his purchase history. 
    - Event Manager - has ability to create, update and delete events.
- Localization. Supported languages: English, Russian, Belarussian.
- 
### Testing
The solution contains unit and integration tests. To run integration tests you need to specify connection string in the appsettings.json file in the TicketManagement.IntegrationTests folder.

