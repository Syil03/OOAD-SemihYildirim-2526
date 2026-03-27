using ConsoleStaticEnumOefenblad.Exercises.Enums;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes;

internal class Bestelling
{
    public string KlantNaam { get; set; }
    public string ProductNaam { get; set; }
    public BestelStatus Status { get; set; }

    public bool KanNogGewijzigdWorden
    {
        get
        {
            return Status == BestelStatus.Nieuw || Status == BestelStatus.InBehandeling;
        }
    }

    public override string ToString()
    {
        string wijzigbaarTekst = KanNogGewijzigdWorden ? "ja" : "nee";

        return $"{ProductNaam} - {Status} - wijzigbaar: {wijzigbaarTekst}";
    }
}