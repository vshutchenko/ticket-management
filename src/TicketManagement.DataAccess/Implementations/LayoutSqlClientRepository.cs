using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public int Create(Layout item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertLayout", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("description", item.Description),
                new SqlParameter("venueId", item.VenueId),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteLayout", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("layoutId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Layout> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, Description, VenueId FROM Layout", connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new Layout
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    VenueId = reader.GetInt32("VenueId"),
                };
            }
        }

        public Layout GetById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, Description, VenueId FROM Layout WHERE Id = {id}", connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return new Layout
            {
                Id = reader.GetInt32("Id"),
                Description = reader.GetString("Description"),
                VenueId = reader.GetInt32("VenueId"),
            };
        }

        public void Update(Layout item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateLayout", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("layoutId", item.Id),
                new SqlParameter("description", item.Description),
                new SqlParameter("venueId", item.VenueId),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
