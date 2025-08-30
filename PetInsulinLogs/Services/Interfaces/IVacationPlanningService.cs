using PetInsulinLogs.Models;

namespace PetInsulinLogs.Services.Interfaces;

public interface IVacationPlanningService
{
    /// <summary>
    /// Generates a vacation schedule that gradually shifts shot times to target AM/PM times
    /// </summary>
    /// <param name="pet">Pet with current schedule</param>
    /// <param name="targetAm">Target morning time</param>
    /// <param name="targetPm">Target evening time</param>
    /// <param name="stepMinutes">Minutes to shift per step (default 15)</param>
    /// <param name="startDate">When to start the plan</param>
    /// <returns>Generated vacation plan</returns>
    VacationPlan GeneratePlan(Pet pet, TimeSpan targetAm, TimeSpan targetPm, int stepMinutes = 15, DateTime? startDate = null);

    /// <summary>
    /// Gets the next recommended shot time for an active vacation plan
    /// </summary>
    /// <param name="plan">Active vacation plan</param>
    /// <param name="currentTime">Current time</param>
    /// <returns>Next recommended shot time, or null if plan is complete</returns>
    DateTime? GetNextVacationTime(VacationPlan plan, DateTime currentTime);

    /// <summary>
    /// Calculates the current schedule for a pet without vacation modifications
    /// </summary>
    /// <param name="pet">Pet to calculate schedule for</param>
    /// <param name="lastLogEntry">Last insulin shot log</param>
    /// <returns>Next scheduled shot times (AM/PM)</returns>
    (TimeSpan nextAm, TimeSpan nextPm) CalculateCurrentSchedule(Pet pet, LogEntry? lastLogEntry);
}