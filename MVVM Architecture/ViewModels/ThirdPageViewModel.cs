namespace MVVM_Architecture.ViewModels;

using System.Diagnostics;
using Interfaces.Services;

public class ThirdPageViewModel : ViewModelBase
{
    readonly INavigationService navigationService;

    public ThirdPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public Command GoBackCommand
        => new (async () => await this.navigationService.NavigateBack());

    public override Task OnNavigatedTo()
    {
        Debug.WriteLine("On navigated to ThirdPage");
        return Task.CompletedTask;
    }

    public override Task OnNavigatingTo(object? parameter)
    {
        Debug.WriteLine($"On navigating to ThirdPage with parameter {parameter}");
        return Task.CompletedTask;
    }

    public override Task OnNavigatedFrom(bool isForwardNavigation)
    {
        Debug.WriteLine($"On {(isForwardNavigation ? "forward" : "backward")} navigated from ThirdPage");
        return Task.CompletedTask;
    }
}
