using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Windows.SystemParameters;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StartGame startGame;
        GameField gameFieldPlayerSetup;
        GameField gameFieldPlayerAttack;
        Shipyard shipyard;
        Grid grid = new Grid();

        public MainWindow()
        {
            InitializeComponent();

            this.Content = grid;

            setup();
            start();
        }


        private void setup()
        {
            // Initialize game components
            startGame = new StartGame();
            gameFieldPlayerSetup = new GameField();
            gameFieldPlayerAttack = new GameField();
            shipyard = new Shipyard();

            // Add start menu and event handler
            grid.Children.Add(startGame);
            startGame.play += new EventHandler(onPlay);
        }

        private void onPlay(object sender, EventArgs e)
        {
            // Hide start menu
            startGame.Visibility = Visibility.Hidden;

            // Resize window
            this.ResizeMode = ResizeMode.CanResize;
            this.MinHeight = 550;
            this.MinWidth = SystemParameters.FullPrimaryScreenWidth;
            this.WindowState = WindowState.Maximized;

            // Add shipyard
            grid.Children.Add(shipyard);
            shipyard.HorizontalAlignment = HorizontalAlignment.Left;
            shipyard.VerticalAlignment = VerticalAlignment.Center;

            // Add game field
            grid.Children.Add(gameFieldPlayerSetup);
            gameFieldPlayerSetup.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Children.Add(gameFieldPlayerAttack);
            gameFieldPlayerAttack.HorizontalAlignment = HorizontalAlignment.Right;
        }

        private void start()
        {

        }
    }
}
