using System;
using System.Collections.Generic;
using System.Text;
using WpfEscapeGame.Classes;

namespace WpfEscapeGame.Classes
{
    public class Door
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; } = false;
        public Item? Key { get; set; }
        public Room? ToRoom { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
