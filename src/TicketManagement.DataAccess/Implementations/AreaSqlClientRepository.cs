using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class AreaSqlClientRepository : IRepository<Area>
    {
        private readonly string _connectionString;

        public AreaSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public int Create(Area item)
        {
            var query = "INSERT INTO Area(LayoutId, Description, CoordX, CoordY) VALUES(@layoutId, @description, @coordX, @coordY); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", item.LayoutId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM Area WHERE Id = @areaId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Area> GetAll()
        {
            var query = "SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new Area
                {
                    Id = reader.GetInt32("Id"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                };
            }
        }

        public Area GetById(int id)
        {
            var query = "SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Area
                {
                    Id = reader.GetInt32("Id"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                };
            }

            return null;
        }

        public void Update(Area item)
        {
            var query = "UPDATE Area SET LayoutId = @layoutId, Description = @description, CoordX = @coordX, CoordY = @coordY WHERE Id = @areaId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", item.Id);
            command.Parameters.AddWithValue("@layoutId", item.LayoutId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
