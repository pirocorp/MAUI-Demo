namespace MVVM_Architecture;

using MVVM_Architecture.ViewModels;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
    {
        this.BindingContext = viewModel;

        this.InitializeComponent();
    }
}
