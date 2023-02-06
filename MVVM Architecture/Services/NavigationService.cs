namespace MVVM_Architecture.Services;

using MVVM_Architecture.Interfaces.Services;
using MVVM_Architecture.Pages;
using MVVM_Architecture.ViewModels;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider services;

    public NavigationService(IServiceProvider services)
    {
        this.services = services;
    }

    protected static INavigation Navigation => Application.Current?.MainPage?.Navigation
        ?? throw new InvalidOperationException("Navigation property is null");

    public Task NavigateToMainPage() => this.NavigateToPage<MainPage>();

    public Task NavigateToSecondPage(string id) => this.NavigateToPage<SecondPage>(id);

    public Task NavigateToThirdPage() => this.NavigateToPage<ThirdPage>();

    public Task NavigateBack()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            return Navigation.PopAsync();
        }

        throw new InvalidOperationException("No pages to navigate back to!");
    }

    private static async void PageNavigatedTo(object? sender, NavigatedToEventArgs e)
        => await CallNavigatedTo(sender as Page);

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

    private static Task CallNavigatedTo(BindableObject? page)
    {
        var fromViewModel = GetPageViewModelBase(page);

        return fromViewModel is not null 
            ? fromViewModel.OnNavigatedTo() 
            : Task.CompletedTask;
    }

    private static Task CallNavigatedFrom(BindableObject page, bool isForward)
    {
        var fromViewModel = GetPageViewModelBase(page);

        return fromViewModel is not null 
            ? fromViewModel.OnNavigatedFrom(isForward) 
            : Task.CompletedTask;
    }

    private static ViewModelBase? GetPageViewModelBase(BindableObject? page)
        => page?.BindingContext as ViewModelBase;

    private T ResolvePage<T>() where T : Page 
        => this.services.GetService<T>() 
        ?? throw new InvalidOperationException($"Unable to resolve type {typeof(T).FullName}");

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
}
