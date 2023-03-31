using Spectre.Console;

//Console.WindowHeight = Console.LargestWindowHeight;
//Console.WindowWidth = Console.LargestWindowWidth;


//var gameBoard = new GameBoard();
//var hiddenGameBoard = new HiddenGameBoard(gameBoard);

//AnsiConsole.Write(new Columns(new Table[] { gameBoard.Display(), hiddenGameBoard.Display() }));

//AnsiConsole.WriteLine();
//Ship ship = Ship.CreateShipPrompt();

//gameBoard.PlaceShip(ship);


//AnsiConsole.Clear();
//AnsiConsole.Write(new Columns(new Table[] { gameBoard.Display(), hiddenGameBoard.Display() }));

//hiddenGameBoard.RevealCell('A', 1);

//AnsiConsole.Clear();
//AnsiConsole.Write(new Columns(new Table[] { gameBoard.Display(), hiddenGameBoard.Display() }));

using System;
using Battleship;
using System.ServiceModel;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize WCF client for the multiplayer communication
            var channelFactory = new ChannelFactory<IBattleshipService>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/BattleshipService"));
            var serviceClient = channelFactory.CreateChannel();

            // Get player info
            AnsiConsole.WriteLine("Enter your name:");
            string playerName = AnsiConsole.Ask<string>("Name: ");
            var player = new Player(playerName);
            var opponent = new Player("opponent");

            // Register player with the WCF service
            serviceClient.RegisterPlayer(player);
            //serviceClient.RegisterPlayer(opponent);

            // Initialize game board and hidden game board
            var gameBoard = new Gameboard();
            var hiddenGameBoard = new HiddenGameBoard(gameBoard);

            // Set up ships on the game board
            SetupShips(gameBoard);

            // Game loop
            Console.WriteLine("Waiting for players...");
            while (serviceClient.WaitForPlayers())
            {
                System.Threading.Thread.Sleep(1000);
            }
            bool gameInProgress = true;
            while (gameInProgress)
            {
                // Display the game board
                AnsiConsole.Clear();
                AnsiConsole.Write(new Columns(gameBoard.Display(), hiddenGameBoard.Display()));

                // Get the opponent's Guess
                Guess opponentGuess = serviceClient.GetOpponentGuess(player);
                if (opponentGuess != null)
                {
                    // Update the hidden game board with the opponent's Guess
                    hiddenGameBoard.ApplyGuess(opponentGuess);

                    // Check if the player lost
                    if (hiddenGameBoard.HasLost())
                    {
                        gameInProgress = false;
                        AnsiConsole.WriteLine("You lost!");
                        continue;
                    }
                }

                // Get player's Guess
                int column;
                char row;
                do
                {
                    column = AnsiConsole.Ask<int>("Enter column (1-10): ");
                    row = AnsiConsole.Ask<char>("Enter row (A-J): ");
                } while (!gameBoard.IsValidGuess(row, column));

                // Send player's Guess to the WCF service
                serviceClient.SendPlayerGuess(player, new Guess(row, column));

                // Get the result of player's Guess from the WCF service
                GuessResult result = serviceClient.GetGuessResult(player);

                // Update the game board with the result
                gameBoard.ApplyGuessResult(column, row, result);

                // Check if the player won
                if (result == GuessResult.Win)
                {
                    gameInProgress = false;
                    AnsiConsole.WriteLine("You won!");
                }
            }

            // Close the WCF client
            channelFactory.Close();
        }

        private static void SetupShips(Gameboard gameBoard)
        {
            gameBoard.PlaceShip(new Ship(3, Orientation.Right, 'A', 3));
        }
    }

}
