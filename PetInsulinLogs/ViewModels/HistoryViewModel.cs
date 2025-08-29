using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetInsulinLogs.Models;
using PetInsulinLogs.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PetInsulinLogs.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly ILogRepository logRepository;
    private readonly IPetRepository petRepository;

    [ObservableProperty]
    private Pet? selectedPet;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private OnTimeFlag? selectedOnTimeFilter;

    [ObservableProperty]
    private InjectionSite? selectedSiteFilter;

    [ObservableProperty]
    private bool? foodGivenFilter;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int totalEntries;

    [ObservableProperty]
    private int onTimeCount;

    [ObservableProperty]
    private int earlyCount;

    [ObservableProperty]
    private int lateCount;

    [ObservableProperty]
    private double averageDeviation;

    [ObservableProperty]
    private double totalUnits;

    [ObservableProperty]
    private double onTimePercentage;

    public ObservableCollection<Pet> Pets { get; } = new();
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public ObservableCollection<LogEntryGroup> GroupedLogEntries { get; } = new();

    public List<OnTimeFlag?> OnTimeFilterOptions { get; } = new()
    {
        null, // All
        OnTimeFlag.OnTime,
        OnTimeFlag.Early,
        OnTimeFlag.Late
    };

    public List<InjectionSite?> SiteFilterOptions { get; } = new()
    {
        null, // All
        InjectionSite.LeftShoulder,
        InjectionSite.MiddleShoulder,
        InjectionSite.RightShoulder,
        InjectionSite.Other
    };

    public List<string> FoodFilterOptions { get; } = new()
    {
        "All",
        "With Food",
        "Without Food"
    };

    public HistoryViewModel(
        ILogRepository logRepository,
        IPetRepository petRepository)
    {
        this.logRepository = logRepository;
        this.petRepository = petRepository;
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
            System.Diagnostics.Debug.WriteLine($"Error loading pets: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        if (SelectedPet == null || IsBusy) return;

        try
        {
            IsBusy = true;

            // Load all logs for the selected pet in the date range
            var allLogs = await logRepository.GetByPetAsync(
                SelectedPet.PetId,
                StartDate.ToUniversalTime(),
                EndDate.AddDays(1).ToUniversalTime());

            // Apply filters
            var filteredLogs = allLogs.AsEnumerable();

            if (SelectedOnTimeFilter.HasValue)
            {
                filteredLogs = filteredLogs.Where(l => l.OnTimeFlag == SelectedOnTimeFilter.Value);
            }

            if (SelectedSiteFilter.HasValue)
            {
                filteredLogs = filteredLogs.Where(l => l.InjectionSite == SelectedSiteFilter.Value);
            }

            if (FoodGivenFilter.HasValue)
            {
                filteredLogs = filteredLogs.Where(l => l.FoodGiven == FoodGivenFilter.Value);
            }

            var filteredList = filteredLogs.ToList();

            // Update log entries
            LogEntries.Clear();
            foreach (var log in filteredList)
            {
                LogEntries.Add(log);
            }

            // Update grouped entries for better UI display
            UpdateGroupedEntries(filteredList);

            // Calculate statistics
            CalculateStatistics(filteredList);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SelectedOnTimeFilter = null;
        SelectedSiteFilter = null;
        FoodGivenFilter = null;
        await LoadHistoryAsync();
    }

    private void UpdateGroupedEntries(List<LogEntry> logs)
    {
        GroupedLogEntries.Clear();

        var grouped = logs
            .GroupBy(l => l.TimestampUtc.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new LogEntryGroup
            {
                Date = g.Key,
                Entries = g.OrderByDescending(l => l.TimestampUtc).ToList()
            });

        foreach (var group in grouped)
        {
            GroupedLogEntries.Add(group);
        }
    }

    private void CalculateStatistics(List<LogEntry> logs)
    {
        TotalEntries = logs.Count;

        if (TotalEntries == 0)
        {
            OnTimeCount = EarlyCount = LateCount = 0;
            AverageDeviation = 0;
            TotalUnits = 0;
            OnTimePercentage = 0;
            return;
        }

        OnTimeCount = logs.Count(l => l.OnTimeFlag == OnTimeFlag.OnTime);
        EarlyCount = logs.Count(l => l.OnTimeFlag == OnTimeFlag.Early);
        LateCount = logs.Count(l => l.OnTimeFlag == OnTimeFlag.Late);

        AverageDeviation = logs.Average(l => Math.Abs(l.DeviationMinutes));
        TotalUnits = logs.Sum(l => l.Units);
        OnTimePercentage = TotalEntries > 0 ? (double)OnTimeCount / TotalEntries * 100 : 0;
    }

    partial void OnSelectedPetChanged(Pet? value)
    {
        if (value != null)
        {
            _ = LoadHistoryAsync();
        }
    }

    partial void OnStartDateChanged(DateTime value)
    {
        _ = LoadHistoryAsync();
    }

    partial void OnEndDateChanged(DateTime value)
    {
        _ = LoadHistoryAsync();
    }

    partial void OnSelectedOnTimeFilterChanged(OnTimeFlag? value)
    {
        _ = LoadHistoryAsync();
    }

    partial void OnSelectedSiteFilterChanged(InjectionSite? value)
    {
        _ = LoadHistoryAsync();
    }

    partial void OnFoodGivenFilterChanged(bool? value)
    {
        _ = LoadHistoryAsync();
    }

    public async Task SelectPetByIdAsync(string petId)
    {
        var pet = Pets.FirstOrDefault(p => p.PetId == petId);
        if (pet != null)
        {
            SelectedPet = pet;
            await LoadHistoryAsync();
        }
    }
}

public class LogEntryGroup
{
    public DateTime Date { get; set; }
    public string DateDisplay => Date.ToString("dddd, MMMM dd, yyyy");
    public List<LogEntry> Entries { get; set; } = new();
    public int Count => Entries.Count;
}