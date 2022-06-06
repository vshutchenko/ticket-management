using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public int Create(EventArea item)
        {
            var query = "INSERT INTO EventArea(EventId, Description, CoordX, CoordY, Price) VALUES(@eventId, @description, @coordX, @coordY, @price); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventId", item.EventId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);
            command.Parameters.AddWithValue("@price", item.Price);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM EventArea WHERE Id = @eventAreaId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventAreaId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<EventArea> GetAll()
        {
            var query = "SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new EventArea
                {
                    Id = reader.GetInt32("Id"),
                    EventId = reader.GetInt32("EventId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                    Price = reader.GetDecimal("Price"),
                };
            }
        }

        public EventArea GetById(int id)
        {
            var query = "SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
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

        public void Update(EventArea item)
        {
            var query = "UPDATE EventArea SET EventId = @eventId, Description = @description, CoordX = @coordX, CoordY = @coordY, Price = @price  WHERE Id = @eventAreaId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@eventAreaId", item.Id);
            command.Parameters.AddWithValue("@eventId", item.EventId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);
            command.Parameters.AddWithValue("@price", item.Price);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
