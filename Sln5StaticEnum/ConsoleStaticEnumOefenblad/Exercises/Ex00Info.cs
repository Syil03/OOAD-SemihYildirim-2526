using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStaticEnumOefenblad.Exercises
{
    internal class Ex00Info
    {
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
=== Oefenblad: Static en Enum ===

Dit oefenblad bevat oefeningen over:
- static members
- static properties
- static methods
- static classes
- enums
- conversies van en naar int en string

Werkwijze:
1. Lees de uitleg commentaar bovenaan de oefening.
2. Maak de gevraagde klasse(s) en enum(s) aan in 'Exercises/Classes/'.
3. Schrijf waar gevraagd de nodige code.
4. Haal waar gevraagd de testcode uit commentaar om je code te testen.
5. Controleer of je output logisch klopt.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
