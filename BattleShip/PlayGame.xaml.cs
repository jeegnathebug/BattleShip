using System;
using System.Windows;
using System.Windows.Controls;

namespace BattleShip
{
    public partial class PlayGame : UserControl
    {
        private Button[] buttonsPlayer;
        private Button[] buttonsAttack;
        private Difficulty difficulty;

        private bool winner = false;

        // Computer's ships
        private bool aircraftCarrierComp = false;
        private bool battleshipComp = false;
        private bool submarineComp = false;
        private bool cruiserComp = false;
        private bool destroyerComp = false;

        private int aircraftCarrierCounterComp = 0;
        private int battleshipCounterComp = 0;
        private int submarineCounterComp = 0;
        private int cruiserCounterComp = 0;
        private int destroyerCounterComp = 0;

        // Human's ships
        private bool aircraftCarrier = false;
        private bool battleship = false;
        private bool submarine = false;
        private bool cruiser = false;
        private bool destroyer = false;

        private int aircraftCarrierCounter = 0;
        private int battleshipCounter = 0;
        private int submarineCounter = 0;
        private int cruiserCounter = 0;
        private int destroyerCounter = 0;

        public PlayGame(MainWindow mainWindow, Difficulty difficulty)
        {
            InitializeComponent();

            this.difficulty = difficulty;

            buttonsPlayer = new Button[100];
            PlayerShips.Children.CopyTo(buttonsPlayer, 0);

            // Set player ships
            for (int i = 0; i < 100; i++)
            {
                buttonsPlayer[i].Content = mainWindow.buttons[i].Content;
                buttonsPlayer[i].Tag = mainWindow.buttons[i].Tag;
            }

            // Set computer ships
            buttonsAttack = new Button[100];
            PlayerAttack.Children.CopyTo(buttonsAttack, 0);

            setShip(5, "Aircraft_Carrier");
            setShip(4, "Battleship");
            setShip(3, "Submarine");
            setShip(3, "Cruiser");
            setShip(2, "Destroyer");
        }

        private void button_Clicked(object sender, EventArgs e)
        {
            Button chosen = (Button)sender;

            // Select button and mark it as hit or miss
            chosen.IsEnabled = false;
            if (chosen.Tag == null)
            {
                chosen.Content = "o";
            }
            else
            {
                chosen.Content = "x";
                string boatName = chosen.Tag.ToString();

                switch (boatName)
                {
                    case "Aircraft_Carrier":
                        aircraftCarrierCounterComp++;
                        if (aircraftCarrierCounterComp == 5)
                        {
                            MessageBox.Show("You sunk my " + boatName, "Success");
                            aircraftCarrierComp = true;
                        }
                        break;
                    case "Battleship":
                        battleshipCounterComp++;
                        if (battleshipCounterComp == 4)
                        {
                            MessageBox.Show("You sunk my " + boatName, "Success");
                            battleshipComp = true;
                        }
                        break;
                    case "Submarine":
                        submarineCounterComp++;
                        if (submarineCounterComp == 3)
                        {
                            MessageBox.Show("You sunk my " + boatName, "Success");
                            submarineComp = true;
                        }
                        break;
                    case "Cruiser":
                        cruiserCounterComp++;
                        if (cruiserCounterComp == 3)
                        {
                            MessageBox.Show("You sunk my " + boatName, "Success");
                            cruiserComp = true;
                        }
                        break;
                    case "Destroyer":
                        destroyerCounterComp++;
                        if (destroyerCounterComp == 2)
                        {
                            MessageBox.Show("You sunk my " + boatName, "Success");
                            destroyerComp = true;
                        }
                        break;
                }
            }

            // Check for winner
            if (aircraftCarrierComp && battleshipComp && submarineComp && cruiserComp && destroyerComp)
            {
                // Disable all buttons
                for (int i = 0; i < 100; i++)
                {
                    buttonsAttack[i].IsEnabled = false;
                    buttonsPlayer[i].IsEnabled = false;
                }

                MessageBox.Show("You sank all the ships!", "Winner!");
                winner = true;
            }

            // Computer move
            if (!winner)
            {
                computerMove();
            }
        }

        private void computerMove()
        {
            Random random = new Random();
            int index;
            do
            {
                index = random.Next(0, 100);

            } while (!buttonsPlayer[index].IsEnabled);

            Button chosen = buttonsPlayer[index];

            // Select button and mark it as hit or miss
            chosen.IsEnabled = false;
            if (chosen.Tag == null)
            {
                chosen.Content = "o";
            }
            else
            {
                chosen.Content = "x";
                string boatName = chosen.Tag.ToString();

                switch (boatName)
                {
                    case "Aircraft_Carrier":
                        aircraftCarrierCounter++;
                        if (aircraftCarrierCounter == 5)
                        {
                            MessageBox.Show("You're " + boatName + " has been sunk!", "Oh no!");
                            aircraftCarrier = true;
                        }
                        break;
                    case "Battleship":
                        battleshipCounter++;
                        if (battleshipCounter == 4)
                        {
                            MessageBox.Show("You're " + boatName + " has been sunk!", "Oh no!");
                            battleship = true;
                        }
                        break;
                    case "Submarine":
                        submarineCounter++;
                        if (submarineCounter == 3)
                        {
                            MessageBox.Show("You're " + boatName + " has been sunk!", "Oh no!");
                            submarine = true;
                        }
                        break;
                    case "Cruiser":
                        cruiserCounter++;
                        if (cruiserCounter == 3)
                        {
                            MessageBox.Show("You're " + boatName + " has been sunk!", "Oh no!");
                            cruiser = true;
                        }
                        break;
                    case "Destroyer":
                        destroyerCounter++;
                        if (destroyerCounter == 2)
                        {
                            MessageBox.Show("You're " + boatName + " has been sunk!", "Oh no!");
                            destroyer = true;
                        }
                        break;
                }
            }

            // Check for winner
            if (aircraftCarrier && battleship && submarine && cruiser && destroyer)
            {
                MessageBox.Show("All of your ships have been sunk!", "Loser");
                winner = true;
            }
        }

        private void setShip(int size, string boatName)
        {
            Random random = new Random();

            int index = random.Next(10);
            bool isChosen;

            // Orientation is horizontal
            if (index % 2 == 0)
            {
                do
                {
                    isChosen = false;

                    do
                    {
                        index = random.Next(0, 100);

                    } while (((index + (size - 1)) % 10 < size - 1));


                    // Go through every button that will be chosen to see if it has already been chosen or not
                    for (int i = 0; i < size; i++)
                    {
                        if (buttonsAttack[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }

                } while (isChosen);

                // Disable buttons
                for (int i = 0; i < size; i++)
                {
                    buttonsAttack[index + i].Content = boatName;
                    buttonsAttack[index + i].Tag = boatName;
                }
            }
            // Orientation is vertical
            else
            {
                do
                {
                    isChosen = false;

                    do
                    {
                        index = random.Next(0, 100);

                    } while (((index + size) / 10) > 10 - size || isChosen);


                    // Go through every button that will be chosen to see if it has already been chosen or not
                    for (int i = 0; i < size * 10; i += 10)
                    {
                        if (buttonsAttack[index + i].Tag != null)
                        {
                            isChosen = true;
                        }
                    }

                } while (isChosen);

                // Disable buttons
                for (int i = 0; i < size * 10; i += 10)
                {
                    buttonsAttack[index + i].Content = boatName;
                    buttonsAttack[index + i].Tag = boatName;
                }
            }
        }
    }
}
