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

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);

            DacPackage dacpac = DacPackage.Load(Path.Combine("TestDatabase", "TicketManagement.Database.dacpac"));
            DacServices dacpacService = new DacServices(_connectionString);
            DacDeployOptions dacOptions = new DacDeployOptions { CreateNewDatabase = true };
            dacpacService.Deploy(dacpac, builder["Initial Catalog"].ToString(), true, dacOptions);
        }

        public string ConnectionString => _connectionString;
    }
}
