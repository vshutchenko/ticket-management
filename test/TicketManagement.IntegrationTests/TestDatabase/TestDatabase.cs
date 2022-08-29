using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;

namespace TicketManagement.IntegrationTests.TestDatabase
{
    public class TestDatabaseInfo
    {
        private readonly string _connectionString;

        public TestDatabaseInfo()
        {
            _connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("TestDatabase");
        }

        public string ConnectionString => _connectionString;

        public void CreateDb()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);

            var dacpac = DacPackage.Load(Path.Combine("TestDatabase", "TicketManagement.Database.dacpac"));
            var dacpacService = new DacServices(_connectionString);
            var dacOptions = new DacDeployOptions { CreateNewDatabase = true };
            dacpacService.Deploy(dacpac, builder["Initial Catalog"].ToString(), true, dacOptions);
        }
    }
}
