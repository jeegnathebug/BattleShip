using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BattleShip
{
    public partial class PlayGame : UserControl
    {

        public event EventHandler done;

        // Player's ship field
        private Button[] buttonsPlayer;
        // Computer's ship field
        private Button[] buttonsAttack;

        // Difficulty
        private Difficulty difficulty;

        // Player name
        private string name;

        // True if player's turn
        private bool turn = true;

        // Computer's ships
        private bool[] shipsComp = new bool[5];
        private int[] shipsCounterComp = new int[5];

        // Human's ships
        private bool[] ships = new bool[5];
        private int[] shipsCounter = new int[5];

        public PlayGame()
        {
            InitializeComponent();
        }

        public PlayGame(Difficulty difficulty, Button[] buttons, string name)
        {
            InitializeComponent();
            initializeGame(difficulty, buttons, name);
        }

        private void initializeGame(Difficulty difficulty, Button[] buttons, string name)
        {
            // Set difficulty
            this.difficulty = difficulty;
            // Set name
            this.name = name.ToLower().Replace(' ', '_');

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
            setShip(5, "Aircraft_Carrier", 1);
            setShip(4, "Battleship", 2);
            setShip(3, "Submarine", 3);
            setShip(3, "Cruiser", 4);
            setShip(2, "Destroyer", 5);
        }

        private int binarySearch(string[] players, string index)
        {

            int low = 0;
            int high = players.Length - 1;

            while (high >= low)
            {
                int middle = (low + high) / 2;

                if (players[middle].CompareTo(index) == 0)
                {
                    return middle;
                }
                if (players[middle].CompareTo(index) < 0)
                {
                    low = middle + 1;
                }
                if (players[middle].CompareTo(index) > 0)
                {
                    high = middle - 1;
                }
            }
            return -(low + 1);
        }

        private void button_Clicked(object sender, EventArgs e)
        {
            // Reset textboxes
            textBoxXCoord.Text = "";
            textBoxYCoord.Text = "";

            playerTurn((Button)sender);
        }

        private void buttonAttack_Clicked(object sender, RoutedEventArgs e)
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

        private void buttonExit_Clicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void buttonRestart_Clicked(object sender, EventArgs e)
        {
            if (done != null)
            {
                done(this, e);
            }
        }

        private void buttonXCoordinate_Clicked(object sender, EventArgs e)
        {
            // Set text box text
            Button button = (Button)sender;
            textBoxXCoord.Text = button.Content.ToString();
        }

        private void buttonYCoordinate_Clicked(object sender, EventArgs e)
        {
            // Set text box text
            Button button = (Button)sender;
            textBoxYCoord.Text = button.Content.ToString();
        }

        private bool checkWinner(string message, string caption, string winnerName)
        {
            bool shipsSunk = true;

            // Player turn
            if (turn)
            {
                // Check if all computer ships have been sunk
                for (int index = 0; index < shipsComp.Length; index++)
                {
                    shipsSunk = shipsSunk && shipsComp[index];
                }
            }
            // Computer turn
            else
            {
                // Check if all player ships have been sunk
                for (int index = 0; index < this.ships.Length; index++)
                {
                    shipsSunk = shipsSunk && this.ships[index];
                }
            }

            if (shipsSunk)
            {
                // Disable all buttons
                disableButtons();

                MessageBox.Show(message, caption);

                // Update file and show highscores
                showHighScore(saveWins(winnerName));

                return true;
            }

            return false;
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
            // TODO shoot diagonally
        }

        private void computerTurn()
        {
            // Set computer turn
            turn = false;

            // Choose difficulty and do turn
            if (difficulty.Equals(Difficulty.Easy))
            {
                computerEasy();
            }
            if (difficulty.Equals(Difficulty.Hard))
            {
                computerHard();
            }

            // Check for winner
            checkWinner("All of your ships have been sunk!", "Loser", "computer");
        }

        private void disableButtons()
        {
            Button[] buttonsCoordinate = new Button[20];

            Coordinate.Children.CopyTo(buttonsCoordinate, 0);

            for (int i = 0; i < 100; i++)
            {
                buttonsAttack[i].IsEnabled = false;
                buttonsPlayer[i].IsEnabled = false;

                if (i < 20)
                {
                    buttonsCoordinate[i].IsEnabled = false;
                }
            }

            buttonAttack.Visibility = Visibility.Collapsed;
            buttonRestart.Visibility = Visibility.Visible;
            buttonExit.Visibility = Visibility.Visible;
        }

        private void killerMode()
        {
            // TODO attack the player's ship, by shooting around the previous button
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
                    message = "You sunk my " + boatName.Replace("_", " ");
                    caption = "Success";
                }
                // Computer turn
                else
                {
                    message = "You're " + boatName.Replace("_", " ") + " has been sunk!";
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

                    // Reset counter so as to not have it true when being checked again
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
            bool win = checkWinner("You sank all the ships!", "Winner!", name);

            // Computer move
            if (!win)
            {
                computerTurn();
            }
        }

        private List<string> saveWins(string winnerName)
        {
            // Filename to save score
            string path = @"score.txt";

            // Create file if it does not exist
            if (!File.Exists(path))
            {
                FileStream stream = File.Create(path);
                stream.Close();
            }

            // Get all previous players
            List<string> previousPlayers = new List<string>(File.ReadAllLines(path));
            string[] previousPlayer = { name, "0", "0" };
            string[] playerNames;
            int index;

            int wins = 0;
            int losses = 0;

            // Get name and index of previous player
            playerNames = new string[previousPlayers.Count];

            for (index = 0; index < previousPlayers.Count; index++)
            {
                playerNames[index] = previousPlayers[index].Split(' ')[0];
            }

            // Find index of player
            index = binarySearch(playerNames, name);

            // Player already exists
            if (index > -1)
            {
                previousPlayer = previousPlayers[index].Split();
                previousPlayers.RemoveAt(index);
            }
            else
            {
                index = -(index + 1);
            }

            // Set wins or losses
            if (winnerName.Equals("computer"))
            {
                losses = int.Parse(previousPlayer[2]) + 1;
            }
            else
            {
                wins = int.Parse(previousPlayer[1]) + 1;
            }

            // Add to array
            previousPlayers.Insert(index, name + " " + wins + " " + losses);

            // Write back to file
            File.WriteAllLines(path, previousPlayers);

            return previousPlayers;
        }

        private void setShip(int size, string boatName, int number)
        {
            // Choose random number
            Random random = new Random();
            int index;

            do
            {
                index = random.Next(10);
                number--;
            } while (number != 0);

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
                    buttonsAttack[index + i].Tag = boatName;
                    // FOR TESTING PURPOSES********************************
                    buttonsAttack[index + i].Content = boatName;
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
                    buttonsAttack[index + i].Tag = boatName;
                    // FOR TESTING PURPOSES********************************
                    buttonsAttack[index + i].Content = boatName;
                }
            }
        }

        private void showHighScore(List<string> highScores)
        {
            HighScoreWindow highScoreWindow = new HighScoreWindow(highScores);

            highScoreWindow.ShowDialog();
        }
    }
}
