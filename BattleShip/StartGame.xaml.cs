using System;
using System.Windows;
using System.Windows.Controls;


namespace BattleShip
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StartGame : UserControl
    {
        public event EventHandler play; 

        public StartGame()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (play != null)
            {
                play(this, e);
            }
        }
    }
}