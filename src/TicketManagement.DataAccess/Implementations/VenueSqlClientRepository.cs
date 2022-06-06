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
            var query = "INSERT INTO Venue(Description, Address, Phone) VALUES(@description, @address, @phone); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@address", item.Address);
            command.Parameters.AddWithValue("@phone", item.Phone);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM Venue WHERE Id = @venueId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@venueId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Venue> GetAll()
        {
            var query = "SELECT Id, Description, Address, Phone FROM Venue";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

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
            var query = "SELECT Id, Description, Address, Phone FROM Venue WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("id", id);

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
            var query = "UPDATE Venue SET Description = @description, Address = @address, Phone = @phone WHERE Id = @venueId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@venueId", item.Id);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@address", item.Address);
            command.Parameters.AddWithValue("@phone", item.Phone);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
