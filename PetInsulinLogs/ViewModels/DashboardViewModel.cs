using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PetInsulinLogs.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IPetRepository petRepository;
    private readonly ILogRepository logRepository;
    private readonly IScheduleEngineService scheduleEngine;
    private readonly ITimeService timeService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool hasPets;

    public ObservableCollection<PetDashboardInfo> Pets { get; } = new();

    public string CurrentUserId { get; set; } = "owner-local"; // TODO: auth

    public DashboardViewModel(
        IPetRepository petRepository, 
        ILogRepository logRepository, 
        IScheduleEngineService scheduleEngine,
        ITimeService timeService)
    {
        this.petRepository = petRepository;
        this.logRepository = logRepository;
        this.scheduleEngine = scheduleEngine;
        this.timeService = timeService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Pets.Clear();
            
            var pets = await petRepository.GetByUserAsync(CurrentUserId);
            
            // If no pets exist, navigate to onboarding
            if (pets.Count == 0)
            {
                await Shell.Current.GoToAsync("onboarding");
                return;
            }
            
            foreach (var pet in pets)
            {
                // Get recent logs for this pet
                var recentLogs = await logRepository.GetByPetAsync(pet.PetId, 
                    timeService.UtcNow.AddDays(-7), timeService.UtcNow);
                var lastLog = recentLogs.OrderByDescending(l => l.TimestampUtc).FirstOrDefault();
                
                // Calculate next shot time
                var nextShotTime = scheduleEngine.CalculateNextDueTime(pet, lastLog);
                
                var dashboardInfo = new PetDashboardInfo
                {
                    Pet = pet,
                    LastShotInfo = GetLastShotInfo(lastLog),
                    NextShotInfo = GetNextShotInfo(nextShotTime),
                    NextShotColor = GetNextShotColor(nextShotTime),
                    ScheduleInfo = $"{pet.DefaultUnits} units every {pet.IntervalHours} hours"
                };
                
                Pets.Add(dashboardInfo);
            }
            
            HasPets = Pets.Count > 0;
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    [RelayCommand]
    public async Task ShowPetActionsAsync(Pet pet)
    {
        var action = await Shell.Current.DisplayActionSheet(
            $"Actions for {pet.Name}", 
            "Cancel", 
            null, 
            "Log Insulin Shot", 
            "View History", 
            "Vacation Mode", 
            "Edit Profile");

        switch (action)
        {
            case "Log Insulin Shot":
                await Shell.Current.GoToAsync($"logshot?petId={pet.PetId}");
                break;
            case "View History":
                await Shell.Current.GoToAsync($"history?petId={pet.PetId}");
                break;
            case "Vacation Mode":
                await Shell.Current.GoToAsync($"vacation?petId={pet.PetId}");
                break;
            case "Edit Profile":
                await Shell.Current.GoToAsync($"pets/profile?petId={pet.PetId}");
                break;
        }
    }

    [RelayCommand]
    public async Task NavigateToLogShotAsync()
    {
        await Shell.Current.GoToAsync("logshot");
    }

    [RelayCommand]
    public async Task NavigateToHistoryAsync()
    {
        await Shell.Current.GoToAsync("history");
    }

    [RelayCommand]
    public async Task NavigateToVacationAsync()
    {
        await Shell.Current.GoToAsync("vacation");
    }

    [RelayCommand]
    public async Task AddPetAsync()
    {
        await Shell.Current.GoToAsync("pets/profile");
    }

    [RelayCommand]
    public async Task NavigateToOnboardingAsync()
    {
        await Shell.Current.GoToAsync("onboarding");
    }

    private string GetLastShotInfo(LogEntry? lastLog)
    {
        if (lastLog == null)
            return "No shots recorded";

        var timeSince = timeService.UtcNow - lastLog.TimestampUtc;
        var localTime = lastLog.TimestampUtc.ToLocalTime();
        
        if (timeSince.TotalHours < 1)
            return $"{(int)timeSince.TotalMinutes}m ago ({lastLog.Units} units)";
        else if (timeSince.TotalDays < 1)
            return $"{(int)timeSince.TotalHours}h ago ({lastLog.Units} units)";
        else
            return $"{localTime:MMM d, h:mm tt} ({lastLog.Units} units)";
    }

    private string GetNextShotInfo(DateTime nextShotTime)
    {
        var timeUntil = nextShotTime - timeService.UtcNow;
        var localTime = nextShotTime.ToLocalTime();
        
        if (timeUntil.TotalMinutes < 0)
            return $"Overdue by {(int)Math.Abs(timeUntil.TotalMinutes)}m";
        else if (timeUntil.TotalHours < 1)
            return $"Due in {(int)timeUntil.TotalMinutes}m";
        else if (timeUntil.TotalDays < 1)
            return $"Due in {(int)timeUntil.TotalHours}h {(int)(timeUntil.TotalMinutes % 60)}m";
        else
            return $"Due {localTime:MMM d, h:mm tt}";
    }

    private Color GetNextShotColor(DateTime nextShotTime)
    {
        var timeUntil = nextShotTime - timeService.UtcNow;
        
        if (timeUntil.TotalMinutes < 0)
            return Colors.Red; // Overdue
        else if (timeUntil.TotalMinutes < 30)
            return Colors.Orange; // Due soon
        else
            return Colors.Green; // On schedule
    }
}

public class PetDashboardInfo
{
    public Pet Pet { get; set; } = new();
    public string LastShotInfo { get; set; } = string.Empty;
    public string NextShotInfo { get; set; } = string.Empty;
    public Color NextShotColor { get; set; } = Colors.Gray;
    public string ScheduleInfo { get; set; } = string.Empty;
}