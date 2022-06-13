﻿using System;
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
            var query = "INSERT INTO Layout(Description, VenueId) VALUES(@description, @venueId); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@venueId", item.VenueId);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM Layout WHERE Id = @layoutId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Layout> GetAll()
        {
            var query = "SELECT Id, Description, VenueId FROM Layout";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

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
            var query = "SELECT Id, Description, VenueId FROM Layout WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Layout
                {
                    Id = reader.GetInt32("Id"),
                    Description = reader.GetString("Description"),
                    VenueId = reader.GetInt32("VenueId"),
                };
            }

            return null;
        }

        public void Update(Layout item)
        {
            var query = "UPDATE Layout SET Description = @description, VenueId = @venueId WHERE Id = @layoutId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", item.Id);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@venueId", item.VenueId);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}