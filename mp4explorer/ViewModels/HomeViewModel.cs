using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mp4explorer.Models;
using mp4explorer.Readers;
using mp4explorer.Services;

namespace mp4explorer.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly StorageProviderService _storage;
    
    [ObservableProperty]
    private IStorageFile? _selectedFile;
    
    
    public ObservableCollection<Atom> Nodes{ get; } = new();
    
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
            await UpdateSelectedFileAsync(files[0]);
        }
    }

    private async Task UpdateSelectedFileAsync(IStorageFile file)
    {
        SelectedFile = file;

        Nodes.Clear();
        var fs = await file.OpenReadAsync();
        var mp4 = new Mp4AtomReader();
        using (var reader = new BinaryReader(fs))
        {
            var atom = mp4.ReadAtoms(reader);
            Nodes.Add(atom);
        }
        fs.Close();
    }
}