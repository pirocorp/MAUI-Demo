namespace ToDo.Maui
{
    using Microsoft.Extensions.Logging;
    using Pages;
    using ToDo.Maui.DataServices;

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

            // builder.Services.AddSingleton<IToDoDataService, ToDoDataService>();
            builder.Services.AddHttpClient<IToDoDataService, ToDoDataService>(); // HttpClient will be managed by HttpClientFactory
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<ManageToDoPage>();

            return builder.Build();
        }
    }
}