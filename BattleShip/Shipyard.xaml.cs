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
        public int hits = 0;
        public bool sunk = false;

        public List<int> location;
        public bool placed = false;

        public Ship(ShipName name, int size)
        {
            this.name = name;
            this.size = size;
        }
    }

    public enum ShipName
    {
        Aircraft_Carrier, Battleship, Submarine, Cruiser, Destroyer
    }

    enum Orientation
    {
        Vertical, Horizontal
    }

    /// <summary>
    /// Interaction logic for Shipyard.xaml
    /// </summary>
    public partial class Shipyard : UserControl
    {
        public event EventHandler play;

        private Orientation orientation = Orientation.Horizontal;
        public Button[] buttons;

        // List of players actions for undoing
        List<Ship> actions = new List<Ship>();

        // Ships
        public Ship[] ships = new Ship[5];
        private Ship aircraftCarrier = new Ship(ShipName.Aircraft_Carrier, 5);
        private Ship battleship = new Ship(ShipName.Battleship, 4);
        private Ship submarine = new Ship(ShipName.Submarine, 3);
        private Ship cruiser = new Ship(ShipName.Cruiser, 3);
        private Ship destroyer = new Ship(ShipName.Destroyer, 2);

        // Selected ship
        private Ship ship;
        // Selected ListBoxItem
        private ListBoxItem item;

        /// <summary>
        /// Initializes setup phase
        /// </summary>
        public Shipyard()
        {
            InitializeComponent();

            // Set ListBoxItem's Tags
            Aircraft_Carrier.Tag = aircraftCarrier;
            Battleship.Tag = battleship;
            Submarine.Tag = submarine;
            Cruiser.Tag = cruiser;
            Destroyer.Tag = destroyer;

            // Initialize ship array
            ships[0] = aircraftCarrier;
            ships[1] = battleship;
            ships[2] = submarine;
            ships[3] = cruiser;
            ships[4] = destroyer;

            buttons = new Button[100];
            gameField.Children.CopyTo(buttons, 0);
            ship = aircraftCarrier;
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

                setShip(ship, index);
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

            randomize(aircraftCarrier, randomOrientation(1));
            randomize(battleship, randomOrientation(2));
            randomize(submarine, randomOrientation(3));
            randomize(cruiser, randomOrientation(4));
            randomize(destroyer, randomOrientation(5));
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
        /// Canges the orientation of the placement
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (orientation == Orientation.Horizontal)
            {
                orientation = Orientation.Vertical;
            }
            else
            {
                orientation = Orientation.Horizontal;
            }

            labelOrientation.Content = "Orientation: " + orientation;
        }

        /// <summary>
        /// Submits the ship placement. Shows an error message if all ships have not been placed
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (!(aircraftCarrier.placed && battleship.placed && submarine.placed && cruiser.placed && destroyer.placed))
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
                buttons[ship.location[i]].Content = "";
                buttons[ship.location[i]].Tag = null;
            }

            // Reset ship
            ship.placed = false;
            ship.location = null;

            // Reset ListItemBox
            ListBoxItem[] items = { Aircraft_Carrier, Battleship, Submarine, Cruiser, Destroyer };
            for (int i = 0; i < 5; i++)
            {
                if (items[i].Tag.Equals(ship))
                {
                    items[i].IsEnabled = true;
                }
            }

            // Remove action
            actions.RemoveAt(actions.Count - 1);

            // Disable undo button if no actions remain
            if (actions.Count == 0)
            {
                buttonUndo.IsEnabled = false;
            }
        }

        /// <summary>
        /// Chooses a boat, and updates labels
        /// </summary>
        /// <param name="sender">The ListBoxItem</param>
        /// <param name="e">The Event</param>
        private void ListBoxItem_Selected(object sender, EventArgs e)
        {
            ListBoxItem selected = (ListBoxItem)sender;

            item = selected;
            ship = (Ship)selected.Tag;

            // Change labels
            labelSize.Content = "Size: " + ship.size;
            labelBoat.Content = ship.name;
        }

        /// <summary>
        /// Check if a ship has been selected from the list box
        /// </summary>
        /// <returns>True if a ship has been selected, false otherwise</returns>
        private bool nullCheck()
        {
            if (!item.IsEnabled)
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
        /// Randomly selects a placement for the boat
        /// </summary>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="orientation">The orientation chosen for the boat placement</param>
        private void randomize(Ship ship, Orientation orientation)
        {
            Random random = new Random();
            bool isChosen;
            int index;
            int size = ship.size;

            // Orientation is horizontal
            if (orientation.Equals(Orientation.Horizontal))
            {
                do
                {
                    isChosen = false;
                    do
                    {
                        index = random.Next(0, 100);

                    } while (((index + (size - 1)) % 10 < size - 1));

                    // Check if every button to be selected is not already selected
                    for (int i = 0; i < size; i++)
                    {
                        if (index + i > 99 || buttons[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }
                } while (isChosen);

                // Set buttons
                for (int i = 0; i < size; i++)
                {
                    buttons[index + i].Tag = ship;
                    buttons[index + i].Content = ship.name;
                }
            }
            // Orientation is vertical
            else
            {
                do
                {
                    isChosen = false;
                    do
                    {
                        index = random.Next(0, 100);

                    } while ((index / 10) + (size * 10) > 100 || isChosen);

                    // Check if every button to be selected is not already selected
                    for (int i = 0; i < size * 10; i += 10)
                    {
                        if (index + i > 99 || buttons[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }
                } while (isChosen);

                // Set buttons
                for (int i = 0; i < size * 10; i += 10)
                {
                    buttons[index + i].Tag = ship;
                    buttons[index + i].Content = ship.name;
                }
            }

            update(ship, false);
        }

        /// <summary>
        /// Chooses a random orientation
        /// </summary>
        /// <param name="number">The seed</param>
        /// <returns>A randomly chosen orientation</returns>
        private Orientation randomOrientation(int number)
        {
            // Choose random number
            Random random = new Random();
            int index;

            do
            {
                index = random.Next(10);
                number--;
            } while (number != 0);

            if (index % 2 == 0)
            {
                return Orientation.Horizontal;
            }
            else
            {
                return Orientation.Vertical;
            }
        }

        /// <summary>
        /// Resets the screen
        /// </summary>
        private void reset()
        {
            buttonUndo.IsEnabled = false;
            actions.Clear();

            // Enable list box items
            Aircraft_Carrier.IsEnabled = true;
            Battleship.IsEnabled = true;
            Submarine.IsEnabled = true;
            Cruiser.IsEnabled = true;
            Destroyer.IsEnabled = true;

            // Unset placed ships
            aircraftCarrier.placed = false;
            battleship.placed = false;
            submarine.placed = false;
            cruiser.placed = false;
            destroyer.placed = false;

            // Enable all buttons
            foreach (Button b in buttons)
            {
                b.Content = "";
                b.Tag = null;
            }
        }

        /// <summary>
        /// Sets the chosen ship based on the button selected, if the ship cannot legally be placed on chosen button, an error message is shown
        /// </summary>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="index">The index of the button chosen in the button field</param>
        private void setShip(Ship ship, int index)
        {
            bool placed = false;
            bool isChosen = false;

            int size = ship.size;

            // Invalid placement check
            // Orientation is horizontal
            if (orientation.Equals(Orientation.Horizontal))
            {
                // Go through every button that will be chosen to see if it has already been chosen or not
                int counter = 1;
                for (int i = 0; i < size; i++)
                {
                    if (index + i <= 99)
                    {
                        if (buttons[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }
                    else
                    {
                        if (buttons[index - counter].Tag != null)
                        {
                            isChosen = true;
                        }
                        counter++;
                    }
                }

                if (isChosen)
                {
                    MessageBox.Show("Invalid placement", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    placed = true;
                }
            }
            // Orientation is vertical
            else
            {
                // Go through every button that will be chosen to see if it has already been chosen or not
                int counter = 10;
                for (int i = 0; i < size * 10; i += 10)
                {
                    if (index + i <= 99)
                    {
                        if (buttons[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }
                    else
                    {
                        if (buttons[index - counter].Tag != null)
                        {
                            isChosen = true;
                        }
                        counter += 10;
                    }
                }
                if (isChosen)
                {
                    MessageBox.Show("Invalid placement", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    placed = true;
                }
            }

            // Choose buttons to select
            int[] chosenButtonIndexes = new int[size];
            if (placed)
            {
                // Orientation is horizontal
                if (orientation.Equals(Orientation.Horizontal))
                {
                    // If placed in two rows
                    if (((index + (size - 1)) % 10 < size - 1))
                    {
                        int counter = 0, reverseCounter = 1;

                        while ((index + counter) % 10 > 1)
                        {
                            chosenButtonIndexes[counter] = index + counter;
                            counter++;
                        }
                        for (int i = counter; i < size; i++)
                        {
                            chosenButtonIndexes[i] = index - reverseCounter;
                            reverseCounter++;
                        }
                    }
                    // If placed in one row
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            chosenButtonIndexes[i] = index + i;
                        }
                    }
                }
                // Orientation is vertical
                else
                {
                    // If placed in two columns
                    if (index + (size * 10) > 100)
                    {
                        int counter = 0, reverseCounter = 10;
                        while ((index / 10 + counter) % 100 < 10)
                        {
                            chosenButtonIndexes[counter] = index + counter * 10;
                            counter++;
                        }
                        for (int i = counter; i < size; i++)
                        {
                            chosenButtonIndexes[i] = index - reverseCounter;
                            reverseCounter += 10;
                        }
                    }
                    // If placed in one column
                    else
                    {
                        int counter = 0;
                        for (int i = 0; i < size * 10; i += 10)
                        {
                            chosenButtonIndexes[counter] = index + i;
                            counter++;
                        }
                    }
                }

                // Select buttons
                ship.location = new List<int>(chosenButtonIndexes);
                for (int i = 0; i < size; i++)
                {
                    buttons[chosenButtonIndexes[i]].Content = ship.name;
                    buttons[chosenButtonIndexes[i]].Tag = ship;

                    ship.location.Add(i);
                }

                update(ship, true);
            }
        }

        /// <summary>
        /// Updates the listbox, and private fields
        /// </summary>
        /// <param name="ship">The ship to be updated</param>
        /// <param name="human">Whether or not to update one listbox and field, or all depending on if the player chose the button, or if the placement was randomized</param>
        private void update(Ship ship, bool human)
        {
            if (human)
            {
                // Reset variables
                item.IsEnabled = false;

                // Placed ship
                ship.placed = true;

                actions.Add(ship);
                buttonUndo.IsEnabled = true;
            }
            // Randomized
            else
            {
                Aircraft_Carrier.IsEnabled = false;
                Battleship.IsEnabled = false;
                Submarine.IsEnabled = false;
                Cruiser.IsEnabled = false;
                Destroyer.IsEnabled = false;

                aircraftCarrier.placed = true;
                battleship.placed = true;
                submarine.placed = true;
                cruiser.placed = true;
                destroyer.placed = true;
            }
        }
    }
}
