﻿using Dapper;

namespace Library.Api.Data
{
    public class DatabaseInitializer
    {

        private readonly IDbConnnectionFactory _connectionFactory;
        public DatabaseInitializer(IDbConnnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task InitializeAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync(
             @"
                    CREATE TABLE IF NOT EXISTS Books (
                        Isbn TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        ShortDescription TEXT NOT NULL,
                        PageCount INTEGER,
                        ReleaseDate TEXT NOT NULL

                    )"
            );
        }
    }
}
