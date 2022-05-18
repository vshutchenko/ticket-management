using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Implementations
{
    internal class SeatSqlClientService : IRepository<Seat>
    {
        private readonly string _connectionString;

        public SeatSqlClientService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public int Create(Seat item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("InsertSeat", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            int newId = -1;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("seatId", newId),
                new SqlParameter("areaId", item.AreaId),
                new SqlParameter("row", item.Row),
                new SqlParameter("number", item.Number),
            };

            command.Parameters.AddRange(parameters);

            connection.Open();

            command.ExecuteNonQuery();

            return newId;
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

            return new Seat
            {
                Id = reader.GetInt32("Id"),
                AreaId = reader.GetInt32("AreaId"),
                Row = reader.GetInt32("Row"),
                Number = reader.GetInt32("Number"),
            };
        }

        public void Update(Seat item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

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
