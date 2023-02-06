namespace MVVM_Architecture.ViewModels;

using System.Diagnostics;
using Interfaces.Services;

public class SecondPageViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;

    public SecondPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public Command GoBackCommand 
        => new(async () => await this.navigationService.NavigateBack());

    public Command NextCommand 
        => new(async () => await this.navigationService.NavigateToThirdPage());

    public override Task OnNavigatedTo()
    {
        Debug.WriteLine("On navigated to SecondPage");
        return Task.CompletedTask;
    }

    public override Task OnNavigatingTo(object? parameter)
    {
        Debug.WriteLine($"On navigating to SecondPage with parameter {parameter}");
        return Task.CompletedTask;
    }

    public override Task OnNavigatedFrom(bool isForwardNavigation)
    {
        Debug.WriteLine($"On {(isForwardNavigation ? "forward" : "backward")} navigated from SecondPage");
        return Task.CompletedTask;
    }
}
