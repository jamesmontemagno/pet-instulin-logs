using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs.Services;

public class ConnectivityService : IConnectivityService
{
    public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    public async Task<bool> CheckConnectivityAsync()
    {
        try
        {
            // You could ping a server here if needed
            await Task.Delay(100); // Simulate network check
            return IsConnected;
        }
        catch
        {
            return false;
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        var isConnected = e.NetworkAccess == NetworkAccess.Internet;
        ConnectivityChanged?.Invoke(this, isConnected);
    }
}