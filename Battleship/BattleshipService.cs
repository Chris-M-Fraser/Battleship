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
        private readonly Dictionary<string, Guess> _playerGuesses;

        public BattleshipService()
        {
            _players = new List<Player>();
            _playerGuesses = new Dictionary<string, Guess>();
        }

        public void RegisterPlayer(Player player)
        {
            if (_players.Count < 2)
            {
                _players.Add(player);
                Console.WriteLine($"{player.Name} has joined the game.");
            }
            else
            {
                throw new InvalidOperationException("The game is full.");
            }
        }

        public CellStatus SendPlayerGuess(Player player, Guess Guess)
        {
            _playerGuesses[player.Name] = Guess;
            List<Player> opponents = GetOpponents(player);
            CellStatus status = CellStatus.Miss;
            foreach (Player opponent in opponents)
            {
                if (opponent.OccupiedCells.Any(c => c.Row == Guess.Row && c.Column == Guess.Column))
                    foreach (Cell cell in opponent.OccupiedCells)
                    {
                        if (cell.Row == Guess.Row && cell.Column == Guess.Column)
                        {
                            status = CellStatus.Hit;
                        }
                    }
            }
            Console.WriteLine($"{player.Name} made a Guess at ({Guess.Row}{Guess.Column}).");
            Console.WriteLine($"Result was {status}!");

            return status;

        }

        public Guess GetOpponentGuess(Player player)
        {
            List<Player> opponents = GetOpponents(player);
            foreach (Player opponent in opponents)
            {
                if (_playerGuesses.ContainsKey(opponent.Name))
                {
                    Guess guess = _playerGuesses[opponent.Name];
                    _playerGuesses.Remove(opponent.Name);
                    return guess;
                }
            }

            return null;
        }


        private List<Player> GetOpponents(Player player)
        {
            return _players.FindAll(p => !p.Name.Equals(player.Name));
        }
        public bool WaitForPlayers(int playerCount = 2)
        {
            return _players.Count < playerCount;
        }
        public bool IsTurn(Player player)
        {
            return _players.Find(p => p.Name == player.Name).IsTurn;
        }
        public Player GetPlayer(string name)
        {
            return _players.Find(p => p.Name == name);
        }
        public void StartGame()
        {
            _players[0].IsTurn = true;
        }

        public void NextTurn()
        {
            int currentPlayerIndex = _players.FindIndex(p => p.IsTurn);
            _players[currentPlayerIndex].IsTurn = false;

            int nextPlayerIndex = (currentPlayerIndex + 1) % _players.Count;
            _players[nextPlayerIndex].IsTurn = true;
            Console.WriteLine("Player " + nextPlayerIndex + 1 + "'s turn ");
        }


    }
}
