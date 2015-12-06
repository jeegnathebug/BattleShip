using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for PlayGame.xaml
    /// </summary>
    public partial class PlayGame : UserControl
    {
        public event EventHandler done;

        // Difficulty
        private Difficulty difficulty;
        // Player name
        private string name;

        // True if player's turn
        private bool turn = true;

        // Computer's moves
        private List<int> computerMoves = new List<int>();
        // Computer's next possible moves
        private List<int> potentialAttacks = new List<int>();
        // Index of last hit
        private int lastHitIndex = -1;
        // Boat hit
        private Ship lastHitShip = new Ship(ShipName.Aircraft_Carrier, 99);

        // Player's ship field
        private Button[] buttonsPlayer;
        // Computer's ship field
        private Button[] buttonsAttack;

        // Player ships
        private Ship aircraftCarrierPlayer;
        private Ship battleshipPlayer;
        private Ship submarinePlayer;
        private Ship cruiserPlayer;
        private Ship destroyerPlayer;

        // Computer ships
        private Ship aircraftCarrierComputer = new Ship(ShipName.Aircraft_Carrier, 5);
        private Ship battleshipComputer = new Ship(ShipName.Battleship, 4);
        private Ship submarineComputer = new Ship(ShipName.Submarine, 3);
        private Ship cruiserComputer = new Ship(ShipName.Cruiser, 3);
        private Ship destroyerComputer = new Ship(ShipName.Destroyer, 2);

        /// <summary>
        /// Initializes gameplay phase
        /// </summary>
        public PlayGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes gameplay phase
        /// </summary>
        /// <param name="difficulty">The chosen difficulty</param>
        /// <param name="buttons">The submitted placement of ships</param>
        /// <param name="name">The name of the player</param>
        public PlayGame(Difficulty difficulty, Button[] buttons, string name, Ship[] ships)
        {
            InitializeComponent();
            initializeGame(buttons, ships);

            // Set difficulty
            this.difficulty = difficulty;
            // Set name
            this.name = name.ToLower().Replace(' ', '_');
        }

        /// <summary>
        /// Initializes gameplay phase
        /// </summary>
        /// <param name="buttons">The submitted placement of ships</param>
        private void initializeGame(Button[] buttons, Ship[] ships)
        {
            // Set button field arrays
            buttonsPlayer = new Button[100];
            PlayerShips.Children.CopyTo(buttonsPlayer, 0);

            buttonsAttack = new Button[100];
            PlayerAttack.Children.CopyTo(buttonsAttack, 0);

            // Set player ships
            aircraftCarrierPlayer = ships[0];
            battleshipPlayer = ships[1];
            submarinePlayer = ships[2];
            cruiserPlayer = ships[3];
            destroyerPlayer = ships[4];

            // Set labels
            labelComputerAircraftCarrier.Tag = aircraftCarrierComputer;
            labelComputerBattleship.Tag = battleshipComputer;
            labelComputerSubmarine.Tag = submarineComputer;
            labelComputerCruiser.Tag = cruiserComputer;
            labelComputerDestroyer.Tag = destroyerComputer;

            labelPlayerAircraftCarrier.Tag = aircraftCarrierPlayer;
            labelPlayerBattleship.Tag = battleshipPlayer;
            labelPlayerSubmarine.Tag = submarinePlayer;
            labelPlayerCruiser.Tag = cruiserPlayer;
            labelPlayerDestroyer.Tag = destroyerPlayer;

            // Set player buttons
            for (int i = 0; i < 100; i++)
            {
                buttonsPlayer[i].Content = buttons[i].Content;
                buttonsPlayer[i].Tag = buttons[i].Tag;
            }

            // Set computer ships
            setShip(aircraftCarrierComputer, randomOrientation(1));
            setShip(battleshipComputer, randomOrientation(2));
            setShip(submarineComputer, randomOrientation(3));
            setShip(cruiserComputer, randomOrientation(4));
            setShip(destroyerComputer, randomOrientation(5));
        }

        /// <summary>
        /// Binary search for player names
        /// </summary>
        /// <param name="players">A list of player names</param>
        /// <param name="index">The name for which to search</param>
        /// <returns>The index of the name, or the negative index - 1 at where the name would occur</returns>
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

        /// <summary>
        /// The chosen button to attack. If the button has already been chosen, an error message will be displayed
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void button_Clicked(object sender, EventArgs e)
        {
            // Reset textboxes
            textBoxXCoord.Text = "";
            textBoxYCoord.Text = "";

            playerTurn((Button)sender);
        }

        /// <summary>
        /// Submits the coordinates to attack. If coordinates are not chosen, or if the coordinates have already been fired at, an error message will be displayed
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
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

        /// <summary>
        /// Exits the game
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonExit_Clicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        /// <summary>
        /// Restarts the game
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonRestart_Clicked(object sender, EventArgs e)
        {
            if (done != null)
            {
                done(this, e);
            }
        }

        /// <summary>
        /// Chooses the button as the x-coordinate
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonXCoordinate_Clicked(object sender, EventArgs e)
        {
            // Set text box text
            Button button = (Button)sender;
            textBoxXCoord.Text = button.Content.ToString();
        }

        /// <summary>
        /// Chooses the button as the y-coordinate
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonYCoordinate_Clicked(object sender, EventArgs e)
        {
            // Set text box text
            Button button = (Button)sender;
            textBoxYCoord.Text = button.Content.ToString();
        }

        /// <summary>
        /// Checks for a winner
        /// </summary>
        /// <param name="message">The message to be displayed, if a winner is chosen</param>
        /// <param name="caption">The caption to be displayed, if a winner is chosen</param>
        /// <param name="winnerName">The winner's name</param>
        /// <returns></returns>
        private bool checkWinner(string message, string caption, string winnerName)
        {
            bool sunk = true;

            // Player turn
            if (turn)
            {
                // Check if all computer ships have been sunk
                sunk = aircraftCarrierComputer.sunk && battleshipComputer.sunk && submarineComputer.sunk && cruiserComputer.sunk && destroyerComputer.sunk;
            }
            // Computer turn
            else
            {
                // Check if all player ships have been sunk
                sunk = aircraftCarrierPlayer.sunk && battleshipPlayer.sunk && submarinePlayer.sunk && cruiserPlayer.sunk && destroyerPlayer.sunk;
            }

            if (sunk)
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

        /// <summary>
        /// Chooses between defaultSet and list as the Button List to be used. If no buttons are enabled in list, defaultSet will be chosen
        /// </summary>
        /// <param name="list">The Button List</param>
        /// <param name="defaultSet">The default set of buttons</param>
        /// <returns>list, if not all the buttons are disabled, or defaultSet otherwise</returns>
        private List<int> chooseSet(List<int> list, List<int> defaultSet)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (buttonsPlayer[list[i]].IsEnabled)
                {
                    list.TrimExcess();
                    return list;
                }
            }

            defaultSet.TrimExcess();
            return defaultSet;
        }

        /// <summary>
        /// The computer's easy difficulty settings
        /// </summary>
        private Button computerEasy()
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
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// The computer's hard difficulty settings
        /// </summary>
        private Button computerHard()
        {
            List<int> firstSet = new List<int>() { 0, 4, 8, 11, 15, 19, 22, 26, 33, 37, 40, 44, 48, 51, 55, 59, 62, 66, 73, 77, 80, 84, 88, 91, 95, 99 };
            List<int> secondSet = new List<int>() { 2, 6, 13, 17, 20, 24, 28, 31, 35, 39, 42, 46, 53, 57, 60, 64, 68, 71, 75, 79, 82, 86, 93, 97 };
            List<int> defaultSet = new List<int>() { 1, 3, 5, 7, 9, 10, 12, 14, 16, 18, 21, 23, 25, 27, 29, 30, 32, 34, 36, 38, 41, 43, 45, 47, 49, 50, 52, 54, 56, 58, 61, 63, 65, 67, 69, 70, 72, 74, 76, 78, 81, 83, 85, 87, 89, 90, 92, 94, 96, 98 };

            List<int> list;

            // Choose set to go through
            list = chooseSet(firstSet, defaultSet);

            if (list.Count != firstSet.Count)
            {
                list = chooseSet(secondSet, defaultSet);
            }

            Random random = new Random();
            int size = list.Capacity;
            int index = 0;

            do
            {
                index = random.Next(list.Count);

            } while (!buttonsPlayer[list[index]].IsEnabled);

            Button chosen = buttonsPlayer[list[index]];

            // Select button and mark it as hit or miss
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// Basically cheating
        /// </summary>
        private Button computerLegendary()
        {
            // Add all player's ships to hit list
            potentialAttacks.Clear();
            for (int i = 0; i < buttonsPlayer.Length; i++)
            {
                if (buttonsPlayer[i].Tag != null && buttonsPlayer[i].IsEnabled)
                {
                    potentialAttacks.Add(i);
                }
            }

            // Chose button from hit list
            Random random = new Random();
            Button chosen;
            int index;
            int percentage;
            int counter;

            // Choose a new seed for random number
            if (computerMoves.Count == 0)
            {
                counter = 10;
            }
            else
            {
                counter = computerMoves[computerMoves.Count - 1];
            }

            do
            {
                percentage = random.Next(1000);
                counter--;
            } while (counter > 0);

            // 25% chance of shooting randomly
            if (percentage <= 250)
            {
                return computerEasy();
            }
            // 75% chance of shooting at a player's ship
            else
            {
                counter = potentialAttacks.Count;
                do
                {
                    index = random.Next(0, counter);
                } while (!buttonsPlayer[potentialAttacks[index]].IsEnabled);

                chosen = buttonsPlayer[potentialAttacks[index]];

                potentialAttacks.RemoveAt(index);
            }

            computerMoves.Add(index);
            return chosen;
        }

        /// <summary>
        /// The computer's medium difficulty setting
        /// </summary>
        private Button computerMedium()
        {
            List<int> firstSet = new List<int>() { 0, 5, 11, 16, 22, 27, 33, 38, 44, 49, 50, 55, 61, 66, 72, 77, 83, 88, 94, 99 };
            List<int> secondSet = new List<int>() { 3, 8, 14, 19, 25, 30, 36, 41, 47, 52, 58, 63, 69, 74, 80, 85, 91, 96 };
            List<int> thirdSet = new List<int>() { 1, 6, 12, 17, 20, 23, 28, 31, 34, 39, 42, 45, 53, 56, 64, 67, 70, 75, 78, 81, 86, 89, 92, 97 };
            List<int> defaultSet = new List<int> { 2, 4, 7, 9, 10, 13, 15, 18, 21, 24, 26, 29, 32, 35, 37, 40, 43, 46, 48, 51, 54, 57, 59, 60, 62, 65, 68, 71, 73, 76, 79, 82, 84, 87, 90, 93, 95, 98 };

            List<int> list;

            // Choose set to go through
            list = chooseSet(firstSet, defaultSet);

            if (list.Count != firstSet.Count)
            {
                list = chooseSet(secondSet, defaultSet);
                if (list.Count != secondSet.Count)
                {
                    list = chooseSet(thirdSet, defaultSet);
                }
            }

            Random random = new Random();
            int size = list.Capacity;
            int index = 0;

            do
            {
                index = random.Next(list.Count);

            } while (!buttonsPlayer[list[index]].IsEnabled);

            Button chosen = buttonsPlayer[list[index]];

            // Select button and mark it as hit or miss
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// The computer's turn, where the computer will act according to the difficulty chosen
        /// </summary>
        private void computerTurn()
        {
            // Set computer turn
            turn = false;

            Button chosen = null;

            // Legendary mode
            if (difficulty.Equals(Difficulty.Legendary))
            {
                chosen = computerLegendary();
            }
            else
            {
                // Last move was a hit
                if (computerMoves.Count != 0 && lastHitIndex > -1)
                {
                    chosen = killerMode();
                }
                // Go to default difficulty move
                if (chosen == null)
                {
                    // Choose difficulty
                    if (difficulty.Equals(Difficulty.Easy))
                    {
                        chosen = computerEasy();
                    }
                    if (difficulty.Equals(Difficulty.Medium))
                    {
                        chosen = computerMedium();
                    }
                    if (difficulty.Equals(Difficulty.Hard))
                    {
                        chosen = computerHard();
                    }
                }
            }

            // If move was a hit
            if (chosen.Tag != null)
            {
                lastHitIndex = Array.IndexOf(buttonsPlayer, chosen);
                lastHitShip = (Ship)chosen.Tag;
            }

            markButton(chosen);

            // Check for winner
            checkWinner("All of your ships have been sunk!", "Loser", "computer");
        }

        /// <summary>
        /// Disables all buttons and allows player to choose whether to restart or exit the game
        /// </summary>
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

        /// <summary>
        /// Once a sihp has been hit, the computer will target the buttons around it
        /// </summary>
        private Button killerMode()
        {
            if (lastHitShip.sunk)
            {
                lastHitIndex = -1;
                return null;
            }

            // Add to hit list
            if (((lastHitIndex + 1) % 9 != 0) && buttonsPlayer[lastHitIndex + 1].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex + 1);
            }
            if (((lastHitIndex - 1) % 10 != 0) && buttonsPlayer[lastHitIndex - 1].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex - 1);
            }
            if (((lastHitIndex + 10) <= 99) && buttonsPlayer[lastHitIndex + 10].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex + 10);
            }
            if (((lastHitIndex - 10) >= 0) && buttonsPlayer[lastHitIndex - 10].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex - 10);
            }

            // Chose button from hit list
            int index;

            if (potentialAttacks.Count != 0)
            {

                index = potentialAttacks[0];

                while (!buttonsPlayer[index].IsEnabled)
                {
                    potentialAttacks.RemoveAt(0);
                    if (potentialAttacks.Count != 0)
                    {
                        index = potentialAttacks[0];
                    }
                    else
                    {
                        break;
                    }
                }

                Button chosen = buttonsPlayer[index];
                computerMoves.Add(index);
                potentialAttacks.RemoveAt(0);

                return chosen;
            }

            return null;
        }

        /// <summary>
        /// Marks chosen button as a hit or miss, and displays message if ship has been sunk
        /// </summary>
        /// <param name="chosen">The Button chosen as a shot</param>
        private void markButton(Button chosen)
        {
            // Select button
            chosen.IsEnabled = false;
            Ship ship = (Ship)chosen.Tag;

            // Shot missed
            if (ship == null)
            {
                // Set button
                chosen.Content = "o";
            }
            // Shot hit
            else
            {
                // Set messages to display
                string boatName = ship.name.ToString();
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

                Label[] labels;
                Label label = null;
                labels = new Label[] { labelComputerAircraftCarrier, labelComputerBattleship, labelComputerSubmarine, labelComputerCruiser, labelComputerDestroyer, labelPlayerAircraftCarrier, labelPlayerBattleship, labelPlayerSubmarine, labelPlayerCruiser, labelPlayerDestroyer };

                for (int i = 0; i < 10; i++)
                {
                    if (labels[i].Tag.Equals(ship))
                    {
                        label = labels[i];
                    }
                }

                // Set button
                chosen.Content = "x";

                ship.hits++;

                // Ship has been sunk
                if (ship.hits == ship.size)
                {
                    ship.sunk = true;
                    MessageBox.Show(message, caption);
                    label.Content = "";
                }
            }
        }

        /// <summary>
        /// The player's turn
        /// </summary>
        /// <param name="chosen">The Button selected to play</param>
        private void playerTurn(Button chosen)
        {
            // Set player turn
            turn = true;

            // Select button and mark it as hit or miss
            markButton(chosen);

            // Check for winner
            bool win = checkWinner("You sank all the ships!", "Winner!", name);

            // Computer move
            if (!win)
            {
                computerTurn();
            }
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
        /// Once a winner is determined, the wins/loss counter for the current player will be updated
        /// </summary>
        /// <param name="winnerName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Randomly places boat
        /// </summary>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="orientation">The orientation of the boat to be placed</param>
        private void setShip(Ship ship, Orientation orientation)
        {
            Random random = new Random();
            int index;
            int size = ship.size;

            bool isChosen;

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
                        if (index + i > 99 || buttonsAttack[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }

                } while (isChosen);

                // Set buttons
                for (int i = 0; i < size; i++)
                {
                    buttonsAttack[index + i].Tag = ship;
                    // FOR TESTING PURPOSES********************************
                    //buttonsAttack[index + i].Content = ship.name;
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
                    buttonsAttack[index + i].Tag = ship;
                    // FOR TESTING PURPOSES********************************
                    //buttonsAttack[index + i].Content = ship.name;
                }
            }
        }

        /// <summary>
        /// Show the highScoreWindow once the game is completed
        /// </summary>
        /// <param name="highScores">A list containing all the players who have previously played</param>
        private void showHighScore(List<string> highScores)
        {
            HighScoreWindow highScoreWindow = new HighScoreWindow(highScores);

            highScoreWindow.ShowDialog();
        }
    }
}
