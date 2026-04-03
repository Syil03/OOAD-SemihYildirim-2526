namespace ConsoleOverervingOefenblad.Exercises.Classes.Workout;

internal class Krachttraining : Workout
{
    public double Gewicht { get; set; }
    public int Reps { get; set; }

    public override double Punten => (Reps * Gewicht) / 5.0;
}
