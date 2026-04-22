using System;
using System.Collections.Generic;
using System.Text;

namespace WpfEscapeGame.Classes
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; } = false;
        public Item? Key { get; set; }
        public Item? HiddenItem { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }

        public bool IsPortable { get; set; } = true;
    }
}