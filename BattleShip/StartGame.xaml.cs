using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    public enum Difficulty
    {
        Easy, Medium, Hard, Legendary
    }

    /// <summary>
    /// Interaction logic for StartGame.xaml
    /// </summary>
    public partial class StartGame : UserControl
    {
        public event EventHandler play;

        public Difficulty difficulty;

        /// <summary>
        /// Initialize startup phase
        /// </summary>
        public StartGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Submits name and difficulty. If no name is provided "anonymous" will be used
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
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
                    if (radioButtonMedium.IsChecked.Value)
                    {
                        difficulty = Difficulty.Medium;
                    }
                    if (radioButtonHard.IsChecked.Value)
                    {
                        difficulty = Difficulty.Hard;
                    }
                    if (radioButtonLegendary.IsChecked.Value)
                    {
                        difficulty = Difficulty.Legendary;
                    }

                    play(this, e);
                }
            }
        }
    }
}
