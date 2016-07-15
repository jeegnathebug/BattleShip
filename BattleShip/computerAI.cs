using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BattleShip
{
    /// <summary>
    /// Computer AI logic
    /// </summary>
    public class ComputerAI
    {
        private PlayGame playGame;

        // Computer's moves
        public List<int> computerMoves = new List<int>();

        // Computer's next possible moves
        public List<int> potentialAttacks = new List<int>();
        // Index of last hit
        public int lastHitIndex = -1;
        // Boat hit
        public Ship lastHitShip = new Ship(ShipName.AIRCRAFT_CARRIER, 99);

        // Difficulty
        public Difficulty difficulty;

        public ComputerAI(PlayGame playGame, Difficulty difficulty)
        {
            this.playGame = playGame;
            // Set difficulty
            this.difficulty = difficulty;
        }

        /// <summary>
        /// Chooses between defaultSet and list as the Button List to be used. If no buttons are enabled in list, defaultSet will be chosen
        /// </summary>
        /// <param name="list">The Button List</param>
        /// <param name="defaultSet">The default set of buttons</param>
        /// <returns>list, if not all the buttons are disabled, or defaultSet otherwise</returns>
        private List<int> chooseSet(List<int> list, List<int> defaultSet)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (playGame.buttonsPlayer[list[i]].IsEnabled)
                {
                    list.TrimExcess();
                    return list;
                }
            }

            defaultSet.TrimExcess();
            return defaultSet;
        }

        /// <summary>
        /// The computer's easy difficulty settings
        /// </summary>
        private Button computerEasy()
        {
            // Randomly select a button that has not been selected
            Random random = new Random();
            int index;

            do
            {
                index = random.Next(0, 100);

            } while (!playGame.buttonsPlayer[index].IsEnabled);

            Button chosen = playGame.buttonsPlayer[index];

            // Select button and mark it as hit or miss
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// The computer's hard difficulty settings
        /// </summary>
        private Button computerHard()
        {
            List<int> firstSet = new List<int>() { 0, 4, 8, 11, 15, 19, 22, 26, 33, 37, 40, 44, 48, 51, 55, 59, 62, 66, 73, 77, 80, 84, 88, 91, 95, 99 };
            List<int> secondSet = new List<int>() { 2, 6, 13, 17, 20, 24, 28, 31, 35, 39, 42, 46, 53, 57, 60, 64, 68, 71, 75, 79, 82, 86, 93, 97 };
            List<int> defaultSet = new List<int>() { 1, 3, 5, 7, 9, 10, 12, 14, 16, 18, 21, 23, 25, 27, 29, 30, 32, 34, 36, 38, 41, 43, 45, 47, 49, 50, 52, 54, 56, 58, 61, 63, 65, 67, 69, 70, 72, 74, 76, 78, 81, 83, 85, 87, 89, 90, 92, 94, 96, 98 };

            List<int> list;

            // Choose set to go through
            list = chooseSet(firstSet, defaultSet);

            if (list.Count != firstSet.Count)
            {
                list = chooseSet(secondSet, defaultSet);
            }

            Random random = new Random();
            int size = list.Capacity;
            int index = 0;

            do
            {
                index = random.Next(list.Count);

            } while (!playGame.buttonsPlayer[list[index]].IsEnabled);

            Button chosen = playGame.buttonsPlayer[list[index]];

            // Select button and mark it as hit or miss
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// Basically cheating
        /// </summary>
        private Button computerLegendary()
        {
            // Add all player's ships to hit list
            potentialAttacks.Clear();
            for (int i = 0; i < playGame.buttonsPlayer.Length; i++)
            {
                if (playGame.buttonsPlayer[i].Tag != null && playGame.buttonsPlayer[i].IsEnabled)
                {
                    potentialAttacks.Add(i);
                }
            }

            // Chose button from hit list
            Random random = new Random();
            Button chosen;
            int index;
            int percentage;
            int counter;

            // Choose a new seed for random number
            if (computerMoves.Count == 0)
            {
                counter = 10;
            }
            else
            {
                counter = computerMoves[computerMoves.Count - 1];
            }

            do
            {
                percentage = random.Next(1000);
                counter--;
            } while (counter > 0);

            // 25% chance of shooting randomly
            if (percentage <= 250)
            {
                return computerEasy();
            }
            // 75% chance of shooting at a player's ship
            else
            {
                counter = potentialAttacks.Count;
                do
                {
                    index = random.Next(0, counter);
                } while (!playGame.buttonsPlayer[potentialAttacks[index]].IsEnabled);

                chosen = playGame.buttonsPlayer[potentialAttacks[index]];

                potentialAttacks.RemoveAt(index);
            }

            computerMoves.Add(index);
            return chosen;
        }

        /// <summary>
        /// The computer's medium difficulty setting
        /// </summary>
        private Button computerMedium()
        {
            List<int> firstSet = new List<int>() { 0, 5, 11, 16, 22, 27, 33, 38, 44, 49, 50, 55, 61, 66, 72, 77, 83, 88, 94, 99 };
            List<int> secondSet = new List<int>() { 3, 8, 14, 19, 25, 30, 36, 41, 47, 52, 58, 63, 69, 74, 80, 85, 91, 96 };
            List<int> thirdSet = new List<int>() { 1, 6, 12, 17, 20, 23, 28, 31, 34, 39, 42, 45, 53, 56, 64, 67, 70, 75, 78, 81, 86, 89, 92, 97 };
            List<int> defaultSet = new List<int> { 2, 4, 7, 9, 10, 13, 15, 18, 21, 24, 26, 29, 32, 35, 37, 40, 43, 46, 48, 51, 54, 57, 59, 60, 62, 65, 68, 71, 73, 76, 79, 82, 84, 87, 90, 93, 95, 98 };

            List<int> list;

            // Choose set to go through
            list = chooseSet(firstSet, defaultSet);

            if (list.Count != firstSet.Count)
            {
                list = chooseSet(secondSet, defaultSet);
                if (list.Count != secondSet.Count)
                {
                    list = chooseSet(thirdSet, defaultSet);
                }
            }

            Random random = new Random();
            int size = list.Capacity;
            int index = 0;

            do
            {
                index = random.Next(list.Count);

            } while (!playGame.buttonsPlayer[list[index]].IsEnabled);

            Button chosen = playGame.buttonsPlayer[list[index]];

            // Select button and mark it as hit or miss
            computerMoves.Add(index);

            return chosen;
        }

        /// <summary>
        /// The computer's turn, where the computer will act according to the difficulty chosen
        /// </summary>
        public Button computerTurn()
        {
            Button chosen = null;

            // Legendary mode
            if (difficulty.Equals(Difficulty.Legendary))
            {
                chosen = computerLegendary();
            }
            else
            {
                // Last move was a hit
                if (computerMoves.Count != 0 && lastHitIndex > -1)
                {
                    chosen = killerMode();
                }
                // Go to default difficulty move
                if (chosen == null)
                {
                    // Choose difficulty
                    if (difficulty.Equals(Difficulty.Easy))
                    {
                        chosen = computerEasy();
                    }
                    if (difficulty.Equals(Difficulty.Medium))
                    {
                        chosen = computerMedium();
                    }
                    if (difficulty.Equals(Difficulty.Hard))
                    {
                        chosen = computerHard();
                    }
                }
            }

            // If move was a hit
            if (chosen.Tag != null)
            {
                lastHitIndex = Array.IndexOf(playGame.buttonsPlayer, chosen);
                lastHitShip = (Ship)chosen.Tag;
            }

            return chosen;
        }

        /// <summary>
        /// Once a ship has been hit, the computer will target the buttons around it
        /// </summary>
        private Button killerMode()
        {
            // If ship being attacked has sunk
            if (lastHitShip.sunk)
            {
                lastHitIndex = -1;
                potentialAttacks.Clear();
                return null;
            }

            // Add to hit list
            if ((lastHitIndex % 9 != 0) && playGame.buttonsPlayer[lastHitIndex + 1].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex + 1);
            }
            if ((lastHitIndex % 10 != 0) && playGame.buttonsPlayer[lastHitIndex - 1].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex - 1);
            }
            if (((lastHitIndex + 10) <= 99) && playGame.buttonsPlayer[lastHitIndex + 10].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex + 10);
            }
            if (((lastHitIndex - 10) >= 0) && playGame.buttonsPlayer[lastHitIndex - 10].IsEnabled)
            {
                potentialAttacks.Add(lastHitIndex - 10);
            }

            // Chose button from hit list
            int index;

            if (potentialAttacks.Count != 0)
            {
                index = potentialAttacks[0];

                while (!playGame.buttonsPlayer[index].IsEnabled)
                {
                    potentialAttacks.RemoveAt(0);
                    if (potentialAttacks.Count != 0)
                    {
                        index = potentialAttacks[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                Button chosen = playGame.buttonsPlayer[index];
                computerMoves.Add(index);
                potentialAttacks.RemoveAt(0);

                return chosen;
            }
            return null;
        }
    }
}
