using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public int Create(Venue item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertVenue", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("description", item.Description),
                new SqlParameter("address", item.Address),
                new SqlParameter("phone", item.Phone),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteVenue", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("venueId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Venue> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, Description, Address, Phone FROM Venue", connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new Venue
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    Address = reader.GetString("Address"),
                    Phone = reader.GetString("Phone"),
                };
            }
        }

        public Venue GetById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, Description, Address, Phone FROM Venue WHERE Id = {id}", connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
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

        public void Update(Venue item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateVenue", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("venueId", item.Id),
                new SqlParameter("description", item.Description),
                new SqlParameter("address", item.Address),
                new SqlParameter("phone", item.Phone),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
