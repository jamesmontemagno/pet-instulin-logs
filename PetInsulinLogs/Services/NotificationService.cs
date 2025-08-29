using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class NotificationService : INotificationService
{
    public async Task ShowToastAsync(string message)
    {
        // In a real app, this would use a toast library like CommunityToolkit.Maui
        await DisplayAlert("Info", message);
    }

    public async Task ShowErrorAsync(string message)
    {
        await DisplayAlert("Error", message);
    }

    public async Task ShowSuccessAsync(string message)
    {
        await DisplayAlert("Success", message);
    }

    private async Task DisplayAlert(string title, string message)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }
}