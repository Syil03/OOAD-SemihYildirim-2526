using DokterspraktijkLib.Helpers;
using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib.Models
{
    // Klasse die een arts in de praktijk vertegenwoordigt
    public class Dokter : Persoon
    {
        public string RizivNummer { get; set; } = string.Empty;
        public bool IsGeconventioneerd { get; set; }

        // Zoekt een dokter op in de databank via e-mail en wachtwoord (SHA256)
        // Geeft null terug als de combinatie niet gevonden wordt
        public static Dokter? Inloggen(string email, string paswoord)
        {
            string hash = HashPaswoord(paswoord);
            Dokter? gevondenDokter = null;
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd " +
                                 "FROM Dokter WHERE email = @email AND paswoord = @paswoord";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@email", email);
                        opdracht.Parameters.AddWithValue("@paswoord", hash);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            if (lezer.Read())
                            {
                                gevondenDokter = LeesUitLezer(lezer);
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij inloggen dokter: " + fout.Message);
            }
            return gevondenDokter;
        }

        // Haalt één dokter op uit de databank op basis van zijn id
        public static Dokter? OphalenOpId(int id)
        {
            Dokter? gevondenDokter = null;
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd " +
                                 "FROM Dokter WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            if (lezer.Read())
                            {
                                gevondenDokter = LeesUitLezer(lezer);
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen dokter: " + fout.Message);
            }
            return gevondenDokter;
        }

        // Geeft een gesorteerde lijst van alle dokters in de databank terug
        public static List<Dokter> GeefAlleDokters()
        {
            List<Dokter> dokters = new List<Dokter>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd " +
                                 "FROM Dokter ORDER BY achternaam, voornaam";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                dokters.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen dokters: " + fout.Message);
            }
            return dokters;
        }

        // Slaat de dokter op: INSERT als Id == 0, anders UPDATE
        public void Opslaan()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    if (Id == 0)
                    {
                        string sql = "INSERT INTO Dokter (voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd) " +
                                     "VALUES (@voornaam, @achternaam, @gsm, @email, @paswoord, @profielfotodata, @rizivnummer, @isgeconventioneerd); " +
                                     "SELECT SCOPE_IDENTITY();";
                        using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                        {
                            VoegParametersToe(opdracht);
                            Id = Convert.ToInt32(opdracht.ExecuteScalar());
                        }
                    }
                    else
                    {
                        string sql = "UPDATE Dokter SET voornaam = @voornaam, achternaam = @achternaam, gsm = @gsm, " +
                                     "email = @email, paswoord = @paswoord, profielfotodata = @profielfotodata, " +
                                     "rizivnummer = @rizivnummer, isgeconventioneerd = @isgeconventioneerd WHERE id = @id";
                        using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                        {
                            VoegParametersToe(opdracht);
                            opdracht.Parameters.AddWithValue("@id", Id);
                            opdracht.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij opslaan dokter: " + fout.Message);
            }
        }

        // Verwijdert de dokter uit de databank op basis van zijn id
        public void Verwijderen()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "DELETE FROM Dokter WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", Id);
                        opdracht.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij verwijderen dokter: " + fout.Message);
            }
        }

        // Voegt alle SQL-parameters toe op basis van de eigenschappen van dit object
        private void VoegParametersToe(SqlCommand opdracht)
        {
            opdracht.Parameters.AddWithValue("@voornaam", Voornaam);
            opdracht.Parameters.AddWithValue("@achternaam", Achternaam);
            opdracht.Parameters.AddWithValue("@gsm", Gsm);
            opdracht.Parameters.AddWithValue("@email", Email);
            opdracht.Parameters.AddWithValue("@paswoord", Paswoord);
            if (ProfielFotoData != null)
                opdracht.Parameters.AddWithValue("@profielfotodata", ProfielFotoData);
            else
                opdracht.Parameters.AddWithValue("@profielfotodata", DBNull.Value);
            opdracht.Parameters.AddWithValue("@rizivnummer", RizivNummer);
            opdracht.Parameters.AddWithValue("@isgeconventioneerd", IsGeconventioneerd);
        }

        // Bouwt een Dokter-object op vanuit een rij van de SqlDataReader
        private static Dokter LeesUitLezer(SqlDataReader lezer)
        {
            Dokter dokter = new Dokter();
            dokter.Id = (int)lezer["id"];
            dokter.Voornaam = (string)lezer["voornaam"];
            dokter.Achternaam = (string)lezer["achternaam"];
            dokter.Gsm = (string)lezer["gsm"];
            dokter.Email = (string)lezer["email"];
            dokter.Paswoord = (string)lezer["paswoord"];
            // rizivnummer is een int in de databank; uitlezen met GetInt32 en omzetten naar string
            dokter.RizivNummer = lezer.GetInt32(lezer.GetOrdinal("rizivnummer")).ToString();
            // isgeconventioneerd is een tinyint (byte); uitlezen met GetByte en omzetten naar bool
            dokter.IsGeconventioneerd = lezer.GetByte(lezer.GetOrdinal("isgeconventioneerd")) != 0;
            if (lezer["profielfotodata"] != DBNull.Value)
                dokter.ProfielFotoData = (byte[])lezer["profielfotodata"];
            return dokter;
        }
    }
}
