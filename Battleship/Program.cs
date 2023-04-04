using Spectre.Console;
using System;
using System.Linq;
using Battleship;
using System.ServiceModel;
using System.Collections.Generic;
using Spectre.Console.Rendering;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize WCF client for the multiplayer communication
            var channelFactory = new ChannelFactory<IBattleshipService>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/BattleshipService"));
            var serviceClient = channelFactory.CreateChannel();

            AnsiConsole.WriteLine();
            AnsiConsole.Write(
            new FigletText("Battleship")
                .Centered()
                .Color(Color.CornflowerBlue));
            AnsiConsole.WriteLine();

            AnsiConsole.Write(new Rule("Chris Fraser & Shea De Vries-Thomas"));
            AnsiConsole.WriteLine();
            string name;

            do
            {
                name = GetName();
                if (!serviceClient.IsValidName(name))
                {
                    AnsiConsole.MarkupLine("[red]Name already taken.\n[/]");
                }

                else break;

            } while(true);

            int numberOfPlayers = serviceClient.GetMaxPlayers();

            if (numberOfPlayers == 0)
            {
                numberOfPlayers = GetNumberOfPlayers();
                serviceClient.SetMaxPlayers(numberOfPlayers);
            }

            AnsiConsole.WriteLine($"Welcome, {name}! There will be {numberOfPlayers} players in the game.");
            var player = new Player(name);


            // Initialize game board and hidden game board
            List<Gameboard> gameboards = new List<Gameboard>();
            List<HiddenGameBoard> hiddenGameboards = new List<HiddenGameBoard>();
            gameboards.Add(new Gameboard());

            for (int i = 1; i < numberOfPlayers; i++)
            {
                gameboards.Add(new HiddenGameBoard());
            }

            // Set up ships on the game board
            SetupShips(gameboards[0], player);

            Console.WindowHeight = 60;
            Console.WindowWidth = Console.LargestWindowWidth - 10;

            DisplayGameboards(gameboards);

            serviceClient.RegisterPlayer(player);

            AnsiConsole.Status()
            .Start("\t[lime]Waiting for players...[/]", ctx =>
            {
                ctx.Spinner(Spinner.Known.Triangle);
                ctx.SpinnerStyle(Style.Parse("green"));
                while (serviceClient.WaitForPlayers(numberOfPlayers))
                {
                    System.Threading.Thread.Sleep(1500);
                }
            });



            bool gameInProgress = true;
            serviceClient.StartGame();

            string gameResult = "";
            List<Player> players = serviceClient.GetPlayers(player);

            // Game loop
            while (gameInProgress)
            {
                for (int i = 1; i < numberOfPlayers; i++)
                {
                    Guess opponentGuess = serviceClient.GetShot(player);
                    if (opponentGuess != null)
                    {
                        // Update the game board with the opponent's Guess
                        gameboards[0].AddGuess(opponentGuess);

                        if (gameboards[0].HasLost())
                        {
                            gameResult = "lose";
                            gameInProgress = false;
                            serviceClient.NextTurn();
                            break;
                        }
                    }
                }
                DisplayGameboards(gameboards, players);
                if (serviceClient.IsTurn(player))
                {
                    // update player states
                    players = serviceClient.GetPlayers(player);

                    DisplayGameboards(gameboards, players);
                    for (int i = 1; i < players.Count; i++)
                    {
                        if (!players[i].HasLost())
                        {
                            Guess opponentGuess = serviceClient.GetShot(players[i]);
                            if (opponentGuess != null)
                            {

                                // Update the game board with the opponent's Guess
                                gameboards[0].AddGuess(opponentGuess);

                                if (players[0].HasLost())
                                {
                                    gameResult = "lose";
                                    gameInProgress = false;
                                    serviceClient.NextTurn();
                                    break;
                                }
                            }
                        }
                    }
                    // Get player's Guess
                    for (int i = 1; i < players.Count; i++)
                    {
                        int column;
                        char row;
                        if (!players[i].HasLost())
                        {
                            do
                            {
                                AnsiConsole.MarkupLine("Attacking " + players[i].Name);

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
                            } while (!serviceClient.IsValidGuess(row, column));
                            Guess guess = new Guess(row, column);

                            TileStatus status = serviceClient.SendShot(player.Name, players[i].Name, guess);
                            gameboards[i].ApplyTileStatus(guess.Row, guess.Column, status);

                            AnsiConsole.Write(
                            new FigletText(status.ToString() + "!")
                            .Centered()
                            .Color(Color.Red));
                            System.Threading.Thread.Sleep(1000);
                            DisplayGameboards(gameboards, players);
                        }
                    }


                    serviceClient.NextTurn();
                }
                else
                {

                    AnsiConsole.Status()
                    .Start("\t[lime]Waiting for turn...[/]", ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Triangle);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        System.Threading.Thread.Sleep(2000);
                    });
                }

                if (serviceClient.HasWon(player))
                {
                    gameResult = "win";
                    gameInProgress = false;
                    break;
                }


            }

            DisplayGameboards(gameboards, players);
            AnsiConsole.Write(
        new FigletText($"You {gameResult}!")
            .Centered()
            .Color(Color.Red));
            AnsiConsole.MarkupLine("[red]Press [bold]Enter[/] to quit.[/]");
            Console.ReadLine();

            // Close the WCF client
            channelFactory.Close();
        }

        private static void SetupShips(Gameboard gameBoard, Player player)
        {
            // FOR LIVE VERSION

            /*int[] shipLengths = new int[] { 2, 3, 3, 4, 5 };
            foreach (int length in shipLengths)
            {
                Ship ship = gameBoard.PlaceShipPrompt(length);
                player.AddShip(ship);
            }*/

            // FOR TESTING
            gameBoard.PlaceShip(new Ship(2, Orientation.Right, 'A', 3));
            player.AddShip(new Ship(2, Orientation.Right, 'A', 3));

        }
        public static string GetName()
        {
            return AnsiConsole.Ask<string>("[bold yellow]What's your name?[/]");
        }

        public static int GetNumberOfPlayers()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("[bold yellow]How many players (including you) will be playing?[/]")
                    .PageSize(10)
                    .AddChoices(new[] { 2, 3, 4 }));
        }
        private static void DisplayGameboards(List<Gameboard> gameboards, List<Player> players)
        {
            Table table = new Table();

            // Add columns for each player and the legend
            foreach (Player player in players)
            {
                table.AddColumn(new TableColumn($"[bold underline]{player.Name}[/]").Centered());
            }
            table.AddColumn(new TableColumn("Legend").Centered());

            // Create an array of IRenderable for gameboards and legend
            List<IRenderable> renderables = new List<IRenderable>();
            foreach (Gameboard gameboard in gameboards)
            {
                renderables.Add(gameboard.Display());
            }
            renderables.Add(DisplayLegend());

            // Add a single row to the table with gameboards and legend
            table.AddRow(renderables);

            // Display the table
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
        }



        private static void DisplayGameboards(List<Gameboard> gameboards)
        {
            Table table = new Table();

            List<IRenderable> renderables = new List<IRenderable>();
            foreach (Gameboard gameboard in gameboards)
            {
                table.AddColumn(new TableColumn("").Centered());
                renderables.Add(gameboard.Display());
            }
            table.AddColumn(new TableColumn("Legend").Centered());
            renderables.Add(DisplayLegend());


            // Add a single row to the table with gameboards and legend
            table.AddRow(renderables);

            // Display the table
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
        }

        private static Table DisplayLegend()
        {
            return new Table()
                .Border(TableBorder.Rounded)
                .HideHeaders()
                .AddColumn(new TableColumn("").Centered())
                .AddColumn(new TableColumn("").LeftAligned())
                .AddRow("[teal]~[/]", "Empty Tile")
                .AddEmptyRow()
                .AddRow("[silver]S[/]", "Occupied Tile")
                .AddEmptyRow()
                .AddRow("[red]X[/]", "Hit")
                .AddEmptyRow()
                .AddRow("[white]O[/]", "Miss")
                .AddEmptyRow()
                .AddRow("[grey]?[/]", "Unknown");
        }
    }

}
