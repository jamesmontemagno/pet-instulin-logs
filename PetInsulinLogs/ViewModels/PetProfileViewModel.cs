using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.ViewModels;

public partial class PetProfileViewModel : ObservableObject
{
    private readonly IPetRepository pets;
    private readonly IIdService ids;

    [ObservableProperty]
    private Pet pet = new();

    [ObservableProperty]
    private bool isReadOnly;

    public string CurrentUserId { get; set; } = "owner-local"; // TODO auth

    public PetProfileViewModel(IPetRepository pets, IIdService ids)
    {
        this.pets = pets;
        this.ids = ids;
        Pet = new Pet { PetId = ids.NewId(), OwnerId = CurrentUserId, IntervalHours = 12, DefaultUnits = 1 };
    }

    public void LoadPet(Pet existing, bool readOnly)
    {
        Pet = existing;
        IsReadOnly = readOnly;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsReadOnly) return;
        if (string.IsNullOrWhiteSpace(Pet.Name)) throw new InvalidOperationException("Name required");
        if (Pet.DefaultUnits <= 0) throw new InvalidOperationException("Units must be > 0");
        await pets.UpsertAsync(Pet);
    }

    [RelayCommand]
    public async Task<string> GenerateShareCodeAsync()
    {
        return await pets.GenerateShareTokenAsync(Pet.PetId, TimeSpan.FromDays(7));
    }
}
