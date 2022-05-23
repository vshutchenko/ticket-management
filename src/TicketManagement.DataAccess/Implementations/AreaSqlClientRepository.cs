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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("layoutId", item.LayoutId),
                new SqlParameter("description", item.Description),
                new SqlParameter("coordX", item.CoordX),
                new SqlParameter("coordY", item.CoordY),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("areaId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Area> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area WHERE Id = {id}", connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return new Area
            {
                Id = reader.GetInt32("Id"),
                LayoutId = reader.GetInt32("LayoutId"),
                Description = reader.GetString("Description"),
                CoordX = reader.GetInt32("CoordX"),
                CoordY = reader.GetInt32("CoordY"),
            };
        }

        public void Update(Area item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateArea", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("areaId", item.Id),
                new SqlParameter("layoutId", item.LayoutId),
                new SqlParameter("description", item.Description),
                new SqlParameter("coordX", item.CoordX),
                new SqlParameter("coordY", item.CoordY),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
