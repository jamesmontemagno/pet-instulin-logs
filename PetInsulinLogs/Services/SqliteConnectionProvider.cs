using SQLite;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class SqliteConnectionProvider : ISqliteConnectionProvider
{
    private readonly Lazy<Task<SQLiteAsyncConnection>> lazyConn;

    public SqliteConnectionProvider(string dbPath)
    {
        lazyConn = new Lazy<Task<SQLiteAsyncConnection>>(async () =>
        {
            var conn = new SQLiteAsyncConnection(dbPath);
            await conn.CreateTableAsync<Pet>();
            await conn.CreateTableAsync<Membership>();
            await conn.CreateTableAsync<LogEntry>();
            await conn.CreateTableAsync<VacationPlan>();
            await conn.CreateTableAsync<ShareToken>();
            return conn;
        });
    }

    public Task<SQLiteAsyncConnection> GetConnectionAsync() => lazyConn.Value;
}
