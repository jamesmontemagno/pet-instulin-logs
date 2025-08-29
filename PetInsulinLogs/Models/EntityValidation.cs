namespace PetInsulinLogs.Models;

public static class EntityValidation
{
    public static void EnsureValid(this Pet pet)
    {
        if (string.IsNullOrWhiteSpace(pet.PetId)) throw new ArgumentException("PetId required");
        if (string.IsNullOrWhiteSpace(pet.Name)) throw new ArgumentException("Name required");
        if (pet.DefaultUnits <= 0) throw new ArgumentException("DefaultUnits must be > 0");
        if (pet.IntervalHours <= 0) throw new ArgumentException("IntervalHours must be > 0");
    }

    public static void EnsureValid(this LogEntry log)
    {
        if (log.Units <= 0) throw new ArgumentException("Units must be > 0");
        if (log.TimestampUtc > DateTime.UtcNow.AddHours(2)) throw new ArgumentException("Timestamp in far future");
    }
}
