using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Models;
using MusicCollection.Repositories;

namespace MusicCollection.ViewModels;

public class AlbumListViewModel : ViewModelBase
{
    private readonly IAlbumRepository _albumRepo;

    public ObservableCollection<Album> Albums { get; } = new();

    private Album? _selectedAlbum;
    public Album? SelectedAlbum
    {
        get => _selectedAlbum;
        set => SetField(ref _selectedAlbum, value);
    }

    private string _chyba = string.Empty;
    public string Chyba
    {
        get => _chyba;
        set => SetField(ref _chyba, value);
    }

    // Navigation callbacks — nastaví MainWindow
    public Action? OnPridatAlbum { get; set; }
    public Action<Album>? OnDetailAlbum { get; set; }
    public Action<Album>? OnUpravitAlbum { get; set; }

    public ICommand NacistCommand { get; }
    public ICommand PridatCommand { get; }
    public ICommand DetailCommand { get; }
    public ICommand UpravitCommand { get; }
    public ICommand SmazatCommand { get; }

    public AlbumListViewModel(IAlbumRepository albumRepo)
    {
        _albumRepo = albumRepo;

        NacistCommand  = new AsyncRelayCommand(NacistAsync);
        PridatCommand  = new RelayCommand(() => OnPridatAlbum?.Invoke());
        DetailCommand  = new RelayCommand<Album>(a => OnDetailAlbum?.Invoke(a!), a => a != null);
        UpravitCommand = new RelayCommand<Album>(a => OnUpravitAlbum?.Invoke(a!), a => a != null);
        SmazatCommand  = new AsyncRelayCommand<Album>(SmazatAsync);
    }

    public async Task NacistAsync()
    {
        try
        {
            Chyba = string.Empty;
            var data = await _albumRepo.GetAllAsync();
            Albums.Clear();
            foreach (var a in data) Albums.Add(a);
        }
        catch (Exception ex)
        {
            Chyba = $"Chyba při načítání: {ex.Message}";
        }
    }

    private async Task SmazatAsync(Album? album)
    {
        if (album == null) return;
        try
        {
            await _albumRepo.DeleteAsync(album.Id);
            Albums.Remove(album);
        }
        catch (Exception ex)
        {
            Chyba = $"Nelze smazat album: {ex.Message}";
        }
    }
}
