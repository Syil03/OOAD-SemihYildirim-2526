namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel
{
    public abstract class ValidatieRegel
    {
        public abstract string FoutBoodschap { get; }

        public abstract bool IsGeldig(string waarde);
    }
}