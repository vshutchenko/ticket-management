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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertEventArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventId", item.EventId),
                new SqlParameter("description", item.Description),
                new SqlParameter("coordX", item.CoordX),
                new SqlParameter("coordY", item.CoordY),
                new SqlParameter("price", item.Price),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteEventArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("eventAreaId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<EventArea> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, EventId, Description, CoordX, CoordY, Price FROM EventArea WHERE Id = {id}", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateEventArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventAreaId", item.Id),
                new SqlParameter("eventId", item.EventId),
                new SqlParameter("description", item.Description),
                new SqlParameter("coordX", item.CoordX),
                new SqlParameter("coordY", item.CoordY),
                new SqlParameter("price", item.Price),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
