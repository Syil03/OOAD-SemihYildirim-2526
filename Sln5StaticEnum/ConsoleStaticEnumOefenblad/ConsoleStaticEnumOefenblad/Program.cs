using ConsoleStaticEnumOefenblad.Exercises;
using System.Runtime.InteropServices;

namespace ConsoleStaticEnumOefenblad
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // voor euroteken
            bool stoppen = false;
            while (!stoppen)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kies een oefening om uit te voeren:");
                Console.WriteLine();
                Console.WriteLine("0 - Lees instructies");
                Console.WriteLine("1 - Deelnemers registreren");
                Console.WriteLine("2 - Couponcodes controleren");
                Console.WriteLine("3 - Tekst analyseren");
                Console.WriteLine("4 - Bestelstatus");
                Console.WriteLine("5 - Enum conversies");
                Console.WriteLine();
                Console.Write("Je keuze (enter om te stoppen): ");
                char choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();

                switch (choice)
                {
                    case '0': Ex00Info.Run(); break;
                    case '1': Ex01StaticProperty.Run(); break;
                    case '2': Ex02StaticMethode.Run(); break;
                    case '3': Ex03StaticClass.Run(); break;
                    case '4': Ex04EersteEnum.Run(); break;
                    case '5': Ex05EnumConversie.Run(); break;
                    default: stoppen = true; break;
                }
                
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("druk een toets om verder te gaan...");
                Console.ReadKey();
            }
        }
    }
}
