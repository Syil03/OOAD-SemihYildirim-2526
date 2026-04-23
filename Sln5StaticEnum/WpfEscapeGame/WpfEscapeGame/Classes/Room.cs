using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

namespace WpfEscapeGame.Classes
{
    public class Room : Actor
    {
        public string? Image { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Door> Doors { get; set; } = new List<Door>();

        public Room(string name, string desc) : base(name, desc)
        {
        }
    }
}
