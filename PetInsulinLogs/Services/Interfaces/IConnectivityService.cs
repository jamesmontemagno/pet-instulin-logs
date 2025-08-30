namespace PetInsulinLogs.Services.Interfaces;

public interface IConnectivityService
{
    bool IsConnected { get; }
    event EventHandler<bool> ConnectivityChanged;
    Task<bool> CheckConnectivityAsync();
}