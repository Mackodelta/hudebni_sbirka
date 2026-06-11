using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Models;
using MusicCollection.Repositories;

namespace MusicCollection.ViewModels;

public class AlbumDetailViewModel : ViewModelBase
{
    private readonly ISkladbaRepository _skladbaRepo;

    private Album? _album;
    public Album? Album
    {
        get => _album;
        set { SetField(ref _album, value); }
    }

    public ObservableCollection<Skladba> Skladby { get; } = new();

    // Formulář pro novou/editovanou skladbu
    private Skladba? _editovanaSkladba;
    public Skladba? EditovanaSkladba
    {
        get => _editovanaSkladba;
        set => SetField(ref _editovanaSkladba, value);
    }

    private string _novaNazev = string.Empty;
    public string NovaNazev
    {
        get => _novaNazev;
        set => SetField(ref _novaNazev, value);
    }

    private string _novaDelka = string.Empty;
    public string NovaDelka
    {
        get => _novaDelka;
        set => SetField(ref _novaDelka, value);
    }

    private string _novaCislo = string.Empty;
    public string NovaCislo
    {
        get => _novaCislo;
        set => SetField(ref _novaCislo, value);
    }

    private string _chyba = string.Empty;
    public string Chyba
    {
        get => _chyba;
        set => SetField(ref _chyba, value);
    }

    private bool _editMode;
    public bool EditMode
    {
        get => _editMode;
        set
        {
            SetField(ref _editMode, value);
            OnPropertyChanged(nameof(FormularTitulek));
            OnPropertyChanged(nameof(UlozitText));
        }
    }

    public string FormularTitulek => EditMode ? "Upravit skladbu" : "Přidat skladbu";
    public string UlozitText => EditMode ? "Uložit změny" : "Přidat skladbu";

    public Action? OnZpet { get; set; }

    public ICommand ZpetCommand { get; }
    public ICommand UlozitSkladbuCommand { get; }
    public ICommand EditovatSkladbuCommand { get; }
    public ICommand SmazatSkladbuCommand { get; }
    public ICommand ZrusitEditCommand { get; }

    public AlbumDetailViewModel(ISkladbaRepository skladbaRepo)
    {
        _skladbaRepo = skladbaRepo;

        ZpetCommand            = new RelayCommand(() => OnZpet?.Invoke());
        UlozitSkladbuCommand   = new AsyncRelayCommand(UlozitSkladbuAsync);
        EditovatSkladbuCommand = new RelayCommand<Skladba>(ZacitEditovat);
        SmazatSkladbuCommand   = new AsyncRelayCommand<Skladba>(SmazatSkladbuAsync);
        ZrusitEditCommand      = new RelayCommand(ZrusitEdit);
    }

    public async Task NacistSkladbyAsync()
    {
        if (Album == null) return;
        var data = await _skladbaRepo.GetByAlbumIdAsync(Album.Id);
        Skladby.Clear();
        foreach (var s in data) Skladby.Add(s);
    }

    private void ZacitEditovat(Skladba? s)
    {
        if (s == null) return;
        EditovanaSkladba = s;
        NovaNazev = s.Nazev;
        NovaDelka = s.DelkaSekund.ToString();
        NovaCislo = s.CisloStopy.ToString();
        EditMode = true;
        Chyba = string.Empty;
    }

    private void ZrusitEdit()
    {
        EditovanaSkladba = null;
        NovaNazev = string.Empty;
        NovaDelka = string.Empty;
        NovaCislo = string.Empty;
        EditMode = false;
        Chyba = string.Empty;
    }

    private async Task UlozitSkladbuAsync()
    {
        if (Album == null) return;

        if (string.IsNullOrWhiteSpace(NovaNazev))
        {
            Chyba = "Název skladby je povinný.";
            return;
        }
        if (!int.TryParse(NovaDelka, out int delka) || delka <= 0)
        {
            Chyba = "Délka musí být kladné celé číslo (sekundy).";
            return;
        }
        if (!int.TryParse(NovaCislo, out int cislo) || cislo <= 0)
        {
            Chyba = "Číslo stopy musí být kladné celé číslo.";
            return;
        }

        try
        {
            if (EditMode && EditovanaSkladba != null)
            {
                EditovanaSkladba.Nazev = NovaNazev.Trim();
                EditovanaSkladba.DelkaSekund = delka;
                EditovanaSkladba.CisloStopy = cislo;
                await _skladbaRepo.UpdateAsync(EditovanaSkladba);
            }
            else
            {
                var nova = new Skladba
                {
                    AlbumId = Album.Id,
                    Nazev = NovaNazev.Trim(),
                    DelkaSekund = delka,
                    CisloStopy = cislo
                };
                await _skladbaRepo.CreateAsync(nova);
            }

            ZrusitEdit();
            await NacistSkladbyAsync();
            Chyba = string.Empty;
        }
        catch (Exception ex)
        {
            Chyba = $"Chyba při ukládání: {ex.Message}";
        }
    }

    private async Task SmazatSkladbuAsync(Skladba? s)
    {
        if (s == null) return;
        try
        {
            await _skladbaRepo.DeleteAsync(s.Id);
            Skladby.Remove(s);
        }
        catch (Exception ex)
        {
            Chyba = $"Chyba při mazání: {ex.Message}";
        }
    }
}
