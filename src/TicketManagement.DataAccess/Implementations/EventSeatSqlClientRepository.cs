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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("EventSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventAreaId", item.EventAreaId),
                new SqlParameter("row", item.Row),
                new SqlParameter("number", item.Number),
                new SqlParameter("state", item.State),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteEventSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("eventSeatId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<EventSeat> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, EventAreaId, Row, Number, State FROM EventSeat", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, EventAreaId, Row, Number, State FROM EventSeat WHERE Id = {id}", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateEventSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventSeatId", item.Id),
                new SqlParameter("eventAreaId", item.EventAreaId),
                new SqlParameter("row", item.Row),
                new SqlParameter("number", item.Number),
                new SqlParameter("state", item.State),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
