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

        Difficulty difficulty;

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

            // Get name and difficulty
            name = startGame.textBoxName.Text.Trim();
            if (startGame.radioButtonEasy.IsChecked.Value)
            {
                difficulty = Difficulty.Easy;
            }
            if (startGame.radioButtonHard.IsChecked.Value)
            {
                difficulty = Difficulty.Hard;
            }

            // Add event handler
            startGame.play += new EventHandler(setup);
        }

        private void setup(object sender, EventArgs e)
        {
            // Close start menu
            startGame.Visibility = Visibility.Hidden;

            // Resize window
            this.MinHeight = 650;
            this.MinWidth = 800;

            // Initialize setup phase
            shipyard = new Shipyard();

            // Add shipyard
            grid.Children.Add(shipyard);
            shipyard.HorizontalAlignment = HorizontalAlignment.Left;
            shipyard.VerticalAlignment = VerticalAlignment.Top;

            // Once submit is pressed
            shipyard.play += new EventHandler(start);
        }

        private void start(object sender, EventArgs e)
        {
            // Close set up
            shipyard.Visibility = Visibility.Collapsed;

            // Resize window
            this.ResizeMode = ResizeMode.CanResize;
            this.WindowState = WindowState.Maximized;

            // Initialize game play phase
            playGame = new PlayGame(difficulty, shipyard.buttons);

            // Add game field
            grid.Children.Add(playGame);
            playGame.HorizontalAlignment = HorizontalAlignment.Center;
            playGame.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
