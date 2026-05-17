using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib.Models
{
    // Abstracte basisklasse voor alle personen in het systeem
    public abstract class Persoon
    {
        public int Id { get; set; }
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string Gsm { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Paswoord { get; set; } = string.Empty;
        public byte[]? ProfielFotoData { get; set; }

        // Geeft de volledige naam terug (voornaam + spatie + achternaam)
        public string GeefVolledigeNaam()
        {
            return Voornaam + " " + Achternaam;
        }

        // Zet een plaintext wachtwoord om naar een SHA256-hashstring in lowercase hex
        public static string HashPaswoord(string paswoord)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(paswoord));
                StringBuilder bouwer = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    bouwer.Append(bytes[i].ToString("x2"));
                }
                return bouwer.ToString();
            }
        }
    }
}
