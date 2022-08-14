using Server.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentPlayer;          // Keep track of whose turn it is
        private Player[] players;           // Two Players array
        internal Game gameGet15;            // Game containing the logic and rules 
        private TcpListener listener;       // Listen for client connection 
        private Thread[] playerThreads;     // Threads for client interaction 
        private Thread getPlayers;          // Thread for acquiring client connections
        internal bool disconnected = false; // True if the server closes  
        public MainWindow()
        {
            // initialize variables and thread for receiving clients
            InitializeComponent();
            currentPlayer = 0;
            players = new Player[2];
            playerThreads = new Thread[2];
            gameGet15 = new Game();
            // accept connections on a different thread   
            getPlayers = new Thread(new ThreadStart(SetUp));
            getPlayers.Start();
        }

        /// <summary>
        /// Close all threads associated with this application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            disconnected = true;
            System.Environment.Exit(System.Environment.ExitCode);
        }

        /// <summary>
        /// DisplayMessage sets TxtDisplay's Text property in a thread-safe manner
        /// </summary>
        /// <param name="message"></param>
        public void DisplayMessage(string message)
        {
            // if modifying displayTextBox is not thread safe
            if (!TxtDisplay.Dispatcher.CheckAccess())
            {
                TxtDisplay.Dispatcher.Invoke(new Action(() => TxtDisplay.Text += message));
            }
            else // safe to modify displayTextBox in current thread
                TxtDisplay.Text += message;
        }

        /// <summary>
        /// Accepts the connection of the two players
        /// </summary>
        public void SetUp()
        {
            DisplayMessage("Waiting for players...\r\n");

            // set up Socket                                           
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            listener.Start();

            // accept player 1 and start a player thread              
            players[0] = new Player(listener.AcceptSocket(), this, 0);
            playerThreads[0] = new Thread(new ThreadStart(players[0].RunPlayer));
            playerThreads[0].Start();

            // accept player 2 and start another player thread       
            players[1] = new Player(listener.AcceptSocket(), this, 1);
            playerThreads[1] = new Thread(new ThreadStart(players[1].RunPlayer));
            playerThreads[1].Start();

            // notify player 1 that player 2 has connected
            lock (players[0])
            {
                players[0].threadSuspended = false;
                Monitor.Pulse(players[0]);
            }
        }

        /// <summary>
        /// Validate input number by player
        /// </summary>
        /// <param name="number"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool InputValidation(byte number, int player)
        {
            // prevent another thread from making a move
            lock (this)
            {
                // while it is not the current player's turn, wait
                while (player != currentPlayer)
                {
                    Monitor.Wait(this);
                }

                // if the number is valid
                if (gameGet15.NumberStillNotPlayed(number))
                {
                    // remove from the available numbers
                    gameGet15.RemoveFromTheRemainingNumbers(number);

                    // add to the current player list
                    gameGet15.AddNumberPlayer(number, player);

                    // set the currentPlayer to be the other player
                    currentPlayer = (currentPlayer + 1) % 2;

                    // notify the other player of the move                
                    players[currentPlayer].OtherPlayerMoved(number);

                    return true;
                }
                else
                    return false;
            }
        }
    }
}
