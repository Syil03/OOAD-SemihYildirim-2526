namespace ConsoleKlassenOefenblad.Exercises.Classes;

internal class Product
{
    // Properties
    public int ProductId { get; set; }
    public string Naam { get; set; }
    public string Beschrijving { get; set; }
    public decimal Prijs { get; set; }
    public int Voorraad { get; set; }
    public bool IsInVoorraad { get { return Voorraad > 0; } }
    public double Korting
    {
        get;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("Percentage moet tussen 0 en 100 liggen.");
            }
            field = value;
        }
    } = 0;

    // Berekende properties
    public decimal PrijsMetKorting 
    { 
        get 
        { 
            return Prijs * (1 - ((decimal) Korting) / 100); 
        } 
    }

    // Override ToString() voor mooie weergave van een product
    public override string ToString()
    {
        return $"{Naam} (ID: {ProductId}) - {Beschrijving} - Prijs: €{Prijs:F2} (Korting: {Korting}%, Prijs met korting: €{PrijsMetKorting:F2}) - Voorraad: {(IsInVoorraad ? Voorraad.ToString() : "Niet in voorraad")}";
    }

    public void GeefKorting(double percentage)
    {
        if (percentage < 0 || percentage > 100) throw new ArgumentOutOfRangeException(nameof(percentage));
        Korting = percentage;
    }
}
