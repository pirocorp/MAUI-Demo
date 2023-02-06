# MVVM Architecture with .NET MAUI

## Dependency injection

One of the awesome new things in MAUI is that we now have a DI container at our disposal out-of-the-box. Just like with ASP.NET Core applications, there’s now this `Builder` that let’s you configure your application in a fluent style and which exposes a DI container through the `Builder.Services` property.

If we want, for example, to add our `MainPage` class to the DI container, we can do this as follows:

```csharp
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

    builder.Services.AddTransient<MainPage>();

		return builder.Build();
	}
```
