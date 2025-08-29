using SQLite;

namespace PetInsulinLogs.Services.Interfaces;

public interface ISqliteConnectionProvider
{
    Task<SQLiteAsyncConnection> GetConnectionAsync();
}
