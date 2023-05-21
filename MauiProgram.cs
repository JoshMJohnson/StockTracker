using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace StockTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		/* stock database access */
		string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stock_App_Database.db");
        builder.Services.AddSingleton<StockRepository>(s => ActivatorUtilities.CreateInstance<StockRepository>(s, dbPath));

		/* background service */
		builder.Services.AddSingleton<Settings>();

#if ANDROID
		builder.Services.AddTransient<Notification_Timers>();
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}
