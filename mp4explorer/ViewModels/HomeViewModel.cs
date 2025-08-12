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
            await UpdateSelectedFileAsync(files[0]);
        }
    }

    private async Task UpdateSelectedFileAsync(IStorageFile file)
    {
        SelectedFile = file;

        var root = new Node("root");
        var fs = await file.OpenReadAsync();
        using (var reader = new BinaryReader(fs))
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            ReadAtoms(root, reader);

            
            
        }
        
        fs.Close();
        
        Nodes.Add(root);
        /*
        var item = new Node("moov");
        item.Children.Add(new Node("faac"));
        Nodes.Add(item);
        */
    }

    private static void ReadAtoms(Node root, BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var atomSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            var atomTypeBytes = reader.ReadBytes(4);
            var atomSize = BitConverter.ToInt32(atomSizeBytes, 0);
            var atomType = Encoding.Default.GetString(atomTypeBytes);
            root.Children.Add(new Node($"{atomType} ({atomSize})"));
            reader.BaseStream.Seek(atomSize - 8,SeekOrigin.Current);
        }

    }
}