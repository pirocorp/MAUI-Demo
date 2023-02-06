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
    this.BindingContext = viewModel;

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

Let’s take a look at what the first implementation of `NavigationService` looks like:

```csharp
public class NavigationService : INavigationService
{
    private readonly IServiceProvider services;
    
    public NavigationService(IServiceProvider services)
    {
        this.services = services;
    }

    protected static INavigation Navigation => Application.Current?.MainPage?.Navigation
        ?? throw new InvalidOperationException("Navigation property is null");

    public Task NavigateToSecondPage()
    {
        var page = _services.GetService<SecondPage>();

        if (page is not null)
        {
	    return Navigation.PushAsync(page, true);
        }

        throw new InvalidOperationException($"Unable to resolve type SecondPage");
    }
}
```

Let’ break this down!

Firstly, the `Navigation` property provides access to the `Navigation` property of the `App`‘s `MainPage`. This is just for convenience inside the `NavigationService`.

Secondly, you’ll notice that the `NavigationService` has a **constructor** with a parameter of type `IServiceProvider`. We can use this **ServiceProvider** to resolve the classes or instances we registered in the **MauiProgram** class. This **ServiceProvider** will get injected when we resolve an instance of the **NavigationService**, and we’ll set this **ServiceProvider** as a readonly field on our class.

Finally, there is a `NavigateToSecondPage` method. This is the kind of method that should be called from a **ViewModel** that gets this **NavigationService** injected. By using the injected **IServiceProvider**, we’ll try to resolve the type of page we want to navigate to. Once we have resolved an instance, we use the **Navigation** property of type **INavigation** to navigate to the resolved instance. While we resolved the requested page, the DI container will have resolved all dependencies, like a **ViewModel** and its dependencies, as long as everything was registered in our container.

We can even clean the **NavigateToSecondPage** up, as most of the code in there can be reused once we add methods to navigate to other pages:

```csharp
public Task NavigateToSecondPage() => NavigateToPage<SecondPage>();

private Task NavigateToPage<T>() where T : Page
{
    var page = ResolvePage<T>();

    if(page is not null)
    {
    	return Navigation.PushAsync(page, true);
    }

    throw new InvalidOperationException($"Unable to resolve type {typeof(T).FullName}");
}

private T? ResolvePage<T>() where T : Page
    => this.services.GetService<T>();
```

Now that we have the NavigationService in place we can register it in our DI container, along with our new **Page** and its **ViewModel**:

```csharp
builder.Services.AddTransient<SecondPage>();
builder.Services.AddTransient<SecondPageViewModel>();
builder.Services.AddSingleton<INavigationService, NavigationService>();
```

Next, we can update the `MainPageViewModel` so that an instance of type `INavigationService` is injected and call its `NavigateToSecondPage` method in a command, for example:

```csharp
public class MainPageViewModel : ViewModelBase
{
    private readonly IDataService dataService;
    private readonly INavigationService navigationService;

    public MainPageViewModel(IDataService dataService, INavigationService navigationService)
    {
        this.dataService = dataService;
        this.navigationService = navigationService;
    }

    public Command NavigateToSecondPageCommand 
        => new(async () => await this.navigationService.NavigateToSecondPage("some id"));
}
```

All that’s left to do, is bind this **NavigateCommand** to a Button in XAML:

```csharp
<Button 
    Text="Click me"
    FontAttributes="Bold"
    Grid.Row="3"
    Command="{Binding NavigateToSecondPageCommand}"
    HorizontalOptions="Center" />
```

Tapping this button will call the `MainPageViewModel`‘s (our BindingContext) `NavigateToSecondPageCommand`, which will call the NavigationService‘s `NavigateToSecondPage` method which will perform the actual navigation by resolving an instance of type `SecondPage` and navigate to that via the App‘s `MainPage`’s `Navigation` property.


