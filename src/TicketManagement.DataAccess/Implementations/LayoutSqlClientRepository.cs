using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class LayoutSqlClientRepository : IRepository<Layout>
    {
        private readonly string _connectionString;

        public LayoutSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(Layout item)
        {
            var query = "INSERT INTO Layout(Description, VenueId) VALUES(@description, @venueId); SELECT SCOPE_IDENTITY()";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@venueId", item.VenueId);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Layout WHERE Id = @layoutId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<Layout> GetAll()
        {
            var query = "SELECT Id, Description, VenueId FROM Layout";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            var layouts = new List<Layout>();

            while (reader.Read())
            {
                layouts.Add(new Layout
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    VenueId = reader.GetInt32("VenueId"),
                });
            }

            return layouts.AsQueryable();
        }

        public async Task<Layout> GetByIdAsync(int id)
        {
            var query = "SELECT Id, Description, VenueId FROM Layout WHERE Id = @id";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using SqlDataReader reader = command.ExecuteReader();

            if (await reader.ReadAsync())
            {
                return new Layout
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    VenueId = reader.GetInt32("VenueId"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(Layout item)
        {
            var query = "UPDATE Layout SET Description = @description, VenueId = @venueId WHERE Id = @layoutId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", item.Id);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@venueId", item.VenueId);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
