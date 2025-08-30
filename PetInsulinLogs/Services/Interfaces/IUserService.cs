namespace PetInsulinLogs.Services.Interfaces;

public interface IUserService
{
    string CurrentUserId { get; }
    string CurrentUserName { get; }
    Task<string> GetCurrentUserIdAsync();
    Task SetCurrentUserAsync(string userId, string userName);
}