namespace ToDo.Maui.Models;

using System.ComponentModel;

public class ToDo : INotifyPropertyChanged
{
    private int id;
    private string toDoName = string.Empty;

    public int Id
    {
        get => this.id;
        set
        {
            if (this.id == value)
            {
                return;
            }

            this.id = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Id)));
        }
    }

    public string ToDoName
    {
        get => this.toDoName;
        set
        {
            if (this.toDoName == value)
            {
                return;
            }

            this.toDoName = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ToDoName)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
