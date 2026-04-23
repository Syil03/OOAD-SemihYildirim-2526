using System;
using System.Collections.Generic;
using System.Text;
using WpfEscapeGame.Classes;

namespace WpfEscapeGame.Classes
{
    public class Door : Actor
    {
        public bool IsLocked { get; set; } = false;
        public Item? Key { get; set; }
        public Room? ToRoom { get; set; }

        public Door(string name, string desc) : base(name, desc)
        {
        }
    }
}
