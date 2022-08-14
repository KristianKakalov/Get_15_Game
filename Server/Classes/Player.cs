using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server;

namespace Server.Classes
{
    public class Player
    {
        internal Socket connection;             // Socket for accepting a connection    
        private NetworkStream socketStream;     // Network data stream          
        private MainWindow server;              // Reference to server          
        private BinaryWriter writer;            // Facilitates writing to the stream   
        private BinaryReader reader;            // Facilitates reading from the stream 
        private int idNumber;                   // Player IdNumber                                                  
        internal bool threadSuspended = true;   // If waiting for other player

        /// <summary>
        /// Constructor requiring Socket, TicTacToeServerForm and int objects as arguments
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="serverValue"></param>
        /// <param name="newNumber"></param>
        public Player(Socket socket, MainWindow serverValue, int newNumber)
        {
            connection = socket;
            server = serverValue;
            idNumber = newNumber;

            // create NetworkStream object for Socket      
            socketStream = new NetworkStream(connection);

            // create Streams for reading/writing bytes
            writer = new BinaryWriter(socketStream);
            reader = new BinaryReader(socketStream);
        }

        /// <summary>
        /// Signal other player of move
        /// </summary>
        /// <param name="number"></param>
        public void OtherPlayerMoved(byte number)
        {
            // signal that opponent moved                     
            writer.Write("Opponent moved");
            writer.Write(number); // send the number
        }

        /// <summary>
        /// Allows the players to make moves and receive moves from the other player
        /// </summary>
        public void RunPlayer()
        {
            bool done = false;

            // display on the server that a connection was made            
            server.DisplayMessage($"Player {idNumber + 1} connected\r\n");

            // player 1 must wait for another player to arrive
            if (idNumber == 0)
            {

                // wait for notification from server that another player has connected
                lock (this)
                {
                    while (threadSuspended == true)
                        Monitor.Wait(this);
                }
            }

            // send the current player's number to the client
            writer.Write(idNumber);


            // start playing the game
            while (!done)
            {
                // wait for data to become available
                while (connection.Available == 0)
                {
                    Thread.Sleep(1000);
                    if (server.disconnected)
                        return;
                }

                // receive number                   
                byte number = reader.ReadByte();

                // if the move is valid, display the number on the server and signal that it is valid
                if (server.InputValidation(number, idNumber))
                {
                    server.DisplayMessage($"Player {idNumber + 1} input: " + number + "\r\n");
                    writer.Write("Valid");
                }
                else
                {
                    // signal that the number was invalid
                    writer.Write("Invalid");
                }

                // if game is over, set done to true to exit while loop
                if (server.gameGet15.CheckState())
                {
                    // send message what was the output of the game
                    string message = server.gameGet15.WinnerIdNumber == -1 ?
                    "It's a draw!" : $"Player {server.gameGet15.WinnerIdNumber + 1} won!";

                    server.DisplayMessage(message);
                    done = true;
                }
            }
            // close the socket connection
            writer?.Close();
            reader?.Close();
            socketStream?.Close();
            connection?.Close();
        }
    }
}
