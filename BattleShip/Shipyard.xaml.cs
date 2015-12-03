using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    enum Orientation
    {
        Down, Right
    }

    public partial class Shipyard : UserControl
    {
        public event EventHandler play;

        private Orientation orientation = Orientation.Right;
        public Button[] buttons;
        private ListBoxItem item;
        private int size = 0;
        private string boatName;

        private bool aircraftCarrier = false;
        private bool battleship = false;
        private bool submarine = false;
        private bool cruiser = false;
        private bool destroyer = false;

        public Shipyard()
        {
            InitializeComponent();

            buttons = new Button[100];
            gameField.Children.CopyTo(buttons, 0);
            item = Aircraft_Carrier;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            // Enable list box items
            Aircraft_Carrier.IsEnabled = true;
            Battleship.IsEnabled = true;
            Submarine.IsEnabled = true;
            Cruiser.IsEnabled = true;
            Destroyer.IsEnabled = true;

            // Unset placed boats
            aircraftCarrier = false;
            battleship = false;
            submarine = false;
            cruiser = false;
            destroyer = false;

            // Enable buttons
            foreach (Button b in buttons)
            {
                b.Content = "";
                b.Tag = null;
            }
        }

        private void ListBoxItem_Selected(object sender, EventArgs e)
        {
            item = (ListBoxItem)sender;

            boatName = item.Name;

            // Change labels
            switch (boatName)
            {
                case "Aircraft_Carrier":
                    size = 5;
                    break;
                case "Battleship":
                    size = 4;
                    break;
                case "Submarine":
                    size = 3;
                    break;
                case "Cruiser":
                    size = 3;
                    break;
                case "Destroyer":
                    size = 2;
                    break;
            }

            labelSize.Content = "Size: " + size;
            labelBoat.Content = boatName;
        }

        private void button_Clicked(object sender, EventArgs e)
        {

            if (nullCheck())
            {
                Button chosen = (Button)sender;
                int index = -1;

                // Find index of chosen button in array
                for (int i = 0; i < 100; i++)
                {
                    if (buttons[i].Name == chosen.Name)
                    {
                        index = i;
                    }
                }

                bool placed = false;
                bool isChosen = false;

                // Invalid placement check
                // Orientation is horizontal
                if (orientation.Equals(Orientation.Right))
                {
                    // Go through every button that will be chosen to see if it has already been chosen or not
                    for (int i = 0; i < size; i++)
                    {
                        if (index + i <= 99 && buttons[index + i].Tag != null)
                        {
                            isChosen = true;
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
                    for (int i = 0; i < size * 10; i += 10)
                    {
                        if (index + i <= 99 && buttons[index + i].Tag != null)
                        {
                            isChosen = true;
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

                Button[] chosenButtons = new Button[size];
                // Disable chosen buttons
                if (placed)
                {
                    // Orientation is horizontal
                    if (orientation.Equals(Orientation.Right))
                    {
                        // If placed in two rows
                        if (((index + (size - 1)) % 10 < size - 1))
                        {
                            int counter = 0, reverseCounter = 1;

                            while ((index + counter) % 10 > 1)
                            {
                                Console.WriteLine((index + counter) % 10);
                                chosenButtons[counter] = buttons[index + counter];
                                counter++;
                            }
                            for (int i = counter; i < size; i++)
                            {
                                chosenButtons[i] = buttons[index - reverseCounter];
                                reverseCounter++;
                            }
                        }
                        // If placed in one row
                        else
                        {
                            for (int i = 0; i < size; i++)
                            {
                                chosenButtons[i] = buttons[index + i];
                            }
                        }

                        for (int i = 0; i < size; i++)
                        {
                            chosenButtons[i].Content = boatName;
                            chosenButtons[i].Tag = boatName;
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
                                chosenButtons[counter] = buttons[index + counter * 10];
                                counter++;
                            }
                            for (int i = counter; i < size; i++)
                            {
                                chosenButtons[i] = buttons[index - reverseCounter];
                                reverseCounter += 10;
                            }
                        }
                        // If placed in one column
                        else
                        {
                            int counter = 0;
                            for (int i = 0; i < size * 10; i += 10)
                            {
                                chosenButtons[counter] = buttons[index + i];
                                counter++;
                            }
                        }

                        for (int i = 0; i < size; i++)
                        {
                            chosenButtons[i].Content = boatName;
                            chosenButtons[i].Tag = boatName;
                        }
                    }

                    // Reset variables
                    item.IsEnabled = false;
                    placed = false;

                    // Placed ship
                    switch (boatName)
                    {
                        case "Aircraft_Carrier":
                            aircraftCarrier = true;
                            break;
                        case "Battleship":
                            battleship = true;
                            break;
                        case "Submarine":
                            submarine = true;
                            break;
                        case "Cruiser":
                            cruiser = true;
                            break;
                        case "Destroyer":
                            destroyer = true;
                            break;
                    }
                }
            }
        }

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (nullCheck())
            {
                if (orientation == Orientation.Right)
                {
                    orientation = Orientation.Down;
                }
                else
                {
                    orientation = Orientation.Right;
                }

                labelOrientation.Content = "Orientation: " + orientation;
            }
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (!(aircraftCarrier && battleship && submarine && cruiser && destroyer))
            {
                MessageBox.Show("All ships must be placed before proceeding", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (play != null)
                {
                    play(this, e);
                }
            }
        }

        private bool nullCheck()
        {
            if (!item.IsEnabled)
            {
                MessageBox.Show("You must first select a ship", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
