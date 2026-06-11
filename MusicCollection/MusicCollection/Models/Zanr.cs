namespace MusicCollection.Models;

public class Zanr
{
    public int Id { get; set; }
    public string Nazev { get; set; } = string.Empty;

    public override string ToString() => Nazev;
}
