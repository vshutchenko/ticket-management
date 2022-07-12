using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.SqlClientImplementations
{
    internal class AreaSqlClientRepository : IRepository<Area>
    {
        private readonly string _connectionString;

        public AreaSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(Area item)
        {
            var query = "INSERT INTO Area(LayoutId, Description, CoordX, CoordY) VALUES(@layoutId, @description, @coordX, @coordY); SELECT SCOPE_IDENTITY()";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@layoutId", item.LayoutId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Area WHERE Id = @areaId";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<Area> GetAll()
        {
            var query = "SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area";

            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            var areas = new List<Area>();

            while (reader.Read())
            {
                areas.Add(new Area
                {
                    Id = reader.GetInt32("Id"),
                    LayoutId = reader.GetInt32("LayoutId"),
                    Description = reader.GetString("Description"),
                    CoordX = reader.GetInt32("CoordX"),
                    CoordY = reader.GetInt32("CoordY"),
                });
            }

            return areas.AsQueryable();
        }

        public async Task<Area> GetByIdAsync(int id)
        {
            var query = "SELECT Id, LayoutId, Description, CoordX, CoordY FROM Area WHERE Id = @id";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
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

        public async Task UpdateAsync(Area item)
        {
            var query = "UPDATE Area SET LayoutId = @layoutId, Description = @description, CoordX = @coordX, CoordY = @coordY WHERE Id = @areaId";

            await using var connection = new SqlConnection(_connectionString);

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", item.Id);
            command.Parameters.AddWithValue("@layoutId", item.LayoutId);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@coordX", item.CoordX);
            command.Parameters.AddWithValue("@coordY", item.CoordY);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
