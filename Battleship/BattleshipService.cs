using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Battleship;

namespace Battleship
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BattleshipService : IBattleshipService
    {
        private readonly List<Player> _players;
        private readonly Dictionary<string, Queue<Guess>> _shots;
        private int _maxPlayers;

        public BattleshipService()
        {
            _players = new List<Player>();
            _shots = new Dictionary<string, Queue<Guess>>();
            _maxPlayers = 0;
        }

        public void RegisterPlayer(Player player)
        {
            if (_players.Count < _maxPlayers)
            {
                _players.Add(player);
                Console.WriteLine($"{player.Name} has joined the game.");
            }
            else
            {
                throw new InvalidOperationException("The game is full.");
            }
        }

        public bool IsValidGuess(char row, int column)
        {
            if (row > 'J' || row < 'A' || column > 10 || column < 1)
            {
                return false;
            }

            return true;
        }

        public TileStatus SendShot(string playerName, string opponentName, Guess Guess)
        {
            if (!_shots.ContainsKey(opponentName))
            {
                _shots.Add(opponentName, new Queue<Guess>());
            }
            _shots[opponentName].Enqueue(Guess);
            TileStatus status = TileStatus.Miss;
            int index = _players.FindIndex(p => p.Name == opponentName);

            foreach (Ship ship in _players[index].Ships)
            {
                if (ship.OccupiedTiles.Any(c => c.Row == Guess.Row && c.Column == Guess.Column))
                {
                    ship.Hit(Guess.Row, Guess.Column);
                    if (ship.Sunk)
                    {
                        status = TileStatus.Sunk;
                    }
                    else
                    {
                        status = TileStatus.Hit;
                    }
                }
            }
            Console.WriteLine($"{playerName} took a shot at {opponentName} ({Guess.Row}{Guess.Column}).");
            Console.WriteLine($"Result was {status}!");

            return status;

        }

        public Guess GetShot(Player player)
        {
            if (_shots.ContainsKey(player.Name))
            {
                if(_shots[player.Name].Count > 0)
                    return _shots[player.Name].Dequeue();
            }
            return null;
        }

        // get the given player's opponents
        public List<Player> GetOpponents(Player player)
        {
            return _players.FindAll(p => !p.Name.Equals(player.Name) && !p.HasLost());
        }
        // check if the given player has won
        public bool HasWon(Player player)
        {
            return !_players.Any(p => !p.Name.Equals(player.Name) && !p.HasLost());
        }
        // returns list of players with given player at [0]
        public List<Player> GetPlayers(Player player)
        {
            List<Player> players = new List<Player> { player };
            players.AddRange(GetOpponents(player));
            return players;
        }
        public bool WaitForPlayers(int playerCount = 2)
        {
            return _players.Count < playerCount;
        }
        public int PlayerCount()
        {
            return _players.Count;
        }
        public void SetMaxPlayers(int maxPlayers)
        {
            _maxPlayers = maxPlayers;
        }
        public int GetMaxPlayers()
        {
            return _maxPlayers;
        }
        public bool IsTurn(Player player)
        {
            return _players.Find(p => p.Name == player.Name).IsTurn;
        }
        public void StartGame()
        {
            _players[0].IsTurn = true;
        }

        public bool IsValidName(string name)
        {
            return !_players.Any(p => p.Name == name);
        }

        public bool NextTurn()
        {
            int currentPlayerIndex = _players.FindIndex(p => p.IsTurn);
            if(currentPlayerIndex == -1)
            {
                return false;
            }
            _players[currentPlayerIndex].IsTurn = false;

            int nextPlayerIndex = (currentPlayerIndex + 1) % _players.Count;
            if (!_players[nextPlayerIndex].HasLost())
            {
                _players[nextPlayerIndex].IsTurn = true;
            }
            Console.WriteLine($"Player {nextPlayerIndex + 1}'s turn ");
            return true;
        }


    }
}
