namespace MVVM_Architecture.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract Task OnNavigatingTo(object? parameter);

    public abstract Task OnNavigatedFrom(bool isForwardNavigation);

    public abstract Task OnNavigatedTo();

    public virtual void RaisePropertyChanged([CallerMemberName] string? property = null)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
