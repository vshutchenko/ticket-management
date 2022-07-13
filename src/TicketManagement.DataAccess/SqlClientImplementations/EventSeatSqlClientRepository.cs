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
    internal class EventSeatSqlClientRepository : IRepository<EventSeat>
    {
        private readonly string _connectionString;

        public EventSeatSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(EventSeat item)
        {
            var query = "INSERT INTO EventSeat(EventAreaId, Row, Number, State) VALUES(@eventAreaId, @row, @number, @state); SELECT SCOPE_IDENTITY()";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("eventAreaId", item.EventAreaId);
            command.Parameters.AddWithValue("row", item.Row);
            command.Parameters.AddWithValue("number", item.Number);
            command.Parameters.AddWithValue("state", item.State);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM EventSeat WHERE Id = @eventSeatId";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("eventSeatId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<EventSeat> GetAll()
        {
            var query = "SELECT Id, EventAreaId, Row, Number, State FROM EventSeat";

            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            var eventSeats = new List<EventSeat>();

            while (reader.Read())
            {
                eventSeats.Add(new EventSeat
                {
                    Id = reader.GetInt32("Id"),
                    EventAreaId = reader.GetInt32("EventAreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                    State = (EventSeatState)reader.GetInt32("State"),
                });
            }

            return eventSeats.AsQueryable();
        }

        public async Task<EventSeat> GetByIdAsync(int id)
        {
            var query = "SELECT Id, EventAreaId, Row, Number, State FROM EventSeat WHERE Id = @id";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new EventSeat
                {
                    Id = reader.GetInt32("Id"),
                    EventAreaId = reader.GetInt32("EventAreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                    State = (EventSeatState)reader.GetInt32("State"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(EventSeat item)
        {
            var query = "UPDATE EventSeat SET EventAreaId = @eventAreaId, Row = @row, Number = @number, State = @state WHERE Id = @eventSeatId";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventSeatId", item.Id);
            command.Parameters.AddWithValue("@eventAreaId", item.EventAreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);
            command.Parameters.AddWithValue("@state", item.State);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
