namespace PetInsulinLogs.Services;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static void Initialize(IServiceProvider provider) => Services = provider;

    public static T Get<T>() where T : notnull => (T)Services.GetService(typeof(T))!;
}
