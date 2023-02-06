namespace MVVM_Architecture;

using Microsoft.Extensions.Logging;

using MVVM_Architecture.Interfaces.Services;
using MVVM_Architecture.Pages;
using MVVM_Architecture.Services;
using MVVM_Architecture.ViewModels;

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

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<SecondPage>();
        builder.Services.AddTransient<SecondPageViewModel>();
        builder.Services.AddTransient<ThirdPage>();
        builder.Services.AddTransient<ThirdPageViewModel>();

        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        return builder.Build();
	}
}
