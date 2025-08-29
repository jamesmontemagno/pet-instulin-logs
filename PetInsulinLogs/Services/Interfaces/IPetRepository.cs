using PetInsulinLogs.Models;

namespace PetInsulinLogs.Services.Interfaces;

public interface IPetRepository
{
    Task<Pet?> GetAsync(string petId);
    Task<IReadOnlyList<Pet>> GetByUserAsync(string userId);
    Task UpsertAsync(Pet pet);
    Task DeleteAsync(string petId);

    Task<IReadOnlyList<Membership>> GetMembershipsAsync(string userId);
    Task AddMembershipAsync(Membership m);
    Task RemoveMembershipAsync(string userId, string petId);

    Task<string> GenerateShareTokenAsync(string petId, TimeSpan? ttl = null);
    Task<Pet?> AcceptShareTokenAsync(string token, string caretakerUserId);
}
