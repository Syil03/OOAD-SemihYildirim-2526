using DokterspraktijkLib.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DokterspraktijkLib.Models
{
    // Klasse die een patiënt in de praktijk vertegenwoordigt
    public class Patient : Persoon
    {
        public int Geslacht { get; set; }
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

        // Geeft alle patiënten in het systeem terug, gesorteerd op achternaam en voornaam.
        // De parameter dokterId wordt bewaard voor compatibiliteit maar niet meer als filter gebruikt:
        // een dokter in de praktijk heeft toegang tot alle patiënten, ook nieuw aangemaakte
        // zonder afspraken. Zo gedraagt deze methode zich identiek aan Zoeken().
        public static List<Patient> GeefAllePatientenVanDokter(int dokterId)
        {
            List<Patient> patienten = new List<Patient>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, " +
                                 "paswoord, geboortedatum, profielfotodata, notificaties " +
                                 "FROM Patient " +
                                 "ORDER BY achternaam, voornaam";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
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
                throw new Exception("Fout bij ophalen patiënten: " + fout.Message);
            }
            return patienten;
        }

        // Zoekt patiënten waarvan de voor- of achternaam de zoekterm bevat.
        // Gebruikt dezelfde basistabel als GeefAllePatientenVanDokter: geen dokterfilter.
        public static List<Patient> Zoeken(string zoekterm)
        {
            List<Patient> patienten = new List<Patient>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, " +
                                 "paswoord, geboortedatum, profielfotodata, notificaties " +
                                 "FROM Patient " +
                                 "WHERE voornaam LIKE @zoekterm OR achternaam LIKE @zoekterm " +
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

        // Verwijdert eerst alle afspraken van de patiënt en daarna de patiënt zelf.
        // Volgorde is verplicht: FK_Afspraak_Patient verbiedt een DELETE op Patient
        // zolang er nog gekoppelde rijen in Afspraak bestaan.
        public void Verwijderen()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();

                    // Stap 1: verwijder alle afspraken van deze patiënt
                    string sqlAfspraken = "DELETE FROM Afspraak WHERE patient_id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sqlAfspraken, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", Id);
                        opdracht.ExecuteNonQuery();
                    }

                    // Stap 2: verwijder de patiënt zelf
                    string sqlPatient = "DELETE FROM Patient WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sqlPatient, verbinding))
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
            // SqlDbType.Image is verplicht: AddWithValue leidt bij DBNull.Value het type
            // af als nvarchar, wat een "type clash: nvarchar incompatible with image" geeft
            SqlParameter fotoParam = new SqlParameter("@profielfotodata", SqlDbType.Image);
            fotoParam.Value = (ProfielFotoData != null) ? (object)ProfielFotoData : DBNull.Value;
            opdracht.Parameters.Add(fotoParam);
            opdracht.Parameters.AddWithValue("@notificaties", (int)Notificaties);
        }

        // Bouwt een Patient-object op vanuit een rij van de SqlDataReader
        // Elke string-kolom wordt eerst op NULL gecontroleerd om een DBNull-cast-fout te vermijden
        private static Patient LeesUitLezer(SqlDataReader lezer)
        {
            Patient patient = new Patient();
            patient.Id = (int)lezer["id"];

            // Voornaam: NULL-veilig uitlezen via kolomindex
            int idxVoornaam = lezer.GetOrdinal("voornaam");
            patient.Voornaam = lezer.IsDBNull(idxVoornaam) ? "" : lezer.GetString(idxVoornaam);

            // Achternaam: NULL-veilig uitlezen via kolomindex
            int idxAchternaam = lezer.GetOrdinal("achternaam");
            patient.Achternaam = lezer.IsDBNull(idxAchternaam) ? "" : lezer.GetString(idxAchternaam);

            // geslacht is een int in de databank; rechtstreeks uitlezen met GetInt32
            patient.Geslacht = lezer.GetInt32(lezer.GetOrdinal("geslacht"));

            // Gsm: NULL-veilig uitlezen via kolomindex (nchar kan NULL zijn)
            int idxGsm = lezer.GetOrdinal("gsm");
            patient.Gsm = lezer.IsDBNull(idxGsm) ? "" : lezer.GetString(idxGsm);

            // Email: NULL-veilig uitlezen via kolomindex
            int idxEmail = lezer.GetOrdinal("email");
            patient.Email = lezer.IsDBNull(idxEmail) ? "" : lezer.GetString(idxEmail);

            // Paswoord: NULL-veilig uitlezen via kolomindex
            int idxPaswoord = lezer.GetOrdinal("paswoord");
            patient.Paswoord = lezer.IsDBNull(idxPaswoord) ? "" : lezer.GetString(idxPaswoord);

            // Geboortedatum: NULL-veilig uitlezen; DateTime.MinValue als fallback
            int idxGeboortedatum = lezer.GetOrdinal("geboortedatum");
            patient.Geboortedatum = lezer.IsDBNull(idxGeboortedatum) ? DateTime.MinValue : lezer.GetDateTime(idxGeboortedatum);

            // notificaties is een int in de databank; omzetten naar het Notificatie-enum
            int idxNotificaties = lezer.GetOrdinal("notificaties");
            patient.Notificaties = lezer.IsDBNull(idxNotificaties) ? Notificatie.Geen : (Notificatie)lezer.GetInt32(idxNotificaties);

            // Profielfoto: alleen uitlezen als de waarde niet NULL is
            if (lezer["profielfotodata"] != DBNull.Value)
                patient.ProfielFotoData = (byte[])lezer["profielfotodata"];

            return patient;
        }
    }
}
