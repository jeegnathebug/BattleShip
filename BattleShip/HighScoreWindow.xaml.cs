using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for HighScoreWindow.xaml
    /// </summary>
    public partial class HighScoreWindow : Window
    {
        public HighScoreWindow(List<string> scores)
        {
            InitializeComponent();

            initialize(scores);
        }

        /// <summary>
        /// Initializes window
        /// </summary>
        /// <param name="scores"></param>
        public void initialize(List<string> scores)
        {
            string[] player;

            string names = "NAME" + Environment.NewLine;
            string wins = "WINS" + Environment.NewLine;
            string losses = "LOSSES" + Environment.NewLine;

            for (int i = 0; i < scores.Count; i++)
            {
                player = scores[i].Split(' ');
                names += player[0].ToUpper().Replace('_', ' ') + Environment.NewLine;
                wins += player[1] + Environment.NewLine;
                losses += player[2] + Environment.NewLine;
            }

            textBlockNames.Text = names;
            textBlockWins.Text = wins;
            textBlockLosses.Text = losses;
        }

        /// <summary>
        /// Leaves form
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">The Event</param>
        private void buttonDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Clears the names/wins/losses on the screen and in the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            // Filename
            string path = @"../../score.txt";

            File.Delete(path);
            File.Create(path);

            textBlockNames.Text = "NAME";
            textBlockWins.Text = "WINS";
            textBlockLosses.Text = "LOSSES";
        }
    }
}
