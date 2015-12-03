using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        public void initialize(List<string> scores)
        {
            string[] player;

            string names = "Name" + Environment.NewLine;
            string wins = "Wins" + Environment.NewLine;
            string losses = "Losses" + Environment.NewLine;

            for (int i = 0; i < scores.Count; i++)
            {
                player = scores[i].Split(' ');
                names += player[0].Replace('_', ' ') + Environment.NewLine;
                wins += player[1] + Environment.NewLine;
                losses += player[2] + Environment.NewLine;
            }

            textBlockNames.Text = names;
            textBlockWins.Text = wins;
            textBlockLosses.Text = losses;
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            // Filename
            string path = @"score.txt";

            File.Delete(path);
            File.Create(path);

            textBlockNames.Text = "Name";
            textBlockWins.Text = "Wins";
            textBlockLosses.Text = "Losses";
        }
    }
}
