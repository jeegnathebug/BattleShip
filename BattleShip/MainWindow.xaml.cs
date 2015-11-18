using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    public enum Difficulty
    {
        Easy, Medium, Hard
    }

    public partial class MainWindow : Window
    {
        StartGame startGame;
        Shipyard shipyard;
        PlayGame playGame;
        Grid grid = new Grid();
        Difficulty difficulty;
        string name;

        public Button[] buttons;

        public MainWindow()
        {
            InitializeComponent();

            initializeGame();
        }

        private void initializeGame()
        {
            // Initialize window
            this.Content = grid;

            // Initialize main menu
            startGame = new StartGame();


            // Add start menu and event handler
            grid.Children.Add(startGame);
            startGame.buttonStart.IsDefault = true;

            // Get name and difficulty
            name = startGame.textBoxName.Text;
            if (startGame.radioButtonEasy.IsChecked.Value)
            {
                difficulty = Difficulty.Easy;
            }
            if (startGame.radioButtonMedium.IsChecked.Value)
            {
                difficulty = Difficulty.Medium;
            }
            if (startGame.radioButtonHard.IsChecked.Value)
            {
                difficulty = Difficulty.Hard;
            }

            // Once start is pressed
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
            shipyard.buttonSubmit.IsDefault = true;
            shipyard.HorizontalAlignment = HorizontalAlignment.Left;
            shipyard.VerticalAlignment = VerticalAlignment.Top;

            // Once submit is pressed
            shipyard.play += new EventHandler(start);
        }

        private void start(object sender, EventArgs e)
        {
            // Save buttons
            buttons = shipyard.buttons;

            // Close set up
            shipyard.Visibility = Visibility.Collapsed;

            // Resize window
            this.ResizeMode = ResizeMode.CanResize;
            this.WindowState = WindowState.Maximized;

            // Initialize game play phase
            playGame = new PlayGame(this, difficulty);

            // Add game field
            grid.Children.Add(playGame);
            playGame.HorizontalAlignment = HorizontalAlignment.Center;
            playGame.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
