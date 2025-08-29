using PetInsulinLogs.Models;

namespace PetInsulinLogs.Services.Interfaces;

public interface IScheduleEngineService
{
    /// <summary>
    /// Calculates the next due time for an insulin shot based on the last log entry and pet schedule
    /// </summary>
    /// <param name="pet">Pet with schedule information</param>
    /// <param name="lastLogEntry">Last insulin log entry, or null if no previous shots</param>
    /// <param name="vacationPlan">Active vacation plan override, or null</param>
    /// <returns>Next due time in UTC</returns>
    DateTime CalculateNextDueTime(Pet pet, LogEntry? lastLogEntry, VacationPlan? vacationPlan = null);

    /// <summary>
    /// Evaluates timing of a proposed shot against the schedule
    /// </summary>
    /// <param name="pet">Pet with schedule information</param>
    /// <param name="proposedTime">Time of the proposed shot</param>
    /// <param name="lastLogEntry">Last insulin log entry, or null if no previous shots</param>
    /// <param name="vacationPlan">Active vacation plan override, or null</param>
    /// <returns>Timing evaluation result</returns>
    ScheduleEvaluation EvaluateShot(Pet pet, DateTime proposedTime, LogEntry? lastLogEntry, VacationPlan? vacationPlan = null);

    /// <summary>
    /// Checks if the minimum safe interval has elapsed since the last shot
    /// </summary>
    /// <param name="proposedTime">Time of the proposed shot</param>
    /// <param name="lastLogEntry">Last insulin log entry</param>
    /// <param name="minimumHours">Minimum hours between shots (default 11)</param>
    /// <returns>True if safe to proceed</returns>
    bool IsSafeInterval(DateTime proposedTime, LogEntry? lastLogEntry, double minimumHours = 11.0);
}

public class ScheduleEvaluation
{
    public OnTimeFlag OnTimeFlag { get; set; }
    public int DeviationMinutes { get; set; }
    public DateTime ExpectedTime { get; set; }
    public bool IsSafeInterval { get; set; }
    public string? WarningMessage { get; set; }
}