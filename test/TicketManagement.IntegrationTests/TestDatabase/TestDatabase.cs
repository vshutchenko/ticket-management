using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;

namespace TicketManagement.IntegrationTests
{
    public class TestDatabase
    {
        private readonly string _connectionString;

        public TestDatabase()
        {
            _connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("TestDatabase");

            var builder = new SqlConnectionStringBuilder(_connectionString);

            var dacpac = DacPackage.Load(Path.Combine("TestDatabase", "TicketManagement.Database.dacpac"));
            var dacpacService = new DacServices(_connectionString);
            var dacOptions = new DacDeployOptions { CreateNewDatabase = true };
            dacpacService.Deploy(dacpac, builder["Initial Catalog"].ToString(), true, dacOptions);
        }

        public string ConnectionString => _connectionString;
    }
}
