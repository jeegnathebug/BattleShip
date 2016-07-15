using System.Collections.Generic;
using System.Windows.Controls;

namespace BattleShip
{
    /// <summary>
    /// Defines a ship type
    /// </summary>
    public class Ship
    {
        public readonly ShipName name;
        public readonly int size;
        public Orientation orientation;
        public int hits = 0;
        public bool sunk = false;
        public Image image;
        public ListBoxItem item;

        public List<int> location;
        public bool placed = false;

        public Ship(ShipName name, int size)
        {
            this.name = name;
            this.size = size;

            location = new List<int>(size);
        }
    }
}
