using CommunityToolkit.Mvvm.ComponentModel;

namespace mp4explorer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
