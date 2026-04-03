using ConsoleOverervingOefenblad.Exercises.Classes.Klant;

namespace ConsoleOverervingOefenblad.Exercises;

/// <summary>
/// Oefening 1 — Basisoefening op overerving: ProfessioneleKlant erft over van Klant
/// </summary>
internal static class Ex01Overerving
{
    public static void Run()
    {
        Console.WriteLine("Oefening 1: Overerving");
        Console.WriteLine("-------------");

        List<Klant> klanten = new List<Klant>();
        klanten.Add(new Klant { Naam = "Lotte Peeters", Email = "lotte.peeters@mail.be" });
        klanten.Add(new Klant { Naam = "Youssef El Amrani", Email = "youssef.elamrani@outlook.com" });
        klanten.Add(new Klant { Naam = "Chloé Van den Broeck", Email = "chloe.vdbroeck@gmail.com" });
        klanten.Add(new Klant { Naam = "Milan De Vos", Email = "milan.devos@yahoo.com" });
        // voeg twee instanties van ProfessioneleKlant toe
        // Voeg deze twee instanties toe op de plek van de puntjes:

        klanten.Add(new ProfessioneleKlant
        {
            Naam = "Niels Verhoeven",
            Email = "niels@studio42.be",
            BedrijfsNaam = "Studio 42 BV",
            BtwNummer = "BE0123.456.789"
        });

        klanten.Add(new ProfessioneleKlant
        {
            Naam = "Sofia Dimitriou",
            Email = "sofia@atliernova.eu",
            BedrijfsNaam = "Atelier Nova",
            BtwNummer = "BE0897.654.321"
        });

        Console.WriteLine("Overzicht klanten:");
        foreach (Klant klant in klanten)
        {
            Console.WriteLine($"- {klant}");
        }
    }
}
