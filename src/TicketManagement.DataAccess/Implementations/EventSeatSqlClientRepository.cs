using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class EventSeatSqlClientRepository : IRepository<EventSeat>
    {
        private readonly string _connectionString;

        public EventSeatSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public int Create(EventSeat item)
        {
            var query = "INSERT INTO EventSeat(EventAreaId, Row, Number, State) VALUES(@eventAreaId, @row, @number, @state); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("eventAreaId", item.EventAreaId);
            command.Parameters.AddWithValue("row", item.Row);
            command.Parameters.AddWithValue("number", item.Number);
            command.Parameters.AddWithValue("state", item.State);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM EventSeat WHERE Id = @eventSeatId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("eventSeatId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<EventSeat> GetAll()
        {
            var query = "SELECT Id, EventAreaId, Row, Number, State FROM EventSeat";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new EventSeat
                {
                    Id = reader.GetInt32("Id"),
                    EventAreaId = reader.GetInt32("EventAreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                    State = (EventSeatState)reader.GetInt32("State"),
                };
            }
        }

        public EventSeat GetById(int id)
        {
            var query = "SELECT Id, EventAreaId, Row, Number, State FROM EventSeat WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
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

        public void Update(EventSeat item)
        {
            var query = "UPDATE EventSeat SET EventAreaId = @eventAreaId, Row = @row, Number = @number, State = @state WHERE Id = @eventSeatId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventSeatId", item.Id);
            command.Parameters.AddWithValue("@eventAreaId", item.EventAreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);
            command.Parameters.AddWithValue("@state", item.State);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
