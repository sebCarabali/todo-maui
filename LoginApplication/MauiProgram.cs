using LoginApplication.Services;
using LoginApplication.Services.Interfaces;
using LoginApplication.ViewModels;
using LoginApplication.Views;
using Microsoft.Extensions.Configuration;
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

        // Load appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // Register configuration
        builder.Services.AddSingleton<IConfiguration>(config);

        // Register services
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register view models
		builder.Services.AddTransient<LoginViewModel>();

        // Register views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();

        // Register SecureStorage
        builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);

        return builder.Build();
	}
}
