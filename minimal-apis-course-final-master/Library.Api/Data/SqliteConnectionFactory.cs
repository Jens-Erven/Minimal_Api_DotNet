﻿using Microsoft.Data.Sqlite;
using System.Data;

namespace Library.Api.Data
{
    public class SqliteConnectionFactory : IDbConnnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;

        }
    }
}
