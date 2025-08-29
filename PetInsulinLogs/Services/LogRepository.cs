using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class LogRepository : ILogRepository
{
    private readonly ISqliteConnectionProvider provider;
    public LogRepository(ISqliteConnectionProvider provider) => this.provider = provider;

    public async Task AppendAsync(LogEntry entry)
    {
        entry.EnsureValid();
        var db = await provider.GetConnectionAsync();
        await db.InsertAsync(entry);
    }

    public async Task<IReadOnlyList<LogEntry>> GetByPetAsync(string petId, DateTime? startUtc = null, DateTime? endUtc = null)
    {
        var db = await provider.GetConnectionAsync();
        var q = db.Table<LogEntry>().Where(l => l.PetId == petId);
        if (startUtc is DateTime s) q = q.Where(l => l.TimestampUtc >= s);
        if (endUtc is DateTime e) q = q.Where(l => l.TimestampUtc <= e);
        var res = await q.OrderByDescending(l => l.TimestampUtc).ToListAsync();
        return res;
    }
}
