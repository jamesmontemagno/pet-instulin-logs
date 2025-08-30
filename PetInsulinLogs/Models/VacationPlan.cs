using SQLite;
using System.Text.Json;

namespace PetInsulinLogs.Models;

[Table("VacationPlans")]
public class VacationPlan
{
    [PrimaryKey]
    public string PetId { get; set; } = string.Empty;
    public TimeSpan? TargetAm { get; set; }
    public TimeSpan? TargetPm { get; set; }
    public int StepMinutes { get; set; } = 15;
    public bool Active { get; set; }
    public string? GeneratedScheduleJson { get; set; }

    [Ignore]
    public List<VacationScheduleEntry>? GeneratedSchedule
    {
        get
        {
            if (string.IsNullOrEmpty(GeneratedScheduleJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<VacationScheduleEntry>>(GeneratedScheduleJson);
            }
            catch
            {
                return null;
            }
        }
        set
        {
            GeneratedScheduleJson = value != null ? JsonSerializer.Serialize(value) : null;
        }
    }
}
