namespace MVVM_Architecture.ViewModels;

using System.Diagnostics;

using MVVM_Architecture.Interfaces.Services;

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

    public override Task OnNavigatedTo()
    {
        Debug.WriteLine("On navigated to MainPage");

        return Task.CompletedTask;
    }

    public override Task OnNavigatingTo(object? parameter)
    {
        Debug.WriteLine($"On navigating to MainPage with parameter {parameter}");

        return Task.CompletedTask;
    }

    public override Task OnNavigatedFrom(bool isForwardNavigation)
    {
        Debug.WriteLine($"On {(isForwardNavigation ? "forward" : "backward")} navigated from MainPage");

        return Task.CompletedTask;
    }
}
