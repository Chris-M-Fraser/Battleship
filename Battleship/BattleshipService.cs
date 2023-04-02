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
        private readonly Dictionary<Player, Guess> _playerGuesses;
        private readonly Dictionary<Player, GuessResult> _GuessResults;

        public BattleshipService()
        {
            _players = new List<Player>();
            _playerGuesses = new Dictionary<Player, Guess>();
            _GuessResults = new Dictionary<Player, GuessResult>();
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

        public void SendPlayerGuess(Player player, Guess Guess)
        {
            _playerGuesses[player] = Guess;
            Console.WriteLine($"{player.Name} made a Guess at ({Guess.Row}{Guess.Column}).");
        }

        public GuessResult GetGuessResult(Player player)
        {
            if (_GuessResults.TryGetValue(player, out GuessResult result))
            {
                _GuessResults.Remove(player);
                return result;
            }

            return GuessResult.Miss;
        }

        public Guess GetOpponentGuess(Player player)
        {

            List<Player> opponents = GetOpponents(player);
            foreach (Player opponent in opponents)
            {
                if (_playerGuesses.TryGetValue(opponent, out Guess Guess))
                {
                    _playerGuesses.Remove(opponent);
                    return Guess;
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
