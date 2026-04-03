using ConsoleOverervingOefenblad.Exercises.Classes.Workout;

namespace ConsoleOverervingOefenblad.Exercises
{
    /// <summary>
    /// Oefening 4 — is en as: type controle en veilige cast bij overerving
    /// </summary>
    internal static class Ex04IsAs
    {
        public static void Run()
        {
            Console.WriteLine("Oefening 4: is en as");
            Console.WriteLine("-------------");

            List<Workout> workouts = new List<Workout>
            {
                new Cardio
                {
                    Naam = "Ochtendrun",
                    Beschrijving = "Rustig tempo door het park",
                    AfstandInKm = 5.2
                },
                new Krachttraining
                {
                    Naam = "Bench press",
                    Beschrijving = "Borstspieren",
                    Gewicht = 60,
                    Reps = 12
                },
                new Stretching
                {
                    Naam = "Rugstretching",
                    Beschrijving = "Na het tillen",
                    LichaamsDeel = LichaamsDeel.Rug
                },
                new Cardio
                {
                    Naam = "Fietstocht",
                    Beschrijving = "Intervaltraining",
                    AfstandInKm = 22.0
                },
                new Krachttraining
                {
                    Naam = "Squat",
                    Beschrijving = "Beenspieren",
                    Gewicht = 80,
                    Reps = 8
                },
                new Stretching
                {
                    Naam = "Nekrol",
                    Beschrijving = "Ontspanning na beeldschermwerk",
                    LichaamsDeel = LichaamsDeel.Nek
                }
            };

            double totaalCardio = 0, totaalKracht = 0, totaalStretch = 0;

            Console.WriteLine("Overzicht workouts:");
            foreach (Workout w in workouts)
            {
                if (w is Cardio c)
                {
                    Console.WriteLine($"[Cardio]         {c.Naam} – {c.AfstandInKm} km");
                    totaalCardio += c.Punten;
                }
                else if (w is Krachttraining k)
                {
                    Console.WriteLine($"[Krachttraining] {k.Naam} – {k.Gewicht} kg x {k.Reps} reps");
                    totaalKracht += k.Punten;
                }
                else if (w is Stretching s)
                {
                    Console.WriteLine($"[Stretching]     {s.Naam} – {s.LichaamsDeel}");
                    totaalStretch += s.Punten;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Totale punten per type:");
            Console.WriteLine($"  Cardio:          {totaalCardio}");
            Console.WriteLine($"  Krachttraining:  {totaalKracht}");
            Console.WriteLine($"  Stretching:      {totaalStretch}");
        }
    }
}