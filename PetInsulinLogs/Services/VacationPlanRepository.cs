using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class VacationPlanRepository : IVacationPlanRepository
{
    private readonly ISqliteConnectionProvider provider;
    public VacationPlanRepository(ISqliteConnectionProvider provider) => this.provider = provider;

    public async Task<VacationPlan?> GetAsync(string petId)
    {
        var db = await provider.GetConnectionAsync();
        return await db.FindAsync<VacationPlan>(petId);
    }

    public async Task SetAsync(VacationPlan plan)
    {
        var db = await provider.GetConnectionAsync();
        await db.InsertOrReplaceAsync(plan);
    }

    public async Task DeleteAsync(string petId)
    {
        var db = await provider.GetConnectionAsync();
        await db.DeleteAsync<VacationPlan>(petId);
    }
}
