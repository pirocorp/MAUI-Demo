namespace MVVM_Architecture;

using Interfaces.Services;

public partial class App : Application
{
    public App(INavigationService navigationService)
	{
        this.InitializeComponent();

        this.MainPage = new NavigationPage();
        navigationService.NavigateToMainPage();
	}
}
