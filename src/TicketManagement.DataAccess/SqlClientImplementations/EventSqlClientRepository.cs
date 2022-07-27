using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.SqlClientImplementations
{
    internal class EventSqlClientRepository : IRepository<Event>
    {
        private readonly string _connectionString;

        public EventSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(Event @event)
        {
            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand("InsertEvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("@eventId", SqlDbType.Int) { Direction = ParameterDirection.Output });
            command.Parameters.AddWithValue("@name", @event.Name);
            command.Parameters.AddWithValue("@description", @event.Description);
            command.Parameters.AddWithValue("@layoutId", @event.LayoutId);
            command.Parameters.AddWithValue("@startDate", @event.StartDate);
            command.Parameters.AddWithValue("@endDate", @event.EndDate);
            command.Parameters.AddWithValue("@imageUrl", @event.ImageUrl);
            command.Parameters.AddWithValue("@published", @event.Published);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return Convert.ToInt32(command.Parameters["@eventId"].Value);
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand("DeleteEvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@eventId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<Event> GetAll()
        {
            var query = "SELECT Id, Name, Description, LayoutId, StartDate, EndDate, ImageUrl, Published FROM Event";

            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            var events = new List<Event>();

            while (reader.Read())
            {
                events.Add(new Event
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    StartDate = reader.GetDateTime("StartDate"),
                    EndDate = reader.GetDateTime("EndDate"),
                    ImageUrl = reader.GetString("ImageUrl"),
                    Published = reader.GetBoolean("Published"),
                });
            }

            return events.AsQueryable();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            var query = "SELECT Id, Name, Description, LayoutId, StartDate, EndDate, ImageUrl, Published FROM Event WHERE Id = @id";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Event
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    StartDate = reader.GetDateTime("StartDate"),
                    EndDate = reader.GetDateTime("EndDate"),
                    ImageUrl = reader.GetString("ImageUrl"),
                    Published = reader.GetBoolean("Published"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(Event @event)
        {
            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand("UpdateEventWithAreas", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@eventId", @event.Id);
            command.Parameters.AddWithValue("@name", @event.Name);
            command.Parameters.AddWithValue("@description", @event.Description);
            command.Parameters.AddWithValue("@layoutId", @event.LayoutId);
            command.Parameters.AddWithValue("@startDate", @event.StartDate);
            command.Parameters.AddWithValue("@endDate", @event.EndDate);
            command.Parameters.AddWithValue("@imageUrl", @event.ImageUrl);
            command.Parameters.AddWithValue("@published", @event.Published);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
