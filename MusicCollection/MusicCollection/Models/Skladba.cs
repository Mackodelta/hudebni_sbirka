namespace MusicCollection.Models;

public class Skladba
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public string Nazev { get; set; } = string.Empty;
    public int DelkaSekund { get; set; }
    public int CisloStopy { get; set; }

    public string DelkaFormatovana =>
        DelkaSekund >= 3600
            ? $"{DelkaSekund / 3600}:{(DelkaSekund % 3600) / 60:D2}:{DelkaSekund % 60:D2}"
            : $"{DelkaSekund / 60}:{DelkaSekund % 60:D2}";
}
