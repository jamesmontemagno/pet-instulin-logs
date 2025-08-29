using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class TimeService : ITimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
