using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PetInsulinLogs.ViewModels;

public partial class LogShotViewModel : ObservableObject
{
    private readonly ILogRepository logRepository;
    private readonly IPetRepository petRepository;
    private readonly IScheduleEngineService scheduleEngine;
    private readonly IVacationPlanRepository vacationPlanRepository;
    private readonly IUserService userService;

    [ObservableProperty]
    private Pet? selectedPet;

    [ObservableProperty]
    private DateTime shotTime = DateTime.Now;

    [ObservableProperty]
    private DateTime shotDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan shotTimeOfDay = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private double units;

    [ObservableProperty]
    private InjectionSite injectionSite = InjectionSite.LeftShoulder;

    [ObservableProperty]
    private bool foodGiven;

    [ObservableProperty]
    private string? foodType;

    [ObservableProperty]
    private string? foodAmount;

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private string? bgNotes;

    [ObservableProperty]
    private string? warningMessage;

    [ObservableProperty]
    private bool hasWarning;

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<Pet> Pets { get; } = new();
    public List<InjectionSite> InjectionSites { get; } = Enum.GetValues<InjectionSite>().ToList();

    public LogShotViewModel(
        ILogRepository logRepository,
        IPetRepository petRepository,
        IScheduleEngineService scheduleEngine,
        IVacationPlanRepository vacationPlanRepository,
        IUserService userService)
    {
        this.logRepository = logRepository;
        this.petRepository = petRepository;
        this.scheduleEngine = scheduleEngine;
        this.vacationPlanRepository = vacationPlanRepository;
        this.userService = userService;
    }

    [RelayCommand]
    private async Task LoadPetsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var currentUserId = await userService.GetCurrentUserIdAsync();
            var pets = await petRepository.GetByUserAsync(currentUserId);
            
            Pets.Clear();
            foreach (var pet in pets)
            {
                Pets.Add(pet);
            }

            // Select first pet if available and set default units
            if (Pets.Count > 0 && SelectedPet == null)
            {
                SelectedPet = Pets[0];
                Units = SelectedPet.DefaultUnits;
            }
        }
        catch (Exception ex)
        {
            // TODO: Handle error - could show toast or dialog
            System.Diagnostics.Debug.WriteLine($"Error loading pets: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EvaluateTimingAsync()
    {
        if (SelectedPet == null) return;

        try
        {
            // Get last log entry for this pet
            var recentLogs = await logRepository.GetByPetAsync(SelectedPet.PetId);
            var lastLog = recentLogs.FirstOrDefault();

            // Get vacation plan if active
            var vacationPlan = await vacationPlanRepository.GetAsync(SelectedPet.PetId);

            // Evaluate the shot timing
            var evaluation = scheduleEngine.EvaluateShot(SelectedPet, ShotTime, lastLog, vacationPlan);

            // Check for duplicate within 30 minutes
            var duplicateWarning = await CheckForDuplicateAsync(SelectedPet.PetId, ShotTime);

            // Set warning message
            if (!string.IsNullOrEmpty(duplicateWarning))
            {
                WarningMessage = duplicateWarning;
                HasWarning = true;
            }
            else if (!string.IsNullOrEmpty(evaluation.WarningMessage))
            {
                WarningMessage = evaluation.WarningMessage;
                HasWarning = true;
            }
            else
            {
                HasWarning = false;
                WarningMessage = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error evaluating timing: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SubmitLogAsync()
    {
        if (SelectedPet == null || IsBusy) return;

        try
        {
            IsBusy = true;

            // Get last log and vacation plan for evaluation
            var recentLogs = await logRepository.GetByPetAsync(SelectedPet.PetId);
            var lastLog = recentLogs.FirstOrDefault();
            var vacationPlan = await vacationPlanRepository.GetAsync(SelectedPet.PetId);

            // Evaluate the shot
            var evaluation = scheduleEngine.EvaluateShot(SelectedPet, ShotTime, lastLog, vacationPlan);

            // Create log entry
            var logEntry = new LogEntry
            {
                PetId = SelectedPet.PetId,
                UserId = await userService.GetCurrentUserIdAsync(),
                TimestampUtc = ShotTime.ToUniversalTime(),
                Units = Units,
                InjectionSite = InjectionSite,
                FoodGiven = FoodGiven,
                FoodType = FoodGiven ? FoodType : null,
                FoodAmount = FoodGiven ? FoodAmount : null,
                Notes = Notes,
                BgNotes = BgNotes,
                OnTimeFlag = evaluation.OnTimeFlag,
                DeviationMinutes = evaluation.DeviationMinutes
            };

            // Save log entry
            await logRepository.AppendAsync(logEntry);

            // TODO: Navigate back or show success message
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            // TODO: Show error message to user
            System.Diagnostics.Debug.WriteLine($"Error saving log: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<string?> CheckForDuplicateAsync(string petId, DateTime proposedTime)
    {
        try
        {
            var thirtyMinutesAgo = proposedTime.AddMinutes(-30);
            var thirtyMinutesLater = proposedTime.AddMinutes(30);

            var recentLogs = await logRepository.GetByPetAsync(
                petId, 
                thirtyMinutesAgo.ToUniversalTime(), 
                thirtyMinutesLater.ToUniversalTime());

            if (recentLogs.Any())
            {
                return "Warning: A shot was already logged within 30 minutes of this time.";
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    partial void OnSelectedPetChanged(Pet? value)
    {
        if (value != null)
        {
            Units = value.DefaultUnits;
            _ = EvaluateTimingAsync();
        }
    }

    partial void OnShotDateChanged(DateTime value)
    {
        ShotTime = value.Date.Add(ShotTimeOfDay);
    }

    partial void OnShotTimeOfDayChanged(TimeSpan value)
    {
        ShotTime = ShotDate.Date.Add(value);
    }

    partial void OnShotTimeChanged(DateTime value)
    {
        _ = EvaluateTimingAsync();
    }

    public async Task SelectPetByIdAsync(string petId)
    {
        var pet = Pets.FirstOrDefault(p => p.PetId == petId);
        if (pet != null)
        {
            SelectedPet = pet;
            Units = pet.DefaultUnits;
            await EvaluateTimingAsync();
        }
    }
}