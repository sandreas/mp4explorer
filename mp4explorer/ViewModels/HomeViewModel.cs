using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mp4explorer.Models;
using mp4explorer.Services;

namespace mp4explorer.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly StorageProviderService _storage;
    
    [ObservableProperty]
    private IStorageFile? _selectedFile;
    
    
    public ObservableCollection<Node> Nodes{ get; } = new();
    
    public HomeViewModel(StorageProviderService storage)
    {
        _storage = storage;
    }

    [RelayCommand]
    private async Task PickFile()
    {
        if (_storage.StorageProvider == null)
        {
            return;
        }
        var files = await _storage.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            UpdateSelectedFileAsync(files[0]);
        }
    }

    private async Task UpdateSelectedFileAsync(IStorageFile file)
    {
        SelectedFile = file;

        var fs = await file.OpenReadAsync();
        fs.Close();
        
        var item = new Node("moov");
        item.Children.Add(new Node("faac"));
        Nodes.Add(item);
    }
}