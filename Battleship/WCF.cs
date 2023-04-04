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
        TileStatus SendShot(string playerName, string opponentName, Guess Guess);
        [OperationContract]
        Guess GetShot(Player player);
        [OperationContract]
        bool WaitForPlayers(int playerCount = 2);
        [OperationContract]
        bool NextTurn();
        [OperationContract]
        void StartGame();
        [OperationContract]
        bool IsTurn(Player player);
        [OperationContract]
        int PlayerCount();
        [OperationContract]
        int GetMaxPlayers();
        [OperationContract]
        void SetMaxPlayers(int maxPlayers);
        [OperationContract]
        bool HasWon(Player player);
        [OperationContract]
        bool IsValidGuess(char row, int column);
        [OperationContract]
        List<Player> GetOpponents(Player player);
        [OperationContract]
        List<Player> GetPlayers(Player player);
        [OperationContract]
        bool IsValidName(string name);
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
        public bool Lost { get; set; }
        [DataMember]
        public List<Ship> Ships { get; private set; }

        public Player(string name)
        {
            Name = name;
            IsTurn = false;
            Lost = false;
            Ships = new List<Ship>();
        }
        public void AddShip(Ship ship)
        {
            Ships.Add(ship);
        }
        public bool HasLost()
        {
            foreach(Ship ship in Ships)
            {
                if (!ship.Sunk)
                {
                    return false;
                }
            }
            return Lost = true;
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
}