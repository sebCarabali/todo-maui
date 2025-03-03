using LoginApplication.Services;
using LoginApplication.Services.Interfaces;
using LoginApplication.ViewModels;
using LoginApplication.Views;
using Microsoft.Extensions.Logging;

namespace LoginApplication;

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

        // Register services
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register view models
		builder.Services.AddTransient<LoginViewModel>();

        // Register views
        builder.Services.AddTransient<LoginPage>();

        return builder.Build();
	}
}
