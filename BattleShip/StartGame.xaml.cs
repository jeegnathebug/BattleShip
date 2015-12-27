using System;
using System.Linq;
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
        public string name;

        /// <summary>
        /// Initialize startup phase
        /// </summary>
        public StartGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Submits name and difficulty. If no name is provided, "anonymous" will be used
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            name = textBoxName.Text.Trim();
            if (name == "")
            {
                name = "anonymous";
            }

            if (play != null)
            {
                // Get difficulty
                RadioButton checkedButton = DifficultyPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.Value);
                difficulty = (Difficulty)int.Parse(checkedButton.Tag.ToString());

                play(this, e);
            }
        }
    }
}
