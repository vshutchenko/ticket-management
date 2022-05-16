using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class EventSqlClientRepository : IRepository<Event>
    {
        private readonly string _connectionString;

        public EventSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public int Create(Event @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertEvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            int newId = -1;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventId", newId),
                new SqlParameter("name", @event.Name),
                new SqlParameter("description", @event.Descpription),
                new SqlParameter("layoutId", @event.LayoutId),
                new SqlParameter("startDate", @event.StartDate),
                new SqlParameter("endDate", @event.EndDate),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();

            return newId;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteEvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("eventId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Event> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, Name, Description, LayoutId, StartDate, EndDate FROM Event");

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new Event
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Descpription = reader.GetString("Description"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    StartDate = reader.GetDateTime("StartDate"),
                    EndDate = reader.GetDateTime("EndDate"),
                };
            }
        }

        public Event GetById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SelectIvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("eventId", id));

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return new Event
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Descpription = reader.GetString("Description"),
                LayoutId = reader.GetInt32("LayoutId"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
            };
        }

        public void Update(Event @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateEvent", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("eventId", @event.Id),
                new SqlParameter("name", @event.Name),
                new SqlParameter("description", @event.Descpription),
                new SqlParameter("layoutId", @event.LayoutId),
                new SqlParameter("startDate", @event.StartDate),
                new SqlParameter("endDate", @event.EndDate),
                new SqlParameter("endDate", @event.EndDate),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
