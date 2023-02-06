namespace ToDo.Maui
{
    using System.Diagnostics;
    
    using ToDo.Maui.DataServices;
    using ToDo.Maui.Models;
    using ToDo.Maui.Pages;

    public partial class MainPage : ContentPage
    {
        private readonly IToDoDataService dataService;

        public MainPage(IToDoDataService dataService)
        {
            this.dataService = dataService;

            this.InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.CollectionView.ItemsSource = await this.dataService.GetAllToDos();
        }

        private async void OnAddToDoClicked(object sender, EventArgs args)
        {
            Debug.WriteLine("---> Add button clicked!");

            var navigationParameter = new Dictionary<string, object>()
            {
                { nameof(ToDo), new ToDo() }
            };

            await Shell.Current.GoToAsync(nameof(ManageToDoPage), navigationParameter);
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var navigationParameter = new Dictionary<string, object>()
            {
                { nameof(ToDo), args.CurrentSelection.FirstOrDefault() as ToDo }
            };

            await Shell.Current.GoToAsync(nameof(ManageToDoPage), navigationParameter);
        }
    }
}