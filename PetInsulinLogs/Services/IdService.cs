using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class IdService : IIdService
{
    public string NewId() => Guid.NewGuid().ToString("N");
}
