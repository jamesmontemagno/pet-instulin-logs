using PetInsulinLogs.Models;

namespace PetInsulinLogs.Services.Interfaces;

public interface IVacationPlanRepository
{
    Task<VacationPlan?> GetAsync(string petId);
    Task SetAsync(VacationPlan plan);
    Task DeleteAsync(string petId);
}
