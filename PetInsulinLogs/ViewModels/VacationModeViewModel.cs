using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PetInsulinLogs.ViewModels;

public partial class VacationModeViewModel : ObservableObject
{
    private readonly IPetRepository petRepository;
    private readonly IVacationPlanRepository vacationPlanRepository;
    private readonly IVacationPlanningService vacationPlanningService;
    private readonly ILogRepository logRepository;

    [ObservableProperty]
    private Pet? selectedPet;

    [ObservableProperty]
    private TimeSpan targetAmTime = TimeSpan.FromHours(8);

    [ObservableProperty]
    private TimeSpan targetPmTime = TimeSpan.FromHours(20);

    [ObservableProperty]
    private int stepMinutes = 15;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private VacationPlan? currentPlan;

    [ObservableProperty]
    private bool hasPlan;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public ObservableCollection<Pet> Pets { get; } = new();
    public ObservableCollection<VacationScheduleEntry> SchedulePreview { get; } = new();

    public VacationModeViewModel(
        IPetRepository petRepository,
        IVacationPlanRepository vacationPlanRepository,
        IVacationPlanningService vacationPlanningService,
        ILogRepository logRepository)
    {
        this.petRepository = petRepository;
        this.vacationPlanRepository = vacationPlanRepository;
        this.vacationPlanningService = vacationPlanningService;
        this.logRepository = logRepository;
    }

    [RelayCommand]
    private async Task LoadPetsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            // TODO: Get current user ID from user service
            var currentUserId = "current-user";
            var pets = await petRepository.GetByUserAsync(currentUserId);

            Pets.Clear();
            foreach (var pet in pets)
            {
                Pets.Add(pet);
            }

            // Select first pet if available
            if (Pets.Count > 0 && SelectedPet == null)
            {
                SelectedPet = Pets[0];
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading pets: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading pets: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadCurrentPlanAsync()
    {
        if (SelectedPet == null) return;

        try
        {
            CurrentPlan = await vacationPlanRepository.GetAsync(SelectedPet.PetId);
            HasPlan = CurrentPlan != null;
            
            if (CurrentPlan != null)
            {
                TargetAmTime = CurrentPlan.TargetAm ?? TimeSpan.FromHours(8);
                TargetPmTime = CurrentPlan.TargetPm ?? TimeSpan.FromHours(20);
                StepMinutes = CurrentPlan.StepMinutes;
                
                StatusMessage = CurrentPlan.Active ? "Vacation plan is active" : "Vacation plan exists but is paused";
            }
            else
            {
                StatusMessage = "No vacation plan exists for this pet";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading plan: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading vacation plan: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GeneratePreviewAsync()
    {
        if (SelectedPet == null) return;

        try
        {
            // Get last log entry to help with current schedule calculation
            var recentLogs = await logRepository.GetByPetAsync(SelectedPet.PetId);
            var lastLog = recentLogs.FirstOrDefault();

            // Generate the plan
            var plan = vacationPlanningService.GeneratePlan(
                SelectedPet, 
                TargetAmTime, 
                TargetPmTime, 
                StepMinutes, 
                StartDate);

            // Show preview of first 10 entries
            SchedulePreview.Clear();
            var previewEntries = plan.GeneratedSchedule?.Take(10) ?? Enumerable.Empty<VacationScheduleEntry>();
            foreach (var entry in previewEntries)
            {
                SchedulePreview.Add(entry);
            }

            StatusMessage = $"Generated {plan.GeneratedSchedule?.Count ?? 0} schedule entries";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error generating preview: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error generating preview: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CreatePlanAsync()
    {
        if (SelectedPet == null || IsBusy) return;

        try
        {
            IsBusy = true;

            // Get last log entry
            var recentLogs = await logRepository.GetByPetAsync(SelectedPet.PetId);
            var lastLog = recentLogs.FirstOrDefault();

            // Generate the plan
            var plan = vacationPlanningService.GeneratePlan(
                SelectedPet, 
                TargetAmTime, 
                TargetPmTime, 
                StepMinutes, 
                StartDate);

            // Save the plan
            await vacationPlanRepository.SetAsync(plan);

            CurrentPlan = plan;
            HasPlan = true;
            StatusMessage = "Vacation plan created successfully. Use 'Activate' to start using it.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating plan: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error creating plan: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ActivatePlanAsync()
    {
        if (CurrentPlan == null || IsBusy) return;

        try
        {
            IsBusy = true;

            CurrentPlan.Active = true;
            await vacationPlanRepository.SetAsync(CurrentPlan);

            StatusMessage = "Vacation plan activated! Shot timing will now follow the vacation schedule.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error activating plan: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error activating plan: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PausePlanAsync()
    {
        if (CurrentPlan == null || IsBusy) return;

        try
        {
            IsBusy = true;

            CurrentPlan.Active = false;
            await vacationPlanRepository.SetAsync(CurrentPlan);

            StatusMessage = "Vacation plan paused. Shot timing will revert to normal schedule.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error pausing plan: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error pausing plan: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeletePlanAsync()
    {
        if (SelectedPet == null || IsBusy) return;

        try
        {
            IsBusy = true;

            await vacationPlanRepository.DeleteAsync(SelectedPet.PetId);

            CurrentPlan = null;
            HasPlan = false;
            SchedulePreview.Clear();
            StatusMessage = "Vacation plan deleted.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting plan: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error deleting plan: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedPetChanged(Pet? value)
    {
        if (value != null)
        {
            _ = LoadCurrentPlanAsync();
        }
    }

    public async Task SelectPetByIdAsync(string petId)
    {
        var pet = Pets.FirstOrDefault(p => p.PetId == petId);
        if (pet != null)
        {
            SelectedPet = pet;
            await LoadCurrentPlanAsync();
        }
    }
}