using Server.Classes;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameClient.PlayerUI
{
    /// <summary>
    /// Interaction logic for PlayerUserControl.xaml
    /// </summary>
    public partial class PlayerUserControl : UserControl
    {
        private Game gameGet15;         // Game containing the logic and rules 
        private Thread outputThread;    // Thread for receiving data from server
        private TcpClient connection;   // Client to establish connection      
        private NetworkStream stream;   // Network data stream                 
        private BinaryWriter writer;    // Facilitates writing to the stream    
        private BinaryReader reader;    // Facilitates reading from the stream  
        private int idNumber;           // Player's number                   
        private bool myTurn;            // Is it this player's turn 
        public PlayerUserControl()
        {
            InitializeComponent();
            // start game
            gameGet15 = new Game();
            // disable input until the two players connected
            EnableInput(false);
            // make connection to server and get the associated network stream                                  
            connection = new TcpClient("127.0.0.1", 50000);
            stream = connection.GetStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            // start a new thread for sending and receiving messages
            outputThread = new Thread(new ThreadStart(Run));
            outputThread.Start();
        }

        #region Display Messages

        /// <summary>
        /// DisplayEnteredNumber sets TxtEnterNumber's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayEnteredNumber(string message)
        {
            // if modifying TxtEnterNumber is not thread safe
            if (!TxtEnterNumber.Dispatcher.CheckAccess())
            {
                TxtEnterNumber.Dispatcher.Invoke(new Action(() => TxtEnterNumber.Text = message));
            }
            else
            {
                // safe to modify TxtEnterNumber in current thread
                TxtEnterNumber.Text = message;
            }
        }

        /// <summary>
        ///  DisplayNameTurn sets TxtNameTurn's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayNameTurn(string message)
        {
            // if modifying TxtNameTurn is not thread safe
            if (!TxtNameTurn.Dispatcher.CheckAccess())
            {
                TxtNameTurn.Dispatcher.Invoke(new Action(() => TxtNameTurn.Text = message));
            }
            else
            {
                // safe to modify TxtNameTurn in current thread
                TxtNameTurn.Text = message;
            }
        }

        /// <summary>
        /// DisplayPlayerNumbers sets TxtYourNumbers's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayPlayerNumbers(string message)
        {
            // if modifying TxtYourNumbers is not thread safe
            if (!TxtYourNumbers.Dispatcher.CheckAccess())
            {
                TxtYourNumbers.Dispatcher.Invoke(new Action(() => TxtYourNumbers.Text = message));
            }
            else
            {
                // safe to modify TxtYourNumbers in current thread
                TxtYourNumbers.Text = message;
            }
        }

        /// <summary>
        /// DisplayOponentNumbers sets TxtOponentsNumbers's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayOponentNumbers(string message)
        {
            // if modifying TxtOponentsNumbers is not thread safe
            if (!TxtOponentsNumbers.Dispatcher.CheckAccess())
            {
                TxtOponentsNumbers.Dispatcher.Invoke(new Action(() => TxtOponentsNumbers.Text = message));
            }
            else
            {
                // safe to modify TxtOponentsNumbers in current thread
                TxtOponentsNumbers.Text = message;
            }
        }

        /// <summary>
        /// DisplayRemainingNumbers sets TxtRemainingNumbers's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayRemainingNumbers(string message)
        {
            // if modifying TxtRemainingNumbers is not thread safe
            if (!TxtRemainingNumbers.Dispatcher.CheckAccess())
            {
                TxtRemainingNumbers.Dispatcher.Invoke(new Action(() => TxtRemainingNumbers.Text = message));
            }
            else
            {
                // safe to modify TxtRemainingNumbers in current thread
                TxtRemainingNumbers.Text = message;
            }
        }

        /// <summary>
        /// DisplayWinningCombination sets TxtWiningCombination's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayWinningCombination(string message, bool gameWon)
        {
            // if modifying TxtWiningCombination is not thread safe
            if (!TxtWiningCombination.Dispatcher.CheckAccess())
            {
                TxtWiningCombination.Dispatcher.Invoke(new Action(() =>
                {
                    TxtWiningCombination.Text = message;
                    if (gameWon == true)
                    {
                        // display the winning combination in player's color
                        TxtWiningCombination.Foreground = new SolidColorBrush(Colors.Blue);
                    }
                    else
                    {
                        // display the winning combination in oponent's color
                        TxtWiningCombination.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }));
            }
            else
            {
                // safe to modify TxtWiningCombination in current thread
                TxtWiningCombination.Text = message;
                if (gameWon == true)
                {
                    // display the winning combination in player's color
                    TxtWiningCombination.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    // display the winning combination in oponent's color
                    TxtWiningCombination.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }
        #endregion

        /// <summary>
        /// EnableInput enables/disables TxtEnterNumber and BtnSubmit's property in a thread-safe manner
        /// </summary>
        public void EnableInput(bool value)
        {
            // if disabling TxtEnterNumber and BtnSubmit are not thread safe
            if (!Application.Current.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    TxtEnterNumber.IsEnabled = value;
                    BtnSubmit.IsEnabled = value;
                }));
            }
            else
            {
                // safe to disable TxtEnterNumber and BtnSubmit in current thread
                TxtEnterNumber.IsEnabled = value;
                BtnSubmit.IsEnabled = value;
            }
        }

        /// <summary>
        /// Control thread that receives updates from the server
        /// </summary> 
        public void Run()
        {
            // first get players's number               
            idNumber = reader.ReadInt32();
            myTurn = (idNumber == 0 ? true : false);
            string message = myTurn == true ? "Your turn!" : "Oponent's turn!";
            DisplayNameTurn(message);
            EnableInput(true);

            // process incoming messages
            try
            {
                // receive messages sent to client       
                while (gameGet15.CheckState() == false)
                {
                    ProcessMessage(reader.ReadString());
                }
                // game over disable input
                EnableInput(false);
                // display that the game is draw
                if (gameGet15.WinnerIdNumber == -1)
                {
                    DisplayNameTurn("It's a draw!");
                }
                else
                {
                    // set the wining combination to string
                    string winingCombinationMessage = $"{gameGet15.WiningCombination.Item1} + {gameGet15.WiningCombination.Item2} + {gameGet15.WiningCombination.Item3} = 15";
                    // display that you won
                    if (idNumber == gameGet15.WinnerIdNumber)
                    {
                        DisplayNameTurn("You won!");
                        DisplayWinningCombination(winingCombinationMessage, true);
                    }
                    // display that you lost
                    else
                    {
                        DisplayNameTurn("You lost!");
                        DisplayWinningCombination(winingCombinationMessage, false);
                    }
                }
            }
            catch (IOException)
            {
                // if server is down
                DisplayNameTurn("Server is down!");
                MessageBox.Show("Server is down, game over!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                EnableInput(false);
            }
        }

        /// <summary>
        /// Process the messages sent to client
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessage(string message)
        {
            // if the move the player sent to the server is valid
            if (message == "Valid")
            {
                DisplayNameTurn("Oponent's turn!");
                // disable input until it is this player's turn
                myTurn = false;
                EnableInput(false);
            }
            // if the move is invalid, display that and it is now this player's turn again
            else if (message == "Invalid")
            {
                MessageBox.Show("The input is not valid!", "Wrong Input", MessageBoxButton.OK, MessageBoxImage.Information);
                myTurn = true;
                // enable input
                EnableInput(true);
            }
            // if opponent moved
            else if (message == "Opponent moved")
            {
                // reveive their number, add it to the oponent's list,
                // remove it from the remaing numbers and update the display
                //with the current information
                byte number = reader.ReadByte();

                gameGet15.AddNumberPlayer(number, (idNumber + 1) % 2);
                gameGet15.RemoveFromTheRemainingNumbers(number);
                DisplayOponentNumbers(gameGet15.GetPlayerNumbersString((idNumber + 1) % 2));
                DisplayRemainingNumbers(gameGet15.GetRemainingNumbersString());

                // it is now this player's turn
                DisplayNameTurn("Your turn!");
                // enable input
                EnableInput(true);
                myTurn = true;
            }
        }

        /// <summary>
        /// Send the server the number of the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (TxtEnterNumber.Text != "")
            {
                // receive the input
                byte number;
                bool isValid = byte.TryParse(TxtEnterNumber.Text, out number);
                if (isValid && myTurn && gameGet15.NumberStillNotPlayed(number))
                {
                    // send the location of the move to the server
                    writer.Write(number);

                    // remove from the remaining numbers
                    gameGet15.RemoveFromTheRemainingNumbers(number);

                    // add to my numbers
                    gameGet15.AddNumberPlayer(number, idNumber);

                    // it is now the other player's turn
                    myTurn = false;
                    // display current information
                    DisplayEnteredNumber("");
                    DisplayPlayerNumbers(gameGet15.GetPlayerNumbersString(idNumber));
                    DisplayRemainingNumbers(gameGet15.GetRemainingNumbersString());

                    // disable input until it is this player's turn
                    EnableInput(false);

                }
                else
                {
                    MessageBox.Show("The input is not valid!", "Wrong Input", MessageBoxButton.OK, MessageBoxImage.Information);
                    DisplayEnteredNumber("");
                }
            }
        }
    }
}
