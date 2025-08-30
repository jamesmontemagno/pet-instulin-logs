using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class ScheduleEngineService : IScheduleEngineService
{
    private readonly ITimeService timeService;
    private const double OnTimeToleranceMinutes = 15.0;
    private const double DefaultMinimumHours = 11.0;

    public ScheduleEngineService(ITimeService timeService)
    {
        this.timeService = timeService;
    }

    public DateTime CalculateNextDueTime(Pet pet, LogEntry? lastLogEntry, VacationPlan? vacationPlan = null)
    {
        // If no previous shots, suggest the current time as starting point
        if (lastLogEntry == null)
        {
            return timeService.UtcNow;
        }

        // Check if vacation plan is active and has schedule overrides
        if (vacationPlan?.Active == true && vacationPlan.GeneratedSchedule?.Count > 0)
        {
            var now = timeService.UtcNow;
            var nextVacationShot = vacationPlan.GeneratedSchedule
                .Where(entry => entry.DateTime > now)
                .OrderBy(entry => entry.DateTime)
                .FirstOrDefault();

            if (nextVacationShot != null)
            {
                return nextVacationShot.DateTime;
            }
        }

        // Standard calculation: last shot + interval hours
        var intervalHours = pet.IntervalHours > 0 ? pet.IntervalHours : 12.0;
        return lastLogEntry.TimestampUtc.AddHours(intervalHours);
    }

    public ScheduleEvaluation EvaluateShot(Pet pet, DateTime proposedTime, LogEntry? lastLogEntry, VacationPlan? vacationPlan = null)
    {
        var expectedTime = CalculateNextDueTime(pet, lastLogEntry, vacationPlan);
        var deviationMinutes = (int)(proposedTime - expectedTime).TotalMinutes;
        var absoluteDeviation = Math.Abs(deviationMinutes);

        var evaluation = new ScheduleEvaluation
        {
            ExpectedTime = expectedTime,
            DeviationMinutes = deviationMinutes,
            IsSafeInterval = IsSafeInterval(proposedTime, lastLogEntry)
        };

        // Determine timing flag
        if (absoluteDeviation <= OnTimeToleranceMinutes)
        {
            evaluation.OnTimeFlag = OnTimeFlag.OnTime;
        }
        else if (deviationMinutes < 0)
        {
            evaluation.OnTimeFlag = OnTimeFlag.Early;
            evaluation.WarningMessage = $"Shot is {absoluteDeviation} minutes early";
        }
        else
        {
            evaluation.OnTimeFlag = OnTimeFlag.Late;
            evaluation.WarningMessage = $"Shot is {absoluteDeviation} minutes late";
        }

        // Add safety warnings
        if (!evaluation.IsSafeInterval)
        {
            var timeSinceLastShot = lastLogEntry != null 
                ? (proposedTime - lastLogEntry.TimestampUtc).TotalHours 
                : 0;
            evaluation.WarningMessage = $"Warning: Only {timeSinceLastShot:F1} hours since last shot. Minimum recommended: {DefaultMinimumHours} hours.";
        }

        return evaluation;
    }

    public bool IsSafeInterval(DateTime proposedTime, LogEntry? lastLogEntry, double minimumHours = DefaultMinimumHours)
    {
        if (lastLogEntry == null)
        {
            return true; // No previous shot, so it's safe
        }

        var hoursSinceLastShot = (proposedTime - lastLogEntry.TimestampUtc).TotalHours;
        return hoursSinceLastShot >= minimumHours;
    }
}