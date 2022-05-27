using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("areaId", item.AreaId),
                new SqlParameter("row", item.Row),
                new SqlParameter("number", item.Number),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            var id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }

        public void Delete(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("DeleteSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(new SqlParameter("seatId", id));

            connection.Open();

            command.ExecuteNonQuery();
        }

        public IEnumerable<Seat> GetAll()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("SELECT Id, AreaId, Row, Number FROM Seat", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand($"SELECT Id, AreaId, Row, Number FROM Seat WHERE Id = {id}", connection);

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
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("UpdateSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("seatId", item.Id),
                new SqlParameter("areaId", item.AreaId),
                new SqlParameter("row", item.Row),
                new SqlParameter("number", item.Number),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
