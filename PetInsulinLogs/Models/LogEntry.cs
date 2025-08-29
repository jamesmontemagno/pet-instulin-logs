using SQLite;

namespace PetInsulinLogs.Models;

[Table("LogEntries")]
public class LogEntry
{
    [PrimaryKey, AutoIncrement]
    public long LogId { get; set; }
    public string PetId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public double Units { get; set; }
    public InjectionSite InjectionSite { get; set; }
    public bool FoodGiven { get; set; }
    public string? FoodType { get; set; }
    public string? FoodAmount { get; set; }
    public string? Notes { get; set; }
    public string? BgNotes { get; set; }
    public OnTimeFlag OnTimeFlag { get; set; }
    public int DeviationMinutes { get; set; }
}
