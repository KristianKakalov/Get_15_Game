using System;
using System.Collections.Generic;

namespace Server.Classes
{
    class Game
    {
        #region Datamembers
        private int winnerIdNumber;                                                                 // Shows which player has won or -1 for draw
        private HashSet<byte>[] playersNumbers;                                                     // Set of players numbers
        private Tuple<byte, byte, byte> winingCombination;                                          // Tuple containing the wining combination of the player
        private HashSet<byte> remainingNumbers = new HashSet<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; // List containing the numbers, which still can be used
        //List of all winning combinations and the player should have one of them in order to win
        private List<Tuple<byte, byte, byte>> winningCombinations = new List<Tuple<byte, byte, byte>>()
        {
            new Tuple<byte, byte,byte>(2,6,7),  new Tuple<byte,byte,byte>(1,5,9),  new Tuple<byte,byte,byte>(3,4,8),
            new Tuple<byte, byte,byte>(2,4,9),  new Tuple<byte,byte,byte>(3,5,7),  new Tuple<byte,byte,byte>(1,6,8),
            new Tuple<byte, byte,byte>(4,5,6),  new Tuple<byte,byte,byte>(2,5,8)
        };
        #endregion

        #region Constructor
        public Game()
        {
            // Initialization of playerNumbers
            playersNumbers = new HashSet<byte>[2] { new HashSet<byte>(), new HashSet<byte>() };

        }
        #endregion

        #region Properties
        public int WinnerIdNumber
        {
            get
            {
                return winnerIdNumber;
            }
        }
        public Tuple<byte, byte, byte> WiningCombination
        {
            get
            {
                return winingCombination;
            }
            private set
            {
                winingCombination = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Function adding the number to the player's list   
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberOfPlayer"></param>
        public void AddNumberPlayer(byte number, int numberOfPlayer)
        {
            playersNumbers[numberOfPlayer].Add(number); //add the number to the player
            remainingNumbers.Remove(number);            //removes the number from the remaining playable numbers
        }

        /// <summary>
        /// Check if it's a draw, when all numbers have been used or any of the players has numbers, which sum is 15
        /// </summary>
        /// <returns></returns>
        public bool CheckState()
        {
            //when all numbers are used it's draw
            if (remainingNumbers.Count == 0)
            {
                winnerIdNumber = -1;
                return true;
            }
            //check if any of the winning combinations exist in player or oponent's list
            foreach (var triplet in winningCombinations)
            {
                if (playersNumbers[0].Contains(triplet.Item1) && playersNumbers[0].Contains(triplet.Item2) && playersNumbers[0].Contains(triplet.Item3))
                {
                    // set which player won and what was his wining combination
                    winnerIdNumber = 0;
                    WiningCombination = triplet;
                    return true;
                }
                else if (playersNumbers[1].Contains(triplet.Item1) && playersNumbers[1].Contains(triplet.Item2) && playersNumbers[1].Contains(triplet.Item3))
                {
                    // set which player won and what was his wining combination
                    winnerIdNumber = 1;
                    WiningCombination = triplet;
                    return true;
                }
            }
            //continue playing
            return false;
        }

        /// <summary>
        /// Check if the number is not used yet
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool NumberStillNotPlayed(byte number)
        {
            return remainingNumbers.Contains(number);
        }

        /// <summary>
        /// Remove the number since it is used
        /// </summary>
        /// <param name="number"></param>
        public void RemoveFromTheRemainingNumbers(byte number)
        {
            remainingNumbers.Remove(number);
        }

        /// <summary>
        /// Returns string of the wanted player numbers
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        public string GetPlayerNumbersString(int idNumber)
        => $"{string.Join("\t", playersNumbers[idNumber])}";

        /// <summary>
        /// Returns string of the remaining numbers
        /// </summary>
        /// <returns></returns>
        public string GetRemainingNumbersString()
            => $"{string.Join(", ", remainingNumbers)}";
        #endregion
    }
}
