using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class VacationPlanningService : IVacationPlanningService
{
    private readonly ITimeService timeService;

    public VacationPlanningService(ITimeService timeService)
    {
        this.timeService = timeService;
    }

    public VacationPlan GeneratePlan(Pet pet, TimeSpan targetAm, TimeSpan targetPm, int stepMinutes = 15, DateTime? startDate = null)
    {
        var start = startDate ?? timeService.UtcNow.Date;
        var schedule = new List<VacationScheduleEntry>();

        // Calculate current typical schedule (assuming 12-hour intervals)
        var intervalHours = pet.IntervalHours > 0 ? pet.IntervalHours : 12.0;
        
        // For simplicity, assume current schedule is 8 AM and 8 PM
        var currentAm = TimeSpan.FromHours(8);
        var currentPm = TimeSpan.FromHours(20);

        // Generate shifting schedule
        var currentDate = start;
        var workingAm = currentAm;
        var workingPm = currentPm;

        // Continue until we reach target times
        while (workingAm != targetAm || workingPm != targetPm)
        {
            // Shift AM time
            if (workingAm != targetAm)
            {
                var amDifference = (int)(targetAm.TotalMinutes - workingAm.TotalMinutes);
                var amStep = Math.Sign(amDifference) * Math.Min(Math.Abs(amDifference), stepMinutes);
                workingAm = workingAm.Add(TimeSpan.FromMinutes(amStep));

                schedule.Add(new VacationScheduleEntry
                {
                    DateTime = currentDate.Add(workingAm),
                    Note = $"AM shot - shifting toward {targetAm:hh\\:mm}"
                });
            }

            // Shift PM time (12 hours after AM)
            if (workingPm != targetPm)
            {
                var pmDifference = (int)(targetPm.TotalMinutes - workingPm.TotalMinutes);
                var pmStep = Math.Sign(pmDifference) * Math.Min(Math.Abs(pmDifference), stepMinutes);
                workingPm = workingPm.Add(TimeSpan.FromMinutes(pmStep));

                schedule.Add(new VacationScheduleEntry
                {
                    DateTime = currentDate.Add(workingPm),
                    Note = $"PM shot - shifting toward {targetPm:hh\\:mm}"
                });
            }

            currentDate = currentDate.AddDays(1);

            // Safety check to prevent infinite loops
            if (schedule.Count > 100)
            {
                break;
            }
        }

        // Add a few days at target schedule
        for (int i = 0; i < 7; i++)
        {
            schedule.Add(new VacationScheduleEntry
            {
                DateTime = currentDate.Add(targetAm),
                Note = "AM shot - at target time"
            });

            schedule.Add(new VacationScheduleEntry
            {
                DateTime = currentDate.Add(targetPm),
                Note = "PM shot - at target time"
            });

            currentDate = currentDate.AddDays(1);
        }

        return new VacationPlan
        {
            PetId = pet.PetId,
            TargetAm = targetAm,
            TargetPm = targetPm,
            StepMinutes = stepMinutes,
            Active = false, // Activate manually
            GeneratedSchedule = schedule
        };
    }

    public DateTime? GetNextVacationTime(VacationPlan plan, DateTime currentTime)
    {
        if (!plan.Active || plan.GeneratedSchedule == null)
        {
            return null;
        }

        var nextEntry = plan.GeneratedSchedule
            .Where(entry => entry.DateTime > currentTime)
            .OrderBy(entry => entry.DateTime)
            .FirstOrDefault();

        return nextEntry?.DateTime;
    }

    public (TimeSpan nextAm, TimeSpan nextPm) CalculateCurrentSchedule(Pet pet, LogEntry? lastLogEntry)
    {
        // Simple calculation - assume typical 8 AM / 8 PM schedule
        // In a real app, this would be more sophisticated based on actual log history
        var intervalHours = pet.IntervalHours > 0 ? pet.IntervalHours : 12.0;

        if (lastLogEntry != null)
        {
            var lastTime = lastLogEntry.TimestampUtc.TimeOfDay;
            var nextTime = lastTime.Add(TimeSpan.FromHours(intervalHours));
            
            // Normalize to reasonable AM/PM times
            if (nextTime.Hours < 12)
            {
                return (nextTime, nextTime.Add(TimeSpan.FromHours(intervalHours)));
            }
            else
            {
                return (nextTime.Subtract(TimeSpan.FromHours(intervalHours)), nextTime);
            }
        }

        // Default schedule
        return (TimeSpan.FromHours(8), TimeSpan.FromHours(20));
    }
}