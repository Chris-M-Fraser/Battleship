using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Battleship
{
    // Define the service contract for the Battleship game
    [ServiceContract]
    public interface IBattleshipService
    {
        [OperationContract]
        void RegisterPlayer(Player player);

        [OperationContract]
        CellStatus SendPlayerGuess(Player player, Guess Guess);
        [OperationContract]
        Guess GetOpponentGuess(Player player);
        [OperationContract]
        bool WaitForPlayers(int playerCount = 2);
        [OperationContract]
        void NextTurn();
        [OperationContract]
        void StartGame();
        [OperationContract]
        bool IsTurn(Player player);
    }

    // Define the data contract for the Player class
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsTurn { get; set; }
        [DataMember]
        public List<Cell> OccupiedCells { get; set; }

        public Player(string name)
        {
            Name = name;
            IsTurn = false;
            OccupiedCells = new List<Cell>();
        }
        public void AddCells(List<Cell> cells)
        {
            OccupiedCells.AddRange(cells);
        }
    }

    // Define the data contract for the Guess class
    [DataContract]
    public class Guess
    {
        [DataMember]
        public int Column { get; set; }

        [DataMember]
        public char Row { get; set; }

    public Guess(char row, int column)
    {
        Row = row;
        Column = column;
    }
}

    // Define the data contract for the CellStatus enum
    //[DataContract]
    //public enum CellStatus
    //{
    //    [EnumMember]
    //    Miss,

    //    [EnumMember]
    //    Hit,

    //    [EnumMember]
    //    Sunk,

    //    [EnumMember]
    //    Win
    //}
}