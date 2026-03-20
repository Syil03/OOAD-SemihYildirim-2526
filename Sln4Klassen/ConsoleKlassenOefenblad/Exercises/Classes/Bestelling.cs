namespace ConsoleKlassenOefenblad.Exercises.Classes;

internal class Bestelling
{
    // Properties
    public int BestellingId { get; set; }
    public DateTime Datum { get; set; } = DateTime.Now;
    public string KlantNaam { get; set; }
    public List<Product> Producten { get; set; } = new List<Product>();
    public string Status
    {
        get;
        set
        {
            string[] toegelaten = { "Bezig", "Afgerond", "Geannuleerd" };
            if (!toegelaten.Contains(value)) throw new ArgumentException($"Ongeldige status: {value}");
            field = value;
        }
    } = "Bezig";

    // Berekende properties
    public decimal TotaalBedrag 
    {
        get 
        {
            decimal totaal = 0;
            foreach (Product p in Producten)
            {
                totaal += p.PrijsMetKorting;
            }
            return totaal;
        }
    }

    // ToString override
    public override string ToString()
    {
        return $"#{BestellingId} — {KlantNaam} | {Producten.Count} product(en) | € {TotaalBedrag:F2} | {Status}";
    }
}
