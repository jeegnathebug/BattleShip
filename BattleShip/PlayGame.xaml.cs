using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for PlayGame.xaml
    /// </summary>
    public partial class PlayGame : UserControl
    {
        public event EventHandler done;
        private ComputerAI computerAI;
        private HighScoreWindow highScoreWindow;
        private Common common;
        private MainWindow main;

        // Player name
        private string name;

        // True if player's turn
        private bool turn = true;

        // Player's ship field
        public Button[] buttonsPlayer;
        // Computer's ship field
        public Button[] buttonsComputer;

        // Player ships
        private Ship[] shipsPlayer;
        // Computer ships
        private Ship[] shipsComputer;

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
        public PlayGame(Difficulty difficulty, Button[] buttons, string name, Ship[] ships, MainWindow main)
        {
            InitializeComponent();

            this.main = main;
            // Set player name
            this.name = name.ToLower().Replace(' ', '_');

            // Set player and computer's ships
            shipsPlayer = ships;
            shipsComputer = new Ship[] {
                                        new Ship(ShipName.AIRCRAFT_CARRIER, 5),
                                        new Ship(ShipName.BATTLESHIP, 4),
                                        new Ship(ShipName.SUBMARINE, 3),
                                        new Ship(ShipName.CRUISER, 3),
                                        new Ship(ShipName.DESTROYER, 2)
            };

            // Set button field arrays
            buttonsPlayer = new Button[100];
            PlayerShips.Children.CopyTo(buttonsPlayer, 0);

            buttonsComputer = new Button[100];
            ComputerShips.Children.CopyTo(buttonsComputer, 0);

            common = new Common(ComputerShips, buttonsComputer);
            computerAI = new ComputerAI(this, difficulty);

            initializeGame(buttons);
        }

        /// <summary>
        /// Initializes gameplay phase
        /// </summary>
        /// <param name="buttons">The submitted placement of ships</param>
        private void initializeGame(Button[] buttons)
        {
            // Set player buttons
            for (int i = 0; i < 100; i++)
            {
                buttonsPlayer[i].Opacity = buttons[i].Opacity;
                buttonsPlayer[i].Tag = buttons[i].Tag;
            }

            Label[] labelsPlayer = new Label[] { labelPlayerAircraftCarrier, labelPlayerBattleship, labelPlayerSubmarine, labelPlayerCruiser, labelPlayerDestroyer };
            Label[] labelsComputer = new Label[] { labelComputerAircraftCarrier, labelComputerBattleship, labelComputerSubmarine, labelComputerCruiser, labelComputerDestroyer };

            Random random = new Random();

            for (int i = 0; i < shipsPlayer.Length; i++)
            {
                // Set ship images
                shipsComputer[i].image = shipsPlayer[i].image;
                PlayerShips.Children.Add(shipsPlayer[i].image);

                // Set labels
                labelsComputer[i].Tag = shipsComputer[i];
                labelsPlayer[i].Tag = shipsPlayer[i];

                // Set computer ships
                do
                {
                    common.setShip(shipsComputer[i], random.Next(0, 100), common.randomOrientation(i), true, true);
                } while (!shipsComputer[i].placed);
            }
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
                Console.Beep(500, 250);
                //MessageBox.Show("You must enter coordinates first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (!buttonsComputer[row + column].IsEnabled)
                {
                    MessageBox.Show("You've already shot there", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Play button
                    playerTurn(buttonsComputer[row + column]);
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
            done?.Invoke(this, e);
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
                for (int i = 0; i < shipsComputer.Length; i++)
                {
                    sunk &= shipsComputer[i].sunk;
                }
            }
            // Computer turn
            else
            {
                // Check if all player ships have been sunk
                for (int i = 0; i < shipsPlayer.Length; i++)
                {
                    sunk &= shipsPlayer[i].sunk;
                }
            }

            if (sunk)
            {
                // Disable all buttons
                disableButtons();

                MessageBox.Show(message, caption);

                // Update file and show highscores
                highScoreWindow = new HighScoreWindow(name, winnerName);
                highScoreWindow.Owner = main;
                highScoreWindow.ShowDialog();

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
            for (int i = 0; i < shipsComputer.Length; i++)
            {
                if (!shipsComputer[i].sunk)
                {
                    ComputerShips.Children.Insert(0, shipsComputer[i].image);
                }
            }

            Coordinate.Children.CopyTo(buttonsCoordinate, 0);

            for (int i = 0; i < 100; i++)
            {
                buttonsComputer[i].IsEnabled = false;
                buttonsPlayer[i].IsEnabled = false;
                buttonsComputer[i].Opacity = 0;
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
                Uri src = new Uri(@"Resources\miss.png", UriKind.Relative);
                BitmapImage img = new BitmapImage(src);
                image.Source = img;

                // Set button
                setHit(chosen, image);
            }
            // Shot hit
            else
            {
                Uri src = new Uri(@"Resources\hit.png", UriKind.Relative);
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
                    message = "Your " + boatName.ToLower().Replace("_", " ") + " has been sunk!";
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
                    ComputerShips.Children.Insert(0, ship.image);
                }
            }

            // Check for winner
            bool win = checkWinner("You sank all the ships!", "Winner!", name);

            // Computer move
            if (!win)
            {
                // Set computer turn
                turn = false;
                markButton(computerAI.computerTurn());

                // Check for winner
                checkWinner("All of your ships have been sunk!", "Loser", "");
            }
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
                ComputerShips.Children.Add(image);
            }
            else
            {
                PlayerShips.Children.Add(image);
            }
        }
    }
}
