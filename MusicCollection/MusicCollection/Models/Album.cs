namespace MusicCollection.Models;

public class Album
{
    public int Id { get; set; }
    public string Nazev { get; set; } = string.Empty;
    public string Interpret { get; set; } = string.Empty;
    public int Rok { get; set; }
    public int ZanrId { get; set; }
    public string? Popis { get; set; }

    // Navigation
    public Zanr? Zanr { get; set; }
}
