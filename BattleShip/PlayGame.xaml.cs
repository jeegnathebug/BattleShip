using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BattleShip
{
    public partial class PlayGame : UserControl
    {
        // Player's ship field
        private Button[] buttonsPlayer;
        // Computer's ship field
        private Button[] buttonsAttack;
        // The difficulty
        private Difficulty difficulty;

        // True if player turn
        private bool turn = true;

        // Indicates whether or not a winner has been chosen
        private bool winner = false;

        // Computer's ships
        private bool[] shipsComp = new bool[5];
        private int[] shipsCounterComp = new int[5];

        // Human's ships
        private bool[] ships = new bool[5];
        private int[] shipsCounter = new int[5];

        public PlayGame(Difficulty difficulty, Button[] buttons)
        {
            InitializeComponent();
            initializeGame(difficulty, buttons);
        }

        private void initializeGame(Difficulty difficulty, Button[] buttons)
        {
            this.difficulty = difficulty;

            // Set button field arrays
            buttonsPlayer = new Button[100];
            PlayerShips.Children.CopyTo(buttonsPlayer, 0);

            buttonsAttack = new Button[100];
            PlayerAttack.Children.CopyTo(buttonsAttack, 0);

            // Set player ships
            for (int i = 0; i < 100; i++)
            {
                buttonsPlayer[i].Content = buttons[i].Content;
                buttonsPlayer[i].Tag = buttons[i].Tag;
            }

            // Set computer ships
            setShip(5, "Aircraft_Carrier");
            setShip(4, "Battleship");
            setShip(3, "Submarine");
            setShip(3, "Cruiser");
            setShip(2, "Destroyer");
        }

        private void button_Clicked(object sender, EventArgs e)
        {
            Button chosen = (Button)sender;
            playerTurn(chosen);
        }

        private void buttonAttack_Click(object sender, RoutedEventArgs e)
        {
            string xCoord = textBoxXCoord.Text.Trim();
            string yCoord = textBoxYCoord.Text.Trim();
            int row;
            int column = 99;

            if (xCoord == "" || yCoord == "")
            {
                MessageBox.Show("You must enter coordinates first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Set row
                row = (int.Parse(yCoord) - 1) * 10;

                // Set column
                switch (xCoord)
                {
                    case "A":
                        column = 0;
                        break;
                    case "B":
                        column = 1;
                        break;
                    case "C":
                        column = 2;
                        break;
                    case "D":
                        column = 3;
                        break;
                    case "E":
                        column = 4;
                        break;
                    case "F":
                        column = 5;
                        break;
                    case "G":
                        column = 6;
                        break;
                    case "H":
                        column = 7;
                        break;
                    case "I":
                        column = 8;
                        break;
                    case "J":
                        column = 9;
                        break;
                }

                // If button has already been chosen
                if (!buttonsAttack[row + column].IsEnabled)
                {
                    MessageBox.Show("You've already shot there", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Play button
                    playerTurn(buttonsAttack[row + column]);
                }

                // Reset text boxes
                textBoxXCoord.Text = "";
                textBoxYCoord.Text = "";
            }
        }

        private void buttonXCoordinate_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            textBoxXCoord.Text = button.Content.ToString();
        }

        private void buttonYCoordinate_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            textBoxYCoord.Text = button.Content.ToString();
        }

        private void checkWinner(string message, string caption)
        {
            bool ships = true;

            if (turn)
            {
                for (int index = 0; index < shipsComp.Length; index++)
                {
                    ships = ships && shipsComp[index];
                }
            }
            else
            {
                for (int index = 0; index < this.ships.Length; index++)
                {
                    ships = ships && this.ships[index];
                }
            }

            if (ships)
            {
                // Disable all buttons
                disableButtons();

                MessageBox.Show(message, caption);
                winner = true;

                restart();
            }
        }

        private void computerEasy()
        {
            // Randomly select a button that has not been selected
            Random random = new Random();
            int index;

            do
            {
                index = random.Next(0, 100);

            } while (!buttonsPlayer[index].IsEnabled);

            Button chosen = buttonsPlayer[index];

            // Select button and mark it as hit or miss
            markButton(chosen, ref shipsCounter, ref ships);
        }

        private void computerHard()
        {
            throw new NotImplementedException();
        }

        private void computerTurn()
        {
            // Set computer turn
            turn = false;

            // Choose difficulty
            if (difficulty == Difficulty.Easy)
            {
                computerEasy();
            }
            if (difficulty == Difficulty.Hard)
            {
                computerHard();
            }

            // Check for winner
            checkWinner("All of your ships have been sunk!", "Loser");
        }

        private void disableButtons()
        {
            for (int i = 0; i < 100; i++)
            {
                buttonsAttack[i].IsEnabled = false;
                buttonsPlayer[i].IsEnabled = false;
            }

            buttonAttack.IsEnabled = false;
        }

        private void markButton(Button chosen, ref int[] counter, ref bool[] ships)
        {
            // Select button
            chosen.IsEnabled = false;

            if (chosen.Tag == null)
            {
                // Set button
                chosen.Content = "o";
            }
            else
            {
                // Set messages to display
                string boatName = chosen.Tag.ToString();
                string message, caption;

                // Player turn
                if (turn)
                {
                    message = "You sunk my " + boatName;
                    caption = "Success";
                }
                // Computer turn
                else
                {
                    message = "You're " + boatName + " has been sunk!";
                    caption = "Oh no!";
                }

                // Set button
                chosen.Content = "x";
                int index = -1;

                switch (boatName)
                {
                    case "Aircraft_Carrier":
                        index = 0;
                        break;
                    case "Battleship":
                        index = 1;
                        break;
                    case "Submarine":
                        index = 2;
                        break;
                    case "Cruiser":
                        index = 3;
                        break;
                    case "Destroyer":
                        index = 4;
                        break;
                }

                counter[index]++;

                if (counter[0] == 5 || counter[1] == 4 || counter[2] == 3 || counter[3] == 3 || counter[4] == 2)
                {
                    MessageBox.Show(message, caption);
                    ships[index] = true;

                    // Reset counter so as to not have it true when being checked
                    counter[index] = 0;
                }
            }
        }

        private void playerTurn(Button chosen)
        {
            // Set player turn
            turn = true;

            // Select button and mark it as hit or miss
            markButton(chosen, ref shipsCounterComp, ref shipsComp);

            // Check for winner
            checkWinner("You sank all the ships!", "Winner!");

            // Computer move
            if (!winner)
            {
                computerTurn();
            }
        }
        private void setShip(int size, string boatName)
        {
            Random random = new Random();

            int index = random.Next(10);
            bool isChosen;

            // Orientation is horizontal
            if (index % 2 == 0)
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
                        if (index + i > 99 || buttonsAttack[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }

                } while (isChosen);

                // Set buttons
                for (int i = 0; i < size; i++)
                {
                    buttonsAttack[index + i].Content = boatName;
                    buttonsAttack[index + i].Tag = boatName;
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
                        if (index + i > 99 || buttonsAttack[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }

                } while (isChosen);

                // Set buttons
                for (int i = 0; i < size * 10; i += 10)
                {
                    buttonsAttack[index + i].Content = boatName;
                    buttonsAttack[index + i].Tag = boatName;
                }
            }
        }

        private void restart()
        {
            // TODO Restart game. Go back to main menu, and add wins to file
        }

        private void textBoxXCoord_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO only allow letters from A - J to be inputted
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;

        }

        private void textBoxYCoord_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO only allow numbers from 1 - 10 to be inputted
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;


        }
    }
}
