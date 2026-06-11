using System;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Repositories;
using MusicCollection.ViewModels;

namespace MusicCollection;

public static class Services
{
    public static ServiceProvider BuildServiceProvider()
    {
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env" }, probeForEnv: true, probeLevelsToSearch: 5));
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var db   = Environment.GetEnvironmentVariable("DB_NAME") ?? "music_collection";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "music_user";
        var pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

        var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

        var services = new ServiceCollection();

        // Repositories
        services.AddSingleton<IAlbumRepository>(_ => new AlbumRepository(connectionString));
        services.AddSingleton<ISkladbaRepository>(_ => new SkladbaRepository(connectionString));
        services.AddSingleton<IZanrRepository>(_ => new ZanrRepository(connectionString));

        // ViewModels
        services.AddTransient<AlbumListViewModel>();
        services.AddTransient<AlbumDetailViewModel>();
        services.AddTransient<AlbumFormViewModel>();

        return services.BuildServiceProvider();
    }
}