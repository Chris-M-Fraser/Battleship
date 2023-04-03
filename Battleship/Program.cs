using Spectre.Console;
using System;
using Battleship;
using System.ServiceModel;
using System.Collections.Generic;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 60;
            Console.WindowWidth = Console.LargestWindowWidth - 10;
            // Initialize WCF client for the multiplayer communication
            var channelFactory = new ChannelFactory<IBattleshipService>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/BattleshipService"));
            var serviceClient = channelFactory.CreateChannel();

            AnsiConsole.WriteLine();
            AnsiConsole.Write(
                new FigletText("Battleship")
                .Centered()
                .Color(Color.Red));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("Chris Fraser & Shea De Vries-Thomas"));



            // Get player info
            AnsiConsole.WriteLine("\tEnter your name:");
            string playerName = AnsiConsole.Ask<string>("Name: ");
            var player = new Player(playerName);


            // Initialize game board and hidden game board
            var gameBoard = new Gameboard();
            var hiddenGameBoard = new HiddenGameBoard(gameBoard);

            // Set up ships on the game board
            SetupShips(gameBoard);

            DisplayGameboards(gameBoard, hiddenGameBoard);

            player.AddCells(gameBoard.GetOccupiedCells());

            serviceClient.RegisterPlayer(player);


            AnsiConsole.Status()
            .Start("\t[lime]Waiting for players...[/]", ctx =>
            {
                ctx.Spinner(Spinner.Known.Triangle);
                ctx.SpinnerStyle(Style.Parse("green"));
                while (serviceClient.WaitForPlayers())
                {
                    System.Threading.Thread.Sleep(1000);
                }
            });



            bool gameInProgress = true;
            serviceClient.StartGame();

            // Game loop
            while (gameInProgress)
            {
                if (serviceClient.IsTurn(player))
                {
                    Guess opponentGuess = serviceClient.GetOpponentGuess(player);
                    if (opponentGuess != null)
                    {
                        // Update the game board with the opponent's Guess
                        gameBoard.AddGuess(opponentGuess);

                        if (hiddenGameBoard.HasLost())
                        {
                            AnsiConsole.WriteLine("You lost!");
                            break;
                        }
                    }
                    DisplayGameboards(gameBoard, hiddenGameBoard);

                    // Get player's Guess
                    int column;
                    char row;
                    var coordinatesPrompt = new TextPrompt<string>("[green]Enter the coordinates (A1-J10):[/]")
                .Validate(coord =>
                {
                    if (coord.Length < 2 || coord.Length > 3) return ValidationResult.Error("[red]Invalid coordinates format.[/]");
                    char rowChar = Char.ToUpper(coord[0]);
                    if (rowChar < 'A' || rowChar > 'J') return ValidationResult.Error("[red]Row must be between A and J.[/]");

                    if (!int.TryParse(coord.Substring(1), out int columnInt)) return ValidationResult.Error("[red]Invalid column value.[/]");
                    if (columnInt < 1 || columnInt > 10) return ValidationResult.Error("[red]Column must be between 1 and 10.[/]");

                    return ValidationResult.Success();
                });

                    string coordinates = AnsiConsole.Prompt(coordinatesPrompt);
                    row = Char.ToUpper(coordinates[0]);
                    column = int.Parse(coordinates.Substring(1));

                    // Send player's Guess to the WCF service
                    CellStatus result = serviceClient.SendPlayerGuess(player, new Guess(row, column));
                    AnsiConsole.Write(
                        new FigletText(result.ToString() + "!")
                        .Centered()
                        .Color(Color.Red));
                    System.Threading.Thread.Sleep(1000);

                    // Update the game board with the result
                    hiddenGameBoard.ApplyCellStatus(row, column, result);
                    DisplayGameboards(gameBoard, hiddenGameBoard);
                    serviceClient.NextTurn();
                }
                else
                {
                    AnsiConsole.Status()
                    .Start("\t[lime]Waiting for turn...[/]", ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Triangle);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        while (!serviceClient.IsTurn(player))
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    });
                }

                continue;

                // Check if the player won
                //if (result == CellStatus.Win)
                //{
                //    gameInProgress = false;
                //    AnsiConsole.WriteLine("You won!");
                //}
            }

            // Close the WCF client
            channelFactory.Close();
        }

        private static void SetupShips(Gameboard gameBoard)
        {
            int[] shipLengths = new int[] { 2, 3, 3, 4, 5 };
            foreach (int length in shipLengths)
            {
                gameBoard.PlaceShipPrompt(length);
            }

            //gameBoard.PlaceShip(new Ship(3, Orientation.Right, 'A', 3));


        }
        private static void DisplayGameboards(Gameboard gameBoard, HiddenGameBoard hiddenGameBoard)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Columns(new Text("Your board"), new Text("Opponent board")));
            AnsiConsole.Write(new Columns(gameBoard.Display(), hiddenGameBoard.Display()));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule());
        }
    }

}
