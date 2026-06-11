using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Models;
using MusicCollection.ViewModels;

namespace MusicCollection.Views;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _sp;

    public MainWindow(IServiceProvider sp)

    {
        _sp = sp;
        InitializeComponent();
        ZobrazitSeznam();
    }

    private void ZobrazitSeznam()
    {
        var vm = _sp.GetRequiredService<AlbumListViewModel>();
        vm.OnPridatAlbum    = () => ZobrazitFormular(null);
        vm.OnDetailAlbum    = ZobrazitDetail;
        vm.OnUpravitAlbum   = a => ZobrazitFormular(a);

        var view = new AlbumListView { DataContext = vm };
        MainContent.Content = view;

        _ = vm.NacistAsync();
    }

    private void ZobrazitDetail(Album album)
    {
        var vm = _sp.GetRequiredService<AlbumDetailViewModel>();
        vm.Album = album;
        vm.OnZpet = ZobrazitSeznam;

        var view = new AlbumDetailView { DataContext = vm };
        MainContent.Content = view;

        _ = vm.NacistSkladbyAsync();
    }

    private void ZobrazitFormular(Album? album)
    {
        var vm = _sp.GetRequiredService<AlbumFormViewModel>();
        vm.OnUlozeno = ZobrazitSeznam;
        vm.OnZruseno = ZobrazitSeznam;

        var view = new AlbumFormView { DataContext = vm };
        MainContent.Content = view;

        _ = vm.InicializovatAsync(album);
    }
}
