start "VenueApi" cmd.exe /k dotnet run --project ./src/TicketManagement.VenueApi/TicketManagement.VenueApi.csproj
start "EventApi" cmd.exe /k dotnet run --configuration Debug --project ./src/TicketManagement.EventApi/TicketManagement.EventApi.csproj
start "UserApi" cmd.exe /k dotnet run --project ./src/TicketManagement.UserApi/TicketManagement.UserApi.csproj
start "PurchaseApi" cmd.exe /k dotnet run --project ./src/TicketManagement.PurchaseApi/TicketManagement.PurchaseApi.csproj
start "WebApplication" cmd.exe /k dotnet run --project ./src/TicketManagement.WebApplication/TicketManagement.WebApplication.csproj
start "UI" cmd.exe /k npm start --prefix ./src/TicketManagement.UI/ticketmanagement.ui