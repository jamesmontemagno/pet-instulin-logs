using SQLite;

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
}
