namespace TicketManagement.UserApi.Data
{
    public interface IContextSeeder
    {
        Task SeedInitialDataAsync();
    }
}
