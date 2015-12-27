using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattleShip
{
    public partial class Common
    {
        private Grid gameField;
        private Button[] buttons;

        public Common(Grid gameField, Button[] buttons)
        {
            this.gameField = gameField;
            this.buttons = buttons;
        }
        /// <summary>
        /// Chooses a random orientation
        /// </summary>
        /// <param name="number">The seed</param>
        /// <returns>A randomly chosen orientation</returns>
        public Orientation randomOrientation(int number)
        {
            // Choose random number
            Random random = new Random();
            int index;

            do
            {
                index = random.Next(10);
                number--;
            } while (number >= 0);

            return (Orientation)(index % 2);
        }

        /// <summary>
        /// Sets image of placed ship on button field
        /// </summary>
        /// <param name="index">The index of the first button chosen, where the front of the image will be placed</param>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="orientation">The orientation of the ship</param>
        private void setImage(int index, Ship ship, bool isComputerPlacement)
        {
            // Copy image
            Image image = new Image();
            image.Source = ship.image.Source;
            image.Stretch = ship.image.Stretch;

            // Set properties
            int span = ship.size;
            int row = Grid.GetRow(buttons[index]);
            int column = Grid.GetColumn(buttons[index]);
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);

            if (ship.orientation.Equals(Orientation.VERTICAL))
            {
                // Rotate image
                image.LayoutTransform = new RotateTransform(90.0);
                Grid.SetRowSpan(image, span);
                image.Height = ship.image.Width;
                image.Width = ship.image.Height;
            }
            else
            {
                Grid.SetColumnSpan(image, span);
                image.Height = ship.image.Height;
                image.Width = ship.image.Width;
            }

            if (isComputerPlacement)
            {
                image.Visibility = System.Windows.Visibility.Hidden;
            }

            // Add image to location
            gameField.Children.Add(image);

            ship.image = new Image();
            ship.image.Source = image.Source;
            ship.image.Stretch = image.Stretch;
            ship.image.Height = image.Height;
            ship.image.Width = image.Width;
            ship.image.LayoutTransform = image.LayoutTransform;
            Grid.SetRow(ship.image, row);
            Grid.SetColumn(ship.image, column);
            Grid.SetRowSpan(ship.image, Grid.GetRowSpan(image));
            Grid.SetColumnSpan(ship.image, Grid.GetColumnSpan(image));
        }

        /// <summary>
        /// Sets the chosen ship based on the button selected, if the ship cannot legally be placed on chosen button, an error message is shown
        /// </summary>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="index">The index of the button chosen in the button field</param>
        /// <param name="orientation">The orientation of the ship to be placed</param>
        /// <param name="isRandomized">Whether or not the player chose to randomize the ship placement</param>
        public bool setShip(Ship ship, int index, Orientation orientation, bool isRandomized = false, bool isComputerPlacement = false)
        {
            int size = ship.size;
            int[] chosenButtonIndexes = new int[size];

            // Orientation is horizontal
            if (orientation.Equals(Orientation.HORIZONTAL))
            {
                // If placed in two rows
                if (((index + (size - 1)) % 10 < size - 1))
                {
                    int counter = 0;
                    int reverseCounter = 1;

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
                    int counter = 0;
                    int reverseCounter = 10;

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
                    for (int i = 0; i < size; i++)
                    {
                        chosenButtonIndexes[i] = index + (i * 10);
                    }
                }
            }

            bool isValidPlacement = true;

            // Invalid placement check
            for (int i = 0; i < size; i++)
            {
                if (buttons[chosenButtonIndexes[i]].Tag != null)
                {
                    isValidPlacement = false;
                }
            }

            if (isValidPlacement)
            {
                // Sort array
                Array.Sort(chosenButtonIndexes);

                // Update ship
                ship.orientation = orientation;
                ship.location = new List<int>(chosenButtonIndexes);
                ship.placed = true;

                if (!isComputerPlacement)
                {
                    ship.item.IsEnabled = false;
                }

                // Set image
                // For some reason, if not done twice and ship is vertical, the image displayed is smaller than intended
                setImage(chosenButtonIndexes[0], ship, isComputerPlacement);
                setImage(chosenButtonIndexes[0], ship, isComputerPlacement);

                // Select buttons
                for (int i = 0; i < size; i++)
                {
                    buttons[chosenButtonIndexes[i]].Tag = ship;
                    if (!isComputerPlacement)
                    {
                        buttons[chosenButtonIndexes[i]].Opacity = 0;
                        buttons[chosenButtonIndexes[i]].IsEnabled = false;
                    }
                }
            }

            // Error if illegally placed
            else if (!isRandomized)
            {
                Console.Beep(500, 250);
            }

            return isValidPlacement;
        }
    }
}
