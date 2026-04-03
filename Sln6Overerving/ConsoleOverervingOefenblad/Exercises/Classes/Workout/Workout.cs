 namespace ConsoleOverervingOefenblad.Exercises.Classes.Workout
{
    public abstract class Workout
    {
        public string Naam { get; set; }
        public string Beschrijving { get; set; }

        public abstract double Punten { get; }
    }
}
