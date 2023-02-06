namespace MVVM_Architecture.Pages;

using MVVM_Architecture.ViewModels;

public partial class SecondPage : ContentPage
{
    public SecondPage(SecondPageViewModel viewModel)
    {
        this.BindingContext = viewModel;

        this.InitializeComponent();
    }
}