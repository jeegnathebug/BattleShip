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
        private string name;
        private string winner;

        public HighScoreWindow(string name, string winner)
        {
            InitializeComponent();

            this.name = name;
            this.winner = winner;

            initialize();
        }

        /// <summary>
        /// Initializes window
        /// </summary>
        /// <param name="scores"></param>
        private void initialize()
        {
            List<string> scores = saveWins();
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
            Close();
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

        /// <summary>
        /// The wins/loss counter for the current player will be updated
        /// </summary>
        /// <returns></returns>
        private List<string> saveWins()
        {
            // Filename to save score
            string path = @"../../score.txt";

            // Create file if it does not exist
            if (!File.Exists(path))
            {
                FileStream stream = File.Create(path);
                stream.Close();
            }

            // Get all previous players
            List<string> previousPlayers = new List<string>(File.ReadAllLines(path));
            string[] previousPlayer = { name, "0", "0" };
            string[] playerNames;
            int index;

            int wins = 0;
            int losses = 0;

            // Get name and index of previous player
            playerNames = new string[previousPlayers.Count];

            for (index = 0; index < previousPlayers.Count; index++)
            {
                playerNames[index] = previousPlayers[index].Split(' ')[0];
            }

            // Find index of player
            index = Array.BinarySearch(playerNames, name);

            // Player already exists
            if (index > -1)
            {
                previousPlayer = previousPlayers[index].Split();
                previousPlayers.RemoveAt(index);
            }
            else
            {
                index = -(index + 1);
            }

            // Set wins or losses
            if (winner.Equals(""))
            {
                losses = int.Parse(previousPlayer[2]) + 1;
            }
            else
            {
                wins = int.Parse(previousPlayer[1]) + 1;
            }

            // Add to array
            previousPlayers.Insert(index, name + " " + wins + " " + losses);

            // Write back to file
            File.WriteAllLines(path, previousPlayers);

            return previousPlayers;
        }
    }
}
