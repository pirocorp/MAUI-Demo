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

Of course, this is only accessible once the `MainPage` property is set, which is typically done in the constructor of the `App` class.

```csharp
public partial class App : Application
{
    public App(MainPage mainPage)
    {
        this.InitializeComponent();
        this.MainPage = new NavigationPage(mainPage);
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

Tapping this button will call the `MainPageViewModel`‘s (our `BindingContext`) `NavigateToSecondPageCommand`, which will call the `NavigationService`‘s `NavigateToSecondPage` method which will perform the actual navigation by resolving an instance of type `SecondPage` and navigate to that via the `App`‘s `MainPage`’s `Navigation` property.

### Navigating back with Navigation Service

All we have to do, is expose a method in the `NavigationService` (and the `INavigationService` interface) which implements the thing you want to achieve. Then you can call that method from your **ViewModel**.

NavigationService:

```csharp
public Task NavigateBack()
{
    if (Navigation.NavigationStack.Count > 1)
    {
        return Navigation.PopAsync();
    }

    throw new InvalidOperationException("No pages to navigate back to!");
}
```

INavigationService:

```csharp
public interface INavigationService
{
    Task NavigateToSecondPage();
    Task NavigateBack();
}
```

SecondPageViewModel:

```csharp
public class SecondPageViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;

    public SecondPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public Command GoBackCommand 
        => new(async () => await this.navigationService.NavigateBack());
}
```

SecondPage:

```xaml
<Button 
    Text="Go back!"
    FontAttributes="Bold"
    Grid.Row="3"
    Command="{Binding GoBackCommand}"
    HorizontalOptions="Center" />
```

### Passing parameters and more

Without being able to properly pass parameters from one page to an other, it’s not that very useful. On top of that, when navigating to or from a page, ideally some methods on your ViewModels should be called to do stuff like fetching or cleaning-up data.

Let’s add a `ViewModelBase` class with three abstract methods methods: `OnNavigatingTo`, `OnNavigatedFrom` and `OnNavigatedTo`

```csharp
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract Task OnNavigatingTo(object? parameter);

    public abstract Task OnNavigatedFrom(bool isForwardNavigation);

    public abstract Task OnNavigatedTo();

    public virtual void RaisePropertyChanged([CallerMemberName] string? property = null)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
```

These methods stand for the following:

- `OnNavigatingTo` is called when navigating ‘forward’ to a **View(Model)**. It accepts a parameter of type `object` which allows us to pass a parameter from one **ViewModel** to another.
- `OnNavigatedFrom` is called when we navigate away from a **View(Model)**. The parameter `isForwardNavigation` indicates if we’re navigating forward from this view to another (true), or if we navigate back from this view to the previous one (false). The later is particularly interesting to clean-up stuff as the page is no longer in the `NavigationStack`.
- `OnNavigatedTo` is called when we have navigated to a **View(Model)**. This method is called when we navigate to a new **View (forward navigation)** AND also when we navigate back to a **View (back navigation)**.

In order to call these methods, we need to add some extra code to the `NavigationService` and make sure our **ViewModels** inherit `ViewModelBase`.

First, let’s take a look at the updated NavigateToPage method:

```csharp
private async Task NavigateToPage<T>(object? parameter = null) where T : Page
{
    var toPage = this.ResolvePage<T>();

    //Subscribe to the toPage's NavigatedTo event
    toPage.NavigatedTo += PageNavigatedTo;

    //Get VM of the toPage
    var toViewModel = GetPageViewModelBase(toPage);

    //Call navigatingTo on VM, passing in the parameter
    if (toViewModel is not null)
    {
        await toViewModel.OnNavigatingTo(parameter);
    }

    //Navigate to requested page
    await Navigation.PushAsync(toPage, true);

    //Subscribe to the toPage's NavigatedFrom event
    toPage.NavigatedFrom += PageNavigatedFrom;
}
```

The first thing that’s added is the fact that this method now accepts an optional `parameter` which will serve as the `parameter` we can pass from one view to another.

Next, once we’ve resolved the page we want to navigate to, we’re going to subscribe to its `NavigatedTo` method. We’ll see the implementation of that in just a second, but you can already guess what it’ll do.

Another thing we added to this method, is the thing were we want to get the page’s **ViewModel**, of type `ViewModelBase`. Nothing fancy going on in this `GetPageViewModelBase` method:

```csharp
private static ViewModelBase? GetPageViewModelBase(BindableObject? page)
    => page?.BindingContext as ViewModelBase;
```

But back to the `NavigateToPage` method, once we have the `ViewModelBase`, we’re going to call our newly created method `OnNavigatingTo`, passing in the parameter.

After that, we’re going to navigate to our page and finally also subscribe to the Page’s `NavigatedFrom` event.

Let’s see what the NavigatedTo eventhandler that I talked about earlier looks like:

```csharp
private static async void PageNavigatedTo(object? sender, NavigatedToEventArgs e)
    => await CallNavigatedTo(sender as Page);
    
private static Task CallNavigatedTo(BindableObject? page)
{
    var fromViewModel = GetPageViewModelBase(page);

    return fromViewModel is not null 
        ? fromViewModel.OnNavigatedTo() 
        : Task.CompletedTask;
}
```

When it’s called, we are going to see if the page has a `ViewModelBase`. If that’s the case, we are going to call the **ViewModel’s** `OnNavigatedTo` method.

The final thing we need to take a look at, is the `NavigatedFrom` eventhandler:

```csharp
private static async void PageNavigatedFrom(object? sender, NavigatedFromEventArgs e)
{
    //To determine forward navigation, we look at the 2nd to last item on the NavigationStack
    //If that entry equals the sender, it means we navigated forward from the sender to another page
    var isForwardNavigation = Navigation.NavigationStack.Count > 1 
			  && Navigation.NavigationStack[^2] == sender;

    if (sender is Page thisPage)
    {
        if (!isForwardNavigation)
        {
	    thisPage.NavigatedTo -= PageNavigatedTo;
	    thisPage.NavigatedFrom -= PageNavigatedFrom;
        }

        await CallNavigatedFrom(thisPage, isForwardNavigation);
    }
}

private static Task CallNavigatedFrom(BindableObject page, bool isForward)
{
    var fromViewModel = GetPageViewModelBase(page);

    return fromViewModel is not null 
        ? fromViewModel.OnNavigatedFrom(isForward) 
        : Task.CompletedTask;
}
```

The `NavigatedFrom` event is triggered by MAUI when we navigate away from a particular page. This can be in one of two ways: or you navigate forward from that page to another, or you navigate back from the page to the previous page. The fact that the navigation is forward or backward is very valuable information, so we want to pass this to **ViewModel** we’ve navigated from.

By looking at the `NavigationStack` we can determine which way (forward or backward) we’ve navigated. If we’ve navigated forward from a particular page, that page should be the 2nd to last item in the NavigationStack (as the page we’ve navigated to will be the last page in the stack). If that’s not the case, we’ve navigated back.

Important to notice here is the fact that when it’s a back navigation, we want to unsubscribe from the page’s events! That way the page and **ViewModel** can be **GC**’ed as there are no more references to the page anymore.

So once we’ve determined if its forward or backward navigation, unsubscribed from events (if needed), we can look up de Page’s **ViewModelBase** and call its `OnNavigatedFrom` method.

With all of this in place, we can now start passing parameters from one **ViewModel** to another. If, for example `SecondPageViewModel` needs to get an id to be properly initialized, we need to do the following:

INavigationService:

```csharp
Task NavigateToSecondPage(string id);
```

NavigationService:

```csharp
public Task NavigateToSecondPage(string id) => this.NavigateToPage<SecondPage>(id);
```

MainPageViewModel:

```csharp
public Command NavigateToSecondPageCommand 
    => new(async () => await this.navigationService.NavigateToSecondPage("some id"));
```

SecondPageViewModel:

```csharp
public override Task OnNavigatingTo(object? parameter)
{
    Debug.WriteLine($"On navigating to SecondPage with parameter {parameter}");
    return Task.CompletedTask;
}
```

### One final thing

With all this navigation stuff in place, there is one final thing we shouldn’t forget. Remember how we initially set-up the MainPage property in our App? We injected in instance of `MainPage` in the `App`‘s constructor. While this works well, by doing so, we are completely bypassing all the above logic for our first page. Let’s make sure these navigation methods are also called on our initial **ViewModel**. To do so, we only need to tweak the `App` class a little bit:

```csharp
public App(INavigationService navigationService)
{
    InitializeComponent();
    MainPage = new NavigationPage();
    navigationService.NavigateToMainPage();
}
```

Instead of injecting an instance of `MainPage`, we’re going to inject an instance of the `INavigationService`. We’ll assign the MainPage property to a `new NavigationPage`, followed by calling `NavigateToMainPage` on our `NavigationService`:

