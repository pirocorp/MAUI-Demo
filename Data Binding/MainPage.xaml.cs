namespace DataBinding
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;

        public MainPage()
        {
            this.InitializeComponent();
            this.BindingContext = this;
        }

        public string ButtonText => this.Count switch
        {
            0 => "Click Here",
            1 => "Clicked 1 time",
            _ => $"Clicked {this.Count} times"
        };

        public int Count
        {
            get => this.count;
            set
            {
                this.count = value;

                // Telling the UI that the property has been updated.
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ButtonText));
            }
        }

        private void OnCounterClicked(object sender, EventArgs e) => this.Count++;
    }
}