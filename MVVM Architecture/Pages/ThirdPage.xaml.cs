namespace MVVM_Architecture.Pages;

using ViewModels;

public partial class ThirdPage : ContentPage
{
	public ThirdPage(ThirdPageViewModel viewModel)
	{
        this.BindingContext = viewModel;

		this.InitializeComponent();
	}
}