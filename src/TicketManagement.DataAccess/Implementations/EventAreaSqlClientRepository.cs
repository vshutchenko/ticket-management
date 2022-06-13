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
    internal class EventAreaSqlClientRepository : IRepository<EventArea>
    {
        private readonly string _connectionString;

        public EventAreaSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(EventArea item)
        {
            var query = "INSERT INTO EventArea(EventId, Description, CoordX, CoordY, Price) VALUES(@eventId, @description, @coordX, @coordY, @price); SELECT SCOPE_IDENTITY()";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventId", item.EventId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);
            command.Parameters.AddWithValue("@price", item.Price);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM EventArea WHERE Id = @eventAreaId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventAreaId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<EventArea> GetAll()
        {
            var query = "SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            var eventAreas = new List<EventArea>();

            while (reader.Read())
            {
                eventAreas.Add(new EventArea
                {
                    Id = reader.GetInt32("Id"),
                    EventId = reader.GetInt32("EventId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                    Price = reader.GetDecimal("Price"),
                });
            }

            return eventAreas.AsQueryable();
        }

        public async Task<EventArea> GetByIdAsync(int id)
        {
            var query = "SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea WHERE Id = @id";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new EventArea
                {
                    Id = reader.GetInt32("Id"),
                    EventId = reader.GetInt32("EventId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                    Price = reader.GetDecimal("Price"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(EventArea item)
        {
            var query = "UPDATE EventArea SET EventId = @eventId, Description = @description, CoordX = @coordX, CoordY = @coordY, Price = @price  WHERE Id = @eventAreaId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventAreaId", item.Id);
            command.Parameters.AddWithValue("@eventId", item.EventId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);
            command.Parameters.AddWithValue("@price", item.Price);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
