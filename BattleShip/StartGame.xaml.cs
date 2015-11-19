using System;
using System.Windows;
using System.Windows.Controls;


namespace BattleShip
{
    public partial class StartGame : UserControl
    {
        public event EventHandler play; 

        public StartGame()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxName.Text.Trim() == "")
            {
                MessageBox.Show("You must enter a name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (play != null)
                {
                    play(this, e);
                }
            }
        }
    }
}