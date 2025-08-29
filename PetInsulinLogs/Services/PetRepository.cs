using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class PetRepository : IPetRepository
{
    private readonly ISqliteConnectionProvider provider;
    private readonly IIdService idService;
    private readonly ITimeService time;

    public PetRepository(ISqliteConnectionProvider provider, IIdService idService, ITimeService time)
    {
        this.provider = provider;
        this.idService = idService;
        this.time = time;
    }

    public async Task<Pet?> GetAsync(string petId)
    {
        var db = await provider.GetConnectionAsync();
        return await db.FindAsync<Pet>(petId);
    }

    public async Task<IReadOnlyList<Pet>> GetByUserAsync(string userId)
    {
        var db = await provider.GetConnectionAsync();
        // Get pet ids from explicit memberships
        var memberships = await db.Table<Membership>().Where(m => m.UserId == userId).ToListAsync();
        var petIdSet = new HashSet<string>(memberships.Select(m => m.PetId));

        // Also include pets where the user is recorded as the Owner (covers legacy records without memberships)
        var ownerPets = await db.Table<Pet>().Where(p => p.OwnerId == userId).ToListAsync();
        foreach (var p in ownerPets)
            petIdSet.Add(p.PetId);

        if (petIdSet.Count == 0) return Array.Empty<Pet>();
        var petIds = petIdSet.ToList();
        var all = await db.Table<Pet>().Where(p => petIds.Contains(p.PetId)).ToListAsync();
        return all;
    }

    public async Task UpsertAsync(Pet pet)
    {
        pet.EnsureValid();
        var db = await provider.GetConnectionAsync();
        if (string.IsNullOrWhiteSpace(pet.PetId)) pet.PetId = idService.NewId();
        await db.InsertOrReplaceAsync(pet);

        // Ensure the pet owner is a member with Role.Owner
        if (!string.IsNullOrWhiteSpace(pet.OwnerId))
        {
            var existing = await db.Table<Membership>()
                .Where(m => m.PetId == pet.PetId && m.UserId == pet.OwnerId)
                .FirstOrDefaultAsync();
            if (existing == null)
            {
                await db.InsertAsync(new Membership { PetId = pet.PetId, UserId = pet.OwnerId, Role = Role.Owner });
            }
        }
    }

    public async Task DeleteAsync(string petId)
    {
        var db = await provider.GetConnectionAsync();
        await db.DeleteAsync<Pet>(petId);
        await db.ExecuteAsync("DELETE FROM Memberships WHERE PetId = ?", petId);
        await db.ExecuteAsync("DELETE FROM LogEntries WHERE PetId = ?", petId);
        await db.DeleteAsync<VacationPlan>(petId);
    }

    public async Task<IReadOnlyList<Membership>> GetMembershipsAsync(string userId)
    {
        var db = await provider.GetConnectionAsync();
        return await db.Table<Membership>().Where(m => m.UserId == userId).ToListAsync();
    }

    public async Task AddMembershipAsync(Membership m)
    {
        var db = await provider.GetConnectionAsync();
        await db.InsertAsync(m);
    }

    public async Task RemoveMembershipAsync(string userId, string petId)
    {
        var db = await provider.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM Memberships WHERE UserId = ? AND PetId = ?", userId, petId);
    }

    public async Task<string> GenerateShareTokenAsync(string petId, TimeSpan? ttl = null)
    {
        var db = await provider.GetConnectionAsync();
        var token = new ShareToken
        {
            Token = Guid.NewGuid().ToString("N"),
            PetId = petId,
            ExpiryUtc = time.UtcNow.Add(ttl ?? TimeSpan.FromDays(7))
        };
        await db.InsertAsync(token);
        return token.Token;
    }

    public async Task<Pet?> AcceptShareTokenAsync(string token, string caretakerUserId)
    {
        var db = await provider.GetConnectionAsync();
        var t = await db.FindAsync<ShareToken>(token);
        if (t == null || t.ExpiryUtc < time.UtcNow) return null;
        var pet = await db.FindAsync<Pet>(t.PetId);
        if (pet == null) return null;
        await db.InsertAsync(new Membership { PetId = t.PetId, UserId = caretakerUserId, Role = Role.Caretaker });
        await db.DeleteAsync(t);
        return pet;
    }
}
