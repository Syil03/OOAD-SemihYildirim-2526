using DokterspraktijkLib.Helpers;
using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib.Models
{
    // Klasse die een afspraak tussen een patiënt en een dokter vertegenwoordigt
    public class Afspraak
    {
        public int Id { get; set; }
        public DateTime Moment { get; set; }
        public string Klacht { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int DokterId { get; set; }

        // Niet opgeslagen in de databank — enkel voor weergave in de UI
        public string PatientNaam { get; set; } = string.Empty;
        public string DokterNaam { get; set; } = string.Empty;

        // Geeft alle afspraken van één patiënt terug, nieuwste eerst
        public static List<Afspraak> GeefAfsprakenVanPatient(int patientId)
        {
            List<Afspraak> afspraken = new List<Afspraak>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id, " +
                                 "p.voornaam + ' ' + p.achternaam AS patientnaam, " +
                                 "d.voornaam + ' ' + d.achternaam AS dokternaam " +
                                 "FROM Afspraak a " +
                                 "INNER JOIN Patient p ON a.patient_id = p.id " +
                                 "INNER JOIN Dokter d ON a.dokter_id = d.id " +
                                 "WHERE a.patient_id = @patientId ORDER BY a.moment DESC";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@patientId", patientId);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                afspraken.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen afspraken van patiënt: " + fout.Message);
            }
            return afspraken;
        }

        // Geeft alle afspraken van één dokter terug, nieuwste eerst
        public static List<Afspraak> GeefAfsprakenVanDokter(int dokterId)
        {
            List<Afspraak> afspraken = new List<Afspraak>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id, " +
                                 "p.voornaam + ' ' + p.achternaam AS patientnaam, " +
                                 "d.voornaam + ' ' + d.achternaam AS dokternaam " +
                                 "FROM Afspraak a " +
                                 "INNER JOIN Patient p ON a.patient_id = p.id " +
                                 "INNER JOIN Dokter d ON a.dokter_id = d.id " +
                                 "WHERE a.dokter_id = @dokterId ORDER BY a.moment DESC";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@dokterId", dokterId);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                afspraken.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen afspraken van dokter: " + fout.Message);
            }
            return afspraken;
        }

        // Geeft enkel de toekomstige afspraken van een dokter terug, vroegste eerst
        public static List<Afspraak> GeefToekomstigeAfspraken(int dokterId)
        {
            List<Afspraak> afspraken = new List<Afspraak>();
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id, " +
                                 "p.voornaam + ' ' + p.achternaam AS patientnaam, " +
                                 "d.voornaam + ' ' + d.achternaam AS dokternaam " +
                                 "FROM Afspraak a " +
                                 "INNER JOIN Patient p ON a.patient_id = p.id " +
                                 "INNER JOIN Dokter d ON a.dokter_id = d.id " +
                                 "WHERE a.dokter_id = @dokterId AND a.moment >= GETDATE() ORDER BY a.moment ASC";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@dokterId", dokterId);
                        using (SqlDataReader lezer = opdracht.ExecuteReader())
                        {
                            while (lezer.Read())
                            {
                                afspraken.Add(LeesUitLezer(lezer));
                            }
                        }
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij ophalen toekomstige afspraken: " + fout.Message);
            }
            return afspraken;
        }

        // Voegt een nieuwe afspraak toe aan de databank op basis van de opgegeven gegevens
        public static void AfspraakToevoegen(int patientId, int dokterId, DateTime moment, string klacht)
        {
            Afspraak nieuweAfspraak = new Afspraak();
            nieuweAfspraak.PatientId = patientId;
            nieuweAfspraak.DokterId = dokterId;
            nieuweAfspraak.Moment = moment;
            nieuweAfspraak.Klacht = klacht;
            // Id is 0, dus Opslaan() voert een INSERT uit
            nieuweAfspraak.Opslaan();
        }

        // Slaat de afspraak op: INSERT als Id == 0, anders UPDATE
        public void Opslaan()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    if (Id == 0)
                    {
                        string sql = "INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id) " +
                                     "VALUES (@moment, @klacht, @patientId, @dokterId); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                        {
                            VoegParametersToe(opdracht);
                            Id = Convert.ToInt32(opdracht.ExecuteScalar());
                        }
                    }
                    else
                    {
                        string sql = "UPDATE Afspraak SET moment = @moment, klacht = @klacht, " +
                                     "patient_id = @patientId, dokter_id = @dokterId WHERE id = @id";
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
                throw new Exception("Fout bij opslaan afspraak: " + fout.Message);
            }
        }

        // Verwijdert de afspraak uit de databank op basis van zijn id
        public void Verwijderen()
        {
            try
            {
                using (SqlConnection verbinding = DatabaseHelper.GetConnection())
                {
                    verbinding.Open();
                    string sql = "DELETE FROM Afspraak WHERE id = @id";
                    using (SqlCommand opdracht = new SqlCommand(sql, verbinding))
                    {
                        opdracht.Parameters.AddWithValue("@id", Id);
                        opdracht.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception fout)
            {
                throw new Exception("Fout bij verwijderen afspraak: " + fout.Message);
            }
        }

        // Voegt alle SQL-parameters toe op basis van de eigenschappen van dit object
        private void VoegParametersToe(SqlCommand opdracht)
        {
            opdracht.Parameters.AddWithValue("@moment", Moment);
            opdracht.Parameters.AddWithValue("@klacht", Klacht);
            opdracht.Parameters.AddWithValue("@patientId", PatientId);
            opdracht.Parameters.AddWithValue("@dokterId", DokterId);
        }

        // Bouwt een Afspraak-object op vanuit een rij van de SqlDataReader
        private static Afspraak LeesUitLezer(SqlDataReader lezer)
        {
            Afspraak afspraak = new Afspraak();
            afspraak.Id = (int)lezer["id"];
            afspraak.Moment = (DateTime)lezer["moment"];
            afspraak.Klacht = (string)lezer["klacht"];
            afspraak.PatientId = (int)lezer["patient_id"];
            afspraak.DokterId = (int)lezer["dokter_id"];
            afspraak.PatientNaam = (string)lezer["patientnaam"];
            afspraak.DokterNaam = (string)lezer["dokternaam"];
            return afspraak;
        }
    }
}
