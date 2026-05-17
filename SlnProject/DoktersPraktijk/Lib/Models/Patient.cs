using DokterspraktijkLib.Helpers;
using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib.Models
{
    // Klasse die een patiënt in de praktijk vertegenwoordigt
    public class Patient : Persoon
    {
        public string Geslacht { get; set; } = string.Empty;
        public DateTime Geboortedatum { get; set; }
        public Notificatie Notificaties { get; set; }

        // Zoekt een patiënt op in de databank via e-mail en wachtwoord (SHA256)
        // Geeft null terug als de combinatie niet gevonden wordt
        public static Patient? Inloggen(string email, string paswoord)
        {
            string hash = HashPaswoord(paswoord);
            Patient? gevondenPatient = null;
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties " +
                                 "FROM Patient WHERE email = @email AND paswoord = @paswoord";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@email", email);
                        opdracht.Parameters.AddWithValue("@paswoord", hash);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            if (lezer.Read())
                            {
                                gevondenPatient = LeesUitLezer(lezer);
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij inloggen patiënt: " + fout.Message);
            }
            return gevondenPatient;
        }

        // Haalt één patiënt op uit de databank op basis van zijn id
        public static Patient? OphalenOpId(int id)
        {
            Patient? gevondenPatient = null;
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties " +
                                 "FROM Patient WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            if (lezer.Read())
                            {
                                gevondenPatient = LeesUitLezer(lezer);
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen patiënt: " + fout.Message);
            }
            return gevondenPatient;
        }

        // Geeft alle patiënten terug die minstens één afspraak hebben bij de opgegeven dokter
        public static List<Patient> GeefAllePatientenVanDokter(int dokterId)
        {
            List<Patient> patienten = new List<Patient>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT DISTINCT p.id, p.voornaam, p.achternaam, p.geslacht, p.gsm, p.email, " +
                                 "p.paswoord, p.geboortedatum, p.profielfotodata, p.notificaties " +
                                 "FROM Patient p INNER JOIN Afspraak a ON p.id = a.patient_id " +
                                 "WHERE a.dokter_id = @dokterId ORDER BY p.achternaam, p.voornaam";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@dokterId", dokterId);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                patienten.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen patiënten van dokter: " + fout.Message);
            }
            return patienten;
        }

        // Zoekt patiënten waarvan de voor- of achternaam de zoekterm bevat
        public static List<Patient> Zoeken(string zoekterm)
        {
            List<Patient> patienten = new List<Patient>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties " +
                                 "FROM Patient WHERE voornaam LIKE @zoekterm OR achternaam LIKE @zoekterm " +
                                 "ORDER BY achternaam, voornaam";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@zoekterm", "%" + zoekterm + "%");
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                patienten.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij zoeken patiënten: " + fout.Message);
            }
            return patienten;
        }

        // Slaat de patiënt op: INSERT als Id == 0, anders UPDATE
        public void Opslaan()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    if (Id == 0)
                    {
                        string sql = "INSERT INTO Patient (voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties) " +
                                     "VALUES (@voornaam, @achternaam, @geslacht, @gsm, @email, @paswoord, @geboortedatum, @profielfotodata, @notificaties); " +
                                     "SELECT SCOPE_IDENTITY();";
                        using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                        {
                            VoegParametersToe(opdracht);
                            Id = Convert.ToInt32(opdracht.ExecuteScalar());
                        }
                    }
                    else
                    {
                        string sql = "UPDATE Patient SET voornaam = @voornaam, achternaam = @achternaam, geslacht = @geslacht, " +
                                     "gsm = @gsm, email = @email, paswoord = @paswoord, geboortedatum = @geboortedatum, " +
                                     "profielfotodata = @profielfotodata, notificaties = @notificaties WHERE id = @id";
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
                throw new Exception("Fout bij opslaan patiënt: " + fout.Message);
            }
        }

        // Verwijdert de patiënt uit de databank op basis van zijn id
        public void Verwijderen()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "DELETE FROM Patient WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", Id);
                        opdracht.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij verwijderen patiënt: " + fout.Message);
            }
        }

        // Voegt alle SQL-parameters toe op basis van de eigenschappen van dit object
        private void VoegParametersToe(SqlCommand opdracht)
        {
            opdracht.Parameters.AddWithValue("@voornaam", Voornaam);
            opdracht.Parameters.AddWithValue("@achternaam", Achternaam);
            opdracht.Parameters.AddWithValue("@geslacht", Geslacht);
            opdracht.Parameters.AddWithValue("@gsm", Gsm);
            opdracht.Parameters.AddWithValue("@email", Email);
            opdracht.Parameters.AddWithValue("@paswoord", Paswoord);
            opdracht.Parameters.AddWithValue("@geboortedatum", Geboortedatum);
            if (ProfielFotoData != null)
                opdracht.Parameters.AddWithValue("@profielfotodata", ProfielFotoData);
            else
                opdracht.Parameters.AddWithValue("@profielfotodata", DBNull.Value);
            opdracht.Parameters.AddWithValue("@notificaties", (int)Notificaties);
        }

        // Bouwt een Patient-object op vanuit een rij van de SqlDataReader
        private static Patient LeesUitLezer(SqlDataReader lezer)
        {
            Patient patient = new Patient();
            patient.Id = (int)lezer["id"];
            patient.Voornaam = (string)lezer["voornaam"];
            patient.Achternaam = (string)lezer["achternaam"];
            patient.Geslacht = (string)lezer["geslacht"];
            patient.Gsm = (string)lezer["gsm"];
            patient.Email = (string)lezer["email"];
            patient.Paswoord = (string)lezer["paswoord"];
            patient.Geboortedatum = (DateTime)lezer["geboortedatum"];
            patient.Notificaties = (Notificatie)(int)lezer["notificaties"];
            if (lezer["profielfotodata"] != DBNull.Value)
                patient.ProfielFotoData = (byte[])lezer["profielfotodata"];
            return patient;
        }
    }
}
