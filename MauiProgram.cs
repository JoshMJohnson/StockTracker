﻿using Microsoft.Extensions.Logging;

namespace StockTracker;

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

		string dbPath = FileAccessHelper.GetLocalFilePath("Stock_App_Database.db");
		builder.Services.AddSingleton<StockRepository>(s => ActivatorUtilities.CreateInstance<StockRepository>(s, dbPath));


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
