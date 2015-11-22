using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    public enum Difficulty
    {
        Easy, Hard
    }

    public partial class MainWindow : Window
    {
        Grid grid = new Grid();

        StartGame startGame;
        Shipyard shipyard;
        PlayGame playGame;

        public Difficulty difficulty;

        string name;

        public MainWindow()
        {
            InitializeComponent();
            initializeGame();
        }

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

        private void setup(object sender, EventArgs e)
        {
            // Get name and difficulty
            name = startGame.textBoxName.Text.Trim();
            difficulty = startGame.difficulty;

            // Close start menu
            grid.Children.Clear();

            // Resize window
            MinHeight = 650;
            MinWidth = 800;

            // Initialize setup phase
            shipyard = new Shipyard();

            // Add shipyard
            grid.Children.Add(shipyard);
            shipyard.HorizontalAlignment = HorizontalAlignment.Left;
            shipyard.VerticalAlignment = VerticalAlignment.Top;

            // Once buttonSubmit is clicked
            shipyard.play += new EventHandler(start);
        }

        private void start(object sender, EventArgs e)
        {
            // Close set up
            grid.Children.Clear();

            // Resize window
            ResizeMode = ResizeMode.CanResize;
            WindowState = WindowState.Maximized;

            // Initialize game play phase
            playGame = new PlayGame(difficulty, shipyard.buttons);

            // Add game field
            grid.Children.Add(playGame);
            playGame.HorizontalAlignment = HorizontalAlignment.Center;
            playGame.VerticalAlignment = VerticalAlignment.Center;

            // Once buttonRestart is clicked
            playGame.restart += new EventHandler(restart);
        }

        private void restart(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
    }
}
