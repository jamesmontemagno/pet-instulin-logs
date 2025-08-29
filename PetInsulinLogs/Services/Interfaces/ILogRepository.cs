using PetInsulinLogs.Models;

namespace PetInsulinLogs.Services.Interfaces;

public interface ILogRepository
{
    Task AppendAsync(LogEntry entry);
    Task<IReadOnlyList<LogEntry>> GetByPetAsync(string petId, DateTime? startUtc = null, DateTime? endUtc = null);
}
