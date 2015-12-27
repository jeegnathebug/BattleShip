using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{

    /// <summary>
    /// Defines a ship type
    /// </summary>
    public partial class Ship
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

    public enum ShipName
    {
        AIRCRAFT_CARRIER, BATTLESHIP, SUBMARINE, CRUISER, DESTROYER
    }

    public enum Orientation
    {
        HORIZONTAL, VERTICAL
    }

    /// <summary>
    /// Interaction logic for Shipyard.xaml
    /// </summary>
    public partial class Shipyard : UserControl
    {
        public event EventHandler play;
        private Common common;

        private Orientation orientation = Orientation.HORIZONTAL;
        public Button[] buttons;

        // List of players actions
        List<Ship> actions = new List<Ship>(5);

        // Ships
        public Ship[] ships;
        private Ship aircraft_carrier = new Ship(ShipName.AIRCRAFT_CARRIER, 5);
        private Ship battleship = new Ship(ShipName.BATTLESHIP, 4);
        private Ship submarine = new Ship(ShipName.SUBMARINE, 3);
        private Ship cruiser = new Ship(ShipName.CRUISER, 3);
        private Ship destroyer = new Ship(ShipName.DESTROYER, 2);

        // Currently selected ship
        private Ship ship;

        /// <summary>
        /// Initializes setup phase
        /// </summary>
        public Shipyard()
        {
            InitializeComponent();

            // Initialize ship array
            ships = new Ship[] { aircraft_carrier, battleship, submarine, cruiser, destroyer };

            // Image array
            Image[] images = { aircraft_carrierImage, battleshipImage, submarineImage, cruiserImage, destroyerImage };
            // ListBoxItem array
            ListBoxItem[] listBoxItems = { Aircraft_Carrier, Battleship, Submarine, Cruiser, Destroyer };

            // Set ships' ListBoxItem's, images and ListBoxItems' Tags
            for (int i = 0; i < ships.Length; i++)
            {
                ships[i].item = listBoxItems[i];
                ships[i].image = images[i];
                listBoxItems[i].Tag = ships[i];
            }

            // Initialize player button field
            buttons = new Button[100];
            gameField.Children.CopyTo(buttons, 0);

            common = new Common(gameField, buttons);

            // Set currently selected ship
            ship = aircraft_carrier;
        }

        /// <summary>
        /// Uses sender as the button chosen for ship placement
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void button_Clicked(object sender, EventArgs e)
        {
            if (nullCheck())
            {
                Button chosen = (Button)sender;
                int index = -1;

                // Find index of chosen button in array
                index = Array.IndexOf(buttons, chosen);

                bool isPlaced = common.setShip(ship, index, orientation);

                if (isPlaced)
                {
                    // Add to list of actions
                    actions.Add(ship);
                    Undo.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Randomizes the ship placement
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonRandomize_Click(object sender, EventArgs e)
        {
            reset();
            Random random = new Random();

            for (int i = 0; i < ships.Length; i++)
            {
                do
                {
                    common.setShip(ships[i], random.Next(0, 100), common.randomOrientation(i), true);
                } while (!ships[i].placed);
            }
        }

        /// <summary>
        /// Resets the screen
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonReset_Click(object sender, EventArgs e)
        {
            reset();
        }

        /// <summary>
        /// Changes the orientation of the placement
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            changeOrientation();
        }

        /// <summary>
        /// Submits the ship placement. Shows an error message if all ships have not been placed
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (!(aircraft_carrier.placed && battleship.placed && submarine.placed && cruiser.placed && destroyer.placed))
            {
                MessageBox.Show("All ships must be placed before proceeding", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (play != null)
                {
                    play(this, e);
                }
            }
        }

        /// <summary>
        /// Undoes the last boat placed. Can be used until there are no boats left on field
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonUndo_Click(object sender, EventArgs e)
        {
            Ship ship = actions[actions.Count - 1];

            // Reset chosen buttons
            for (int i = 0; i < ship.size; i++)
            {
                buttons[ship.location[i]].Tag = null;
                buttons[ship.location[i]].Opacity = 100;
                buttons[ship.location[i]].IsEnabled = true;
            }

            // Reset ship
            ship.placed = false;
            ship.location = new List<int>();

            // Reset image
            gameField.Children.RemoveRange(gameField.Children.Count - 2, gameField.Children.Count - 1);

            // Reset ListItemBox
            ship.item.IsEnabled = true;

            // Remove action
            actions.RemoveAt(actions.Count - 1);

            // Hide undo button if no actions remain
            if (actions.Count == 0)
            {
                Undo.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Changes orientation of ship placement
        /// </summary>
        private void changeOrientation()
        {
            int i = (int)orientation;
            i++;
            i %= 2;

            orientation = (Orientation)i;
            labelOrientation.Content = "ORIENTATION: " + orientation;
        }

        /// <summary>
        /// Chooses a boat, and updates labels
        /// </summary>
        /// <param name="sender">The ListBoxItem</param>
        /// <param name="e">The Event</param>
        private void ListBoxItem_Selected(object sender, EventArgs e)
        {
            ListBoxItem selected = (ListBoxItem)sender;

            ship = (Ship)selected.Tag;

            // Change labels
            labelSize.Content = "SIZE: " + ship.size;
            labelBoat.Content = ship.name.ToString().Replace('_', ' ');
        }

        /// <summary>
        /// Checks if a ship has been selected from the list box
        /// </summary>
        /// <returns>True if a ship has been selected, false otherwise</returns>
        private bool nullCheck()
        {
            if (!ship.item.IsEnabled)
            {
                MessageBox.Show("You must first select a ship", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Resets the screen
        /// </summary>
        private void reset()
        {
            Undo.Visibility = Visibility.Hidden;
            actions.Clear();

            // Image array
            Image[] images = { aircraft_carrierImage, battleshipImage, submarineImage, cruiserImage, destroyerImage };

            // Enable list box items, unset placed ships, reset ship images
            for (int i = 0; i < ships.Length; i++)
            {
                ships[i].item.IsEnabled = true;
                ships[i].placed = false;
                ships[i].image = images[i];
                ships[i].location = new List<int>();
            }

            // Remove images from field
            gameField.Children.RemoveRange(100, gameField.Children.Count - 100);

            // Enable all buttons
            foreach (Button b in buttons)
            {
                b.Tag = null;
                b.Opacity = 100;
                b.IsEnabled = true;
            }
        }
    }
}
