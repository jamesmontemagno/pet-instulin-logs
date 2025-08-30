using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class UserService : IUserService
{
    public string CurrentUserId { get; private set; } = "default-user";
    public string CurrentUserName { get; private set; } = "Pet Owner";

    public async Task<string> GetCurrentUserIdAsync()
    {
        // In a real app, this might load from secure storage or authentication service
        await Task.CompletedTask;
        return CurrentUserId;
    }

    public async Task SetCurrentUserAsync(string userId, string userName)
    {
        CurrentUserId = userId;
        CurrentUserName = userName;
        
        // In a real app, might save to secure storage
        await SecureStorage.SetAsync("current_user_id", userId);
        await SecureStorage.SetAsync("current_user_name", userName);
    }
}