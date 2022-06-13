using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public int Create(Seat item)
        {
            var query = "INSERT INTO Seat(AreaId, Row, Number) VALUES(@areaId, @row, @number); SELECT SCOPE_IDENTITY()";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@areaId", item.AreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            var query = "DELETE FROM Seat WHERE Id = @seatId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@seatId", id);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Seat> GetAll()
        {
            var query = "SELECT Id, AreaId, Row, Number FROM Seat";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new Seat
                {
                    Id = reader.GetInt32("Id"),
                    AreaId = reader.GetInt32("AreaId"),
                    Row = reader.GetInt32("Row"),
                    Number = reader.GetInt32("Number"),
                };
            }
        }

        public Seat GetById(int id)
        {
            var query = "SELECT Id, AreaId, Row, Number FROM Seat WHERE Id = @id";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
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

        public void Update(Seat item)
        {
            var query = "UPDATE Seat SET AreaId = @areaId, Row = @row, Number = @number WHERE Id = @seatId";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@seatId", item.Id);
            command.Parameters.AddWithValue("@areaId", item.AreaId);
            command.Parameters.AddWithValue("@row", item.Row);
            command.Parameters.AddWithValue("@number", item.Number);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
