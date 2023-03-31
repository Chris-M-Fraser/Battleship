using System;
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
        void SendPlayerGuess(Player player, Guess guess);

        [OperationContract]
        GuessResult GetGuessResult(Player player);

        [OperationContract]
        Guess GetOpponentGuess(Player player);
        [OperationContract]
        bool WaitForPlayers(int playerCount = 2);
    }

    // Define the data contract for the Player class
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
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

    // Define the data contract for the GuessResult enum
    [DataContract]
    public enum GuessResult
    {
        [EnumMember]
        Miss,

        [EnumMember]
        Hit,

        [EnumMember]
        Sunk,

        [EnumMember]
        Win
    }
}