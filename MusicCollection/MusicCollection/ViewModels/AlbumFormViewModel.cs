using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Models;
using MusicCollection.Repositories;

namespace MusicCollection.ViewModels;

public class AlbumFormViewModel : ViewModelBase
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IZanrRepository _zanrRepo;

    private Album? _existujiciAlbum;

    public ObservableCollection<Zanr> Zanry { get; } = new();

    private string _nazev = string.Empty;
    public string Nazev
    {
        get => _nazev;
        set => SetField(ref _nazev, value);
    }

    private string _interpret = string.Empty;
    public string Interpret
    {
        get => _interpret;
        set => SetField(ref _interpret, value);
    }

    private string _rok = DateTime.Now.Year.ToString();
    public string Rok
    {
        get => _rok;
        set => SetField(ref _rok, value);
    }

    private Zanr? _vybranýZanr;
    public Zanr? VybranyZanr
    {
        get => _vybranýZanr;
        set => SetField(ref _vybranýZanr, value);
    }

    private string _popis = string.Empty;
    public string Popis
    {
        get => _popis;
        set => SetField(ref _popis, value);
    }

    private string _chyba = string.Empty;
    public string Chyba
    {
        get => _chyba;
        set => SetField(ref _chyba, value);
    }

    public string Titulek => _existujiciAlbum == null ? "Přidat album" : "Upravit album";

    public Action? OnUlozeno { get; set; }
    public Action? OnZruseno { get; set; }

    public ICommand UlozitCommand { get; }
    public ICommand ZrusitCommand { get; }

    public AlbumFormViewModel(IAlbumRepository albumRepo, IZanrRepository zanrRepo)
    {
        _albumRepo = albumRepo;
        _zanrRepo = zanrRepo;

        UlozitCommand = new AsyncRelayCommand(UlozitAsync);
        ZrusitCommand = new RelayCommand(() => OnZruseno?.Invoke());
    }

    public async Task InicializovatAsync(Album? album = null)
    {
        var zanry = await _zanrRepo.GetAllAsync();
        Zanry.Clear();
        foreach (var z in zanry) Zanry.Add(z);

        _existujiciAlbum = album;
        OnPropertyChanged(nameof(Titulek));

        if (album != null)
        {
            Nazev = album.Nazev;
            Interpret = album.Interpret;
            Rok = album.Rok.ToString();
            Popis = album.Popis ?? string.Empty;
            VybranyZanr = Zanry.FirstOrDefault(z => z.Id == album.ZanrId);
        }
        else
        {
            Nazev = string.Empty;
            Interpret = string.Empty;
            Rok = DateTime.Now.Year.ToString();
            Popis = string.Empty;
            VybranyZanr = Zanry.FirstOrDefault();
        }
    }

    private async Task UlozitAsync()
    {
        if (string.IsNullOrWhiteSpace(Nazev))
        {
            Chyba = "Název alba je povinný.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Interpret))
        {
            Chyba = "Interpret je povinný.";
            return;
        }
        if (!int.TryParse(Rok, out int rok) || rok < 1900 || rok > 2100)
        {
            Chyba = "Rok musí být číslo mezi 1900 a 2100.";
            return;
        }
        if (VybranyZanr == null)
        {
            Chyba = "Vyberte žánr.";
            return;
        }

        try
        {
            if (_existujiciAlbum == null)
            {
                var novy = new Album
                {
                    Nazev = Nazev.Trim(),
                    Interpret = Interpret.Trim(),
                    Rok = rok,
                    ZanrId = VybranyZanr.Id,
                    Popis = string.IsNullOrWhiteSpace(Popis) ? null : Popis.Trim()
                };
                await _albumRepo.CreateAsync(novy);
            }
            else
            {
                _existujiciAlbum.Nazev = Nazev.Trim();
                _existujiciAlbum.Interpret = Interpret.Trim();
                _existujiciAlbum.Rok = rok;
                _existujiciAlbum.ZanrId = VybranyZanr.Id;
                _existujiciAlbum.Popis = string.IsNullOrWhiteSpace(Popis) ? null : Popis.Trim();
                await _albumRepo.UpdateAsync(_existujiciAlbum);
            }

            Chyba = string.Empty;
            OnUlozeno?.Invoke();
        }
        catch (Exception ex)
        {
            Chyba = $"Chyba při ukládání: {ex.Message}";
        }
    }
}
