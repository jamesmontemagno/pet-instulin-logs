using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PetInsulinLogs.ViewModels;

public partial class PetListViewModel : ObservableObject
{
    private readonly IPetRepository pets;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEmpty;

    public ObservableCollection<Pet> Items { get; } = new();

    public string CurrentUserId { get; set; } = "owner-local"; // TODO: auth

    public PetListViewModel(IPetRepository pets)
    {
        this.pets = pets;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Items.Clear();
            foreach (var p in await pets.GetByUserAsync(CurrentUserId))
                Items.Add(p);
            
            // Update empty state
            IsEmpty = Items.Count == 0;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task NavigateToOnboardingAsync()
    {
        await Shell.Current.GoToAsync("//onboarding");
    }
}
