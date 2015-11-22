using System;
using System.Windows;
using System.Windows.Controls;


namespace BattleShip
{
    public partial class StartGame : UserControl
    {
        public event EventHandler play;

        public Difficulty difficulty;

        public StartGame()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxName.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("You must enter a name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (play != null)
                {
                    if (radioButtonEasy.IsChecked.Value)
                    {
                        difficulty = Difficulty.Easy;
                    }
                    else
                    {
                        difficulty = Difficulty.Hard;
                    }

                    play(this, e);
                }
            }
        }
    }
}