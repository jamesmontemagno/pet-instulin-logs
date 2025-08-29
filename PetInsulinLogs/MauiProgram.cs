using Microsoft.Extensions.Logging;
using PetInsulinLogs.Services;
using PetInsulinLogs.Services.Interfaces;

namespace PetInsulinLogs;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// SQLite database path
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "petinsulin.db3");
	builder.Services.AddSingleton<ISqliteConnectionProvider>(_ => new SqliteConnectionProvider(dbPath));

		// Core services
	builder.Services.AddSingleton<IIdService, IdService>();
	builder.Services.AddSingleton<ITimeService, TimeService>();
	builder.Services.AddSingleton<IScheduleEngineService, ScheduleEngineService>();

		// Repositories
	builder.Services.AddSingleton<IPetRepository, PetRepository>();
	builder.Services.AddSingleton<ILogRepository, LogRepository>();
	builder.Services.AddSingleton<IVacationPlanRepository, VacationPlanRepository>();

		// ViewModels
		builder.Services.AddTransient<ViewModels.PetListViewModel>();
		builder.Services.AddTransient<ViewModels.PetProfileViewModel>();
		builder.Services.AddTransient<ViewModels.LogShotViewModel>();

		// Views
		builder.Services.AddTransient<Views.PetListPage>();
		builder.Services.AddTransient<Views.PetProfilePage>();
		builder.Services.AddTransient<Views.OnboardingPage>();
		builder.Services.AddTransient<Views.LogShotPage>();

		var app = builder.Build();
		ServiceHelper.Initialize(app.Services);
		return app;
	}
}
