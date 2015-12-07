using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid grid = new Grid();

        private StartGame startGame;
        private Shipyard shipyard;
        private PlayGame playGame;

        /// <summary>
        /// Initializes window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            initializeGame();
        }

        /// <summary>
        /// Initializes startup phase
        /// </summary>
        private void initializeGame()
        {
            // Initialize window
            Content = grid;

            // Initialize main menu
            startGame = new StartGame();

            // Add start menu
            grid.Children.Add(startGame);

            // Once buttonStart is clicked
            startGame.play += new EventHandler(setup);
        }

        /// <summary>
        /// Initializes setup phase
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void setup(object sender, EventArgs e)
        {
            // Close start menu
            grid.Children.Clear();

            // Initialize setup phase
            shipyard = new Shipyard();

            // Add shipyard
            grid.Children.Add(shipyard);

            // Once buttonSubmit is clicked
            shipyard.play += new EventHandler(start);
        }

        /// <summary>
        /// Initializes gameplay phase
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void start(object sender, EventArgs e)
        {
            // Close set up
            grid.Children.Clear();

            // Initialize game play phase
            playGame = new PlayGame(startGame.difficulty, shipyard.buttons, startGame.textBoxName.Text.Trim(), shipyard.ships);

            // Add game field
            grid.Children.Add(playGame);

            // Once buttonRestart is clicked
            playGame.done += new EventHandler(restart);
        }

        /// <summary>
        /// Restarts game
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void restart(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
    }
}
