namespace ToDo.Maui
{
    using ToDo.Maui.Pages;

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            this.InitializeComponent();

            Routing.RegisterRoute(nameof(ManageToDoPage), typeof(ManageToDoPage));
        }
    }
}