using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for PlayGame.xaml
    /// </summary>
    public partial class PlayGame : UserControl
    {
        public event EventHandler done;
        private ComputerAI ComputerAI;

        // Player name
        private string name;

        // True if player's turn
        private bool turn = true;

        // Player's ship field
        public Button[] buttonsPlayer;
        // Computer's ship field
        public Button[] buttonsAttack;

        // Player ships
        private Ship aircraftCarrierPlayer;
        private Ship battleshipPlayer;
        private Ship submarinePlayer;
        private Ship cruiserPlayer;
        private Ship destroyerPlayer;
        // Computer ships
        public Ship aircraftCarrierComputer = new Ship(ShipName.AIRCRAFT_CARRIER, 5);
        public Ship battleshipComputer = new Ship(ShipName.BATTLESHIP, 4);
        public Ship submarineComputer = new Ship(ShipName.SUBMARINE, 3);
        public Ship cruiserComputer = new Ship(ShipName.CRUISER, 3);
        public Ship destroyerComputer = new Ship(ShipName.DESTROYER, 2);

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

            ComputerAI = new ComputerAI(this, difficulty);

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

            // Set player buttons
            for (int i = 0; i < 100; i++)
            {
                buttonsPlayer[i].Opacity = buttons[i].Opacity;
                buttonsPlayer[i].Tag = buttons[i].Tag;
            }

            // Set player ships
            aircraftCarrierPlayer = ships[0];
            battleshipPlayer = ships[1];
            submarinePlayer = ships[2];
            cruiserPlayer = ships[3];
            destroyerPlayer = ships[4];

            Ship[] shipsComputer = new Ship[] { aircraftCarrierComputer, battleshipComputer, submarineComputer, cruiserComputer, destroyerComputer };
            Label[] labels = new Label[] { labelPlayerAircraftCarrier, labelPlayerBattleship, labelPlayerSubmarine, labelPlayerCruiser, labelPlayerDestroyer };
            Label[] labelsComputer = new Label[] { labelComputerAircraftCarrier, labelComputerBattleship, labelComputerSubmarine, labelComputerCruiser, labelComputerDestroyer };

            for (int i = 0; i < ships.Length; i++)
            {

                // Add images
                setImage(ships[i], PlayerShips, buttonsPlayer, true);
                setImage(ships[i], PlayerShips, buttonsPlayer, true);

                // Set computer ship images
                shipsComputer[i].image = ships[i].image;

                // Set labels
                labelsComputer[i].Tag = shipsComputer[i];
                labels[i].Tag = ships[i];

                // Set computer ships
                setShip(shipsComputer[i], randomOrientation(i + 1));
            }
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
        public bool checkWinner(string message, string caption, string winnerName)
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
        /// Makes all buttons opaque and allows player to choose whether to restart or exit the game
        /// </summary>
        private void disableButtons()
        {
            Button[] buttonsCoordinate = new Button[20];

            // Set computer's non displayed ships
            if (!aircraftCarrierComputer.sunk)
            {
                PlayerAttack.Children.Insert(0, aircraftCarrierComputer.image);
            }
            if (!battleshipComputer.sunk)
            {
                PlayerAttack.Children.Insert(0, battleshipComputer.image);
            }
            if (!submarineComputer.sunk)
            {
                PlayerAttack.Children.Insert(0, submarineComputer.image);
            }
            if (!cruiserComputer.sunk)
            {
                PlayerAttack.Children.Insert(0, cruiserComputer.image);
            }
            if (!destroyerComputer.sunk)
            {
                PlayerAttack.Children.Insert(0, destroyerComputer.image);
            }

            Coordinate.Children.CopyTo(buttonsCoordinate, 0);

            for (int i = 0; i < 100; i++)
            {
                buttonsAttack[i].Opacity = 0;
                buttonsPlayer[i].Opacity = 0;
            }

            Coordinate.Visibility = Visibility.Collapsed;
            Attack.Visibility = Visibility.Collapsed;
            buttonsEnd.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Marks chosen button as a hit or miss, and displays message if ship has been sunk
        /// </summary>
        /// <param name="chosen">The Button chosen as a shot</param>
        public void markButton(Button chosen)
        {
            // Select button
            chosen.Opacity = 0;
            chosen.IsEnabled = false;

            Ship ship = (Ship)chosen.Tag;
            Image image = new Image();

            // Shot missed
            if (ship == null)
            {
                Uri src = new Uri(@"miss.png", UriKind.Relative);
                BitmapImage img = new BitmapImage(src);
                image.Source = img;

                // Set button
                setHit(chosen, image);
            }
            // Shot hit
            else
            {
                Uri src = new Uri(@"hit.png", UriKind.Relative);
                BitmapImage img = new BitmapImage(src);
                image.Source = img;

                // Set messages to display
                string boatName = ship.name.ToString();
                string message, caption;

                // Player turn
                if (turn)
                {
                    message = "You sunk my " + boatName.ToLower().Replace("_", " ");
                    caption = "Success";
                }
                // Computer turn
                else
                {
                    message = "You're " + boatName.ToLower().Replace("_", " ") + " has been sunk!";
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
                setHit(chosen, image);

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
            chosen.Opacity = 0;
            markButton(chosen);

            // Show ship image once it has been sunk
            object tag = chosen.Tag;
            if (tag != null)
            {
                Ship ship = (Ship)tag;
                if (ship.sunk)
                {
                    PlayerAttack.Children.Insert(0, ship.image);
                }
            }

            // Check for winner
            bool win = checkWinner("You sank all the ships!", "Winner!", name);

            // Computer move
            if (!win)
            {
                // Set computer turn
                turn = false;
                ComputerAI.computerTurn();
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
                return Orientation.HORIZONTAL;
            }
            else
            {
                return Orientation.VERTICAL;
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
            string path = @"../../score.txt";

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
        /// Applies the given image to the given button to either one of the button fields depending on the turn
        /// </summary>
        private void setHit(Button button, Image image)
        {
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);

            if (turn)
            {
                PlayerAttack.Children.Add(image);
            }
            else
            {
                PlayerShips.Children.Add(image);
            }
        }

        /// <summary>
        /// Sets image of placed ship on button field
        /// </summary>
        /// <param name="ship">The ship to be placed</param>
        /// <param name="grid">The grid to set the images on</param>
        /// <param name="buttons">The array of buttons to use</param>
        private void setImage(Ship ship, Grid grid, Button[] buttons, bool show)
        {
            // Copy image
            Image image = new Image();
            image.Source = ship.image.Source;
            image.Stretch = ship.image.Stretch;
            int index = ship.location[0];

            // Set properties
            int span = ship.size;
            int row = Grid.GetRow(buttons[index]);
            int column = Grid.GetColumn(buttons[index]);
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);

            if (ship.orientation.Equals(Orientation.VERTICAL))
            {
                // Rotate image
                image.LayoutTransform = new RotateTransform(90.0, 0, 0);
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

            if (show)
            {
                // Add image to location
                grid.Children.Add(image);
            }

            ship.image = new Image();
            ship.image.Stretch = image.Stretch;
            ship.image.Source = image.Source;
            ship.image.Height = image.Height;
            ship.image.Width = image.Width;
            ship.image.LayoutTransform = image.LayoutTransform;
            Grid.SetRow(ship.image, row);
            Grid.SetColumn(ship.image, column);
            Grid.SetRowSpan(ship.image, Grid.GetRowSpan(image));
            Grid.SetColumnSpan(ship.image, Grid.GetColumnSpan(image));
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
            if (orientation.Equals(Orientation.HORIZONTAL))
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
                    ship.location.Add(index + i);
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
                    ship.location.Add(index + i);
                }
            }

            ship.orientation = orientation;
            ship.location.Sort();

            setImage(ship, PlayerAttack, buttonsAttack, false);
            setImage(ship, PlayerAttack, buttonsAttack, false);
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
