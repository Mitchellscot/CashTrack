using CashTrack.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace CashTrack.Tests.Services.Common
{
    public class AppDbContextFactory : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        public AppDbContextFactory()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();

        }
        public AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
            var DbContext = new AppDbContext(options, new TestWebHostEnvironment());
            DbContext.Database.EnsureCreated();
            return DbContext;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
