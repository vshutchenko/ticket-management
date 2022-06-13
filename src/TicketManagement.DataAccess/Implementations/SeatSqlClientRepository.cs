using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class SeatSqlClientRepository : IRepository<Seat>
    {
        private readonly string _connectionString;

        public SeatSqlClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> CreateAsync(Seat item)
        {
            var query = "INSERT INTO Seat(AreaId, Row, Number) VALUES(@areaId, @row, @number); SELECT SCOPE_IDENTITY()";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", item.AreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);

            await connection.OpenAsync();

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Seat WHERE Id = @seatId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@seatId", id);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public IQueryable<Seat> GetAll()
        {
            var query = "SELECT Id, AreaId, Row, Number FROM Seat";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            var seats = new List<Seat>();

            while (reader.Read())
            {
                seats.Add(new Seat
                {
                    Id = reader.GetInt32("Id"),
                    AreaId = reader.GetInt32("AreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                });
            }

            return seats.AsQueryable();
        }

        public async Task<Seat> GetByIdAsync(int id)
        {
            var query = "SELECT Id, AreaId, Row, Number FROM Seat WHERE Id = @id";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Seat
                {
                    Id = reader.GetInt32("Id"),
                    AreaId = reader.GetInt32("AreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                };
            }

            return null;
        }

        public async Task UpdateAsync(Seat item)
        {
            var query = "UPDATE Seat SET AreaId = @areaId, Row = @row, Number = @number WHERE Id = @seatId";

            await using SqlConnection connection = new SqlConnection(_connectionString);

            await using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@seatId", item.Id);
            command.Parameters.AddWithValue("@areaId", item.AreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
