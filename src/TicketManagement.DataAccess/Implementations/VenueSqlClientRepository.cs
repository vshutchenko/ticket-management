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
    internal class VenueSqlClientRepository : IRepository<Venue>
    {
        private readonly string _connectionString;

        public VenueSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(Venue item)
        {
            var query = "INSERT INTO Venue(Description, Address, Phone) VALUES(@description, @address, @phone); SELECT SCOPE_IDENTITY()";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@address", item.Address);
            command.Parameters.AddWithValue("@phone", item.Phone);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Venue WHERE Id = @venueId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@venueId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<Venue> GetAll()
        {
            var query = "SELECT Id, Description, Address, Phone FROM Venue";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            var venues = new List<Venue>();

            while (reader.Read())
            {
                venues.Add(new Venue
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    Address = reader.GetString("Address"),
                    Phone = reader.GetString("Phone"),
                });
            }

            return venues.AsQueryable();
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            var query = "SELECT Id, Description, Address, Phone FROM Venue WHERE Id = @id";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("id", id);

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Venue
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    Address = reader.GetString("Address"),
                    Phone = reader.GetString("Phone"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(Venue item)
        {
            var query = "UPDATE Venue SET Description = @description, Address = @address, Phone = @phone WHERE Id = @venueId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@venueId", item.Id);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@address", item.Address);
            command.Parameters.AddWithValue("@phone", item.Phone);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
