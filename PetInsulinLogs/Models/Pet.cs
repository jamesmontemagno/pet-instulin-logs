using SQLite;
using System.Text.Json;

namespace PetInsulinLogs.Models;

[Table("Pets")]
public class Pet
{
    [PrimaryKey]
    public string PetId { get; set; } = string.Empty; // UUID/ULID
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double DefaultUnits { get; set; }
    public int IntervalHours { get; set; } = 12;
    public string? PhotoUri { get; set; }
    
    // JSON storage for complex properties
    public string? FeedingPlanJson { get; set; }
    public string? VetInfoJson { get; set; }
    public string? EmergencyContactsJson { get; set; }
    
    // Computed properties for easy access
    [Ignore]
    public FeedingPlan FeedingPlan
    {
        get => string.IsNullOrEmpty(FeedingPlanJson) ? new FeedingPlan() : JsonSerializer.Deserialize<FeedingPlan>(FeedingPlanJson) ?? new FeedingPlan();
        set => FeedingPlanJson = JsonSerializer.Serialize(value);
    }
    
    [Ignore]
    public VetInfo VetInfo
    {
        get => string.IsNullOrEmpty(VetInfoJson) ? new VetInfo() : JsonSerializer.Deserialize<VetInfo>(VetInfoJson) ?? new VetInfo();
        set => VetInfoJson = JsonSerializer.Serialize(value);
    }
    
    [Ignore]
    public EmergencyContacts EmergencyContacts
    {
        get => string.IsNullOrEmpty(EmergencyContactsJson) ? new EmergencyContacts() : JsonSerializer.Deserialize<EmergencyContacts>(EmergencyContactsJson) ?? new EmergencyContacts();
        set => EmergencyContactsJson = JsonSerializer.Serialize(value);
    }
}

public class FeedingPlan
{
    public TimeOnly MorningTime { get; set; } = new TimeOnly(7, 0); // 7:00 AM
    public TimeOnly EveningTime { get; set; } = new TimeOnly(19, 0); // 7:00 PM
    public string FoodType { get; set; } = string.Empty;
    public string FoodAmount { get; set; } = string.Empty;
    public string SpecialInstructions { get; set; } = string.Empty;
}

public class VetInfo
{
    public string ClinicName { get; set; } = string.Empty;
    public string VetName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class EmergencyContacts
{
    public string Primary { get; set; } = string.Empty;
    public string PrimaryPhone { get; set; } = string.Empty;
    public string Secondary { get; set; } = string.Empty;
    public string SecondaryPhone { get; set; } = string.Empty;
    public string EmergencyVet { get; set; } = string.Empty;
    public string EmergencyVetPhone { get; set; } = string.Empty;
}
