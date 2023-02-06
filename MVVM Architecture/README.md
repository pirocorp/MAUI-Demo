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
So, to have DI working, we don’t need to do anything special because we already get this out of the box.


## Binding ViewModels to Views

We can use this mechanism as well to inject a ViewModel into our View. The only thing we have to do, is adding a constructor parameter to our MainPage, and make sure we register such a type (MainPageViewModel for example) in our DI container.

```csharp
public MainPage(MainPageViewModel viewModel)
{
    this.viewModel = viewModel;

    this.InitializeComponent();
}
```

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });
	
    builder.Services.AddTransient<MainPage>();
    builder.Services.AddTransient<MainPageViewModel>();
    
    return builder.Build();
}
```

This means that when an instance of `App` is resolved, `MainPage` needs to be resolved and all of its dependencies, like in this example `MainPageViewModel`. If said **ViewModel** would have a dependency (like a **Service** or **Repository**) on itself, which needs to be injected through the constructor, the **DI container** will (try to) resolve all of them. If you wouldn’t have registered a particular type which is a dependency of another type, a `System.InvalidOperationException` will be thrown, as you might expect.

The following code demonstrates how you can add a dependency of an `IDataService` interface to the `MainPageViewModel`, and how to register a concrete implementation of said interface.

```csharp
public class MainPageViewModel : ViewModelBase
{
    private readonly IDataService dataService;

    public MainPageViewModel(IDataService dataService)
    {
        this.dataService = dataService;
    }
}
```

## NavigationService

A `NavigationService` should be responsible for…. navigation, of course. It should expose methods that can be called in order to navigate from one page to another, and passing parameters from one **ViewModel** to another. I would like a `NavigationService` to be injected in my **ViewModels**, so that I can perform a navigation from a command that was triggered when the user tapped a button, for example. Injecting such a `NavigationService` into our **ViewModels** is the easy part, thanks to MAUI’s baked-in DI container.

How can we implement the navigation itself? Well, navigation in MAUI is done through the `INavigation` interface. Once we get a hold of an implementation of that interface, we can do stuff like `PushAsync`, `PopAync`, `PushModalAsync`, … Every Page in MAUI has a `Navigation` property which is of type `INavigation`. But in our architecture, the **ViewModel** doesn’t know the Page, it doesn’t have a reference to it. Luckily, we can access the App‘s MainPage and get its `Navigation` property:

```csharp
INavigation navigation = App.Current.MainPage.Navigation;
```

Of course, this is only accessible once the MainPage property is set, which is typically done in the constructor of the App class.

```csharp
public partial class App : Application
{
    public App(INavigationService navigationService)
    {
        this.InitializeComponent();

        this.MainPage = new NavigationPage();
        navigationService.NavigateToMainPage();
    }
}
```







