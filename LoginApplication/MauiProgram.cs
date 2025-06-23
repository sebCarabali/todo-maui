using LoginApplication.Config;
using LoginApplication.ImageProcessing;
using LoginApplication.Services;
using LoginApplication.Services.Interfaces;
using LoginApplication.ViewModels;
using LoginApplication.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
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
        var assembly = Assembly.GetExecutingAssembly();

        // Use the EXACT resource name from the debug output
        var resourceName = "LoginApplication.appsettings.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        builder.Services.Configure<Endpoints>(configuration.GetSection("Endpoints"));
        builder.Services.Configure<ImageOptimization>(configuration.GetSection("ImageOptimization"));

        // Register httpclient
        builder.Services.AddHttpClient();

        // Register services
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ImageOptimizer>();
        builder.Services.AddScoped<ICameraService, CameraService>();
        builder.Services.AddScoped<IClienteService, ClienteService>();
        builder.Services.AddScoped<IFacialAuthenticationService, FacialAuthenticationService>();
        builder.Services.AddScoped<IFeatureExtractionService, FeatureExtractionService>();
        builder.Services.AddScoped<IRegistroAccesoService, RegistroAccesoService>();

        // Register view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<UserAuthenticationViewModel>();
        builder.Services.AddTransient<UserRegistrationViewModel>();

        // Register views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<UserAuthenticationPage>();
        builder.Services.AddTransient<UserRegistrationPage>();
        builder.Services.AddTransient<DashboardPage>();

        // Register SecureStorage
        builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);

        return builder.Build();
    }
}
