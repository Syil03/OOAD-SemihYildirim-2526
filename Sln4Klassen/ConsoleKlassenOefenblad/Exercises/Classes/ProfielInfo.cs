namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    public class ProfielInfo
    {
        // Properties (verplichte info)
        public int Id { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Email { get; set; }
        public DateTime AanmaakDatum { get; private set; }

        // Properties (optionele info)
        public string Voornaam { get; set; } = "";
        public string Achternaam { get; set; } = "";
        public string Biografie { get; set; } = "";
        public string Website { get; set; } = "";
        public bool IsPubliek { get; set; } = true;

        // Berekende properties
        public bool IsVolledig
        {
            get
            {
                return !string.IsNullOrEmpty(Voornaam)
                    && !string.IsNullOrEmpty(Achternaam)
                    && !string.IsNullOrEmpty(Biografie)
                    && !string.IsNullOrEmpty(Website);
            }
        }

        // Verplichte constructor — minimale gegevens om een geldig profiel te maken
            public ProfielInfo(int id, string gebruikersnaam, string email)
            {
                Id = id;
                Gebruikersnaam = gebruikersnaam;
                Email = email;
                AanmaakDatum = DateTime.Now;
            }

        // Uitgebreide constructor — verplichte én optionele gegevens in één keer
            public ProfielInfo(int id, string gebruikersnaam, string email, string voornaam, string achternaam, string biografie, string website, bool isPubliek)
                : this(id, gebruikersnaam, email) // hergebruik van de eerste constructor
            {
                Voornaam = voornaam;
                Achternaam = achternaam;
                Biografie = biografie;
                Website = website;
                IsPubliek = isPubliek;
            }

        // ToString override
        public override string ToString()
        {
            return $"{Gebruikersnaam} — {(IsPubliek ? "publiek" : "privé")}";
        }
    }
}
