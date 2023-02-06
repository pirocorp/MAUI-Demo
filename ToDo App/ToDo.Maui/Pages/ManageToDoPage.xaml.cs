namespace ToDo.Maui.Pages;

using System.Diagnostics;

using ToDo.Maui.DataServices;
using ToDo.Maui.Models;

[QueryProperty(nameof(ToDo), "ToDo")]
public partial class ManageToDoPage : ContentPage
{
    private readonly IToDoDataService dataService;
    private ToDo toDo;
    private bool isNew;

    public ManageToDoPage(IToDoDataService dataService)
    {
        this.dataService = dataService;

        // Set the binding context to the current UI control.
        this.BindingContext = this;

        this.InitializeComponent();
    }

    public ToDo ToDo
    {
        get => this.toDo;
        set
        {
            this.isNew = this.IsNew(value);
            this.toDo = value;

            this.OnPropertyChanged();
        }
    }

    private bool IsNew(ToDo item) => item.Id == 0;

    private async void OnSaveButtonClicked(object sender, EventArgs args)
    {
        if (this.isNew)
        {
            Debug.WriteLine("---> Add new Item");
            await this.dataService.AddToDo(this.ToDo);
        }
        else
        {
            Debug.WriteLine("---> Edit Item");
            await this.dataService.UpdateToDo(this.ToDo);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs args)
    {
        await this.dataService.DeleteToDo(this.ToDo.Id);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelButtonClicked(object sender, EventArgs args)
    {
        await Shell.Current.GoToAsync("..");
    }
}