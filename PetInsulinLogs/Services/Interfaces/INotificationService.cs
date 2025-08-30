namespace PetInsulinLogs.Services.Interfaces;

public interface INotificationService
{
    Task ShowToastAsync(string message);
    Task ShowErrorAsync(string message);
    Task ShowSuccessAsync(string message);
}