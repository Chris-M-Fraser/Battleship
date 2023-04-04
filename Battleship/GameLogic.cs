using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Battleship
{
    public class Gameboard
    {
        public int Size { get; }
        public Tile[,] Tiles { get; private set; }
        public List<Ship> Ships { get; private set; }

        public Gameboard(int size = 10)
        {
            Size = size;
            Tiles = new Tile[size, size];
            Ships = new List<Ship>();
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    Tiles[row, col] = new Tile((char)('A' + row), col, TileStatus.Empty);
                }
            }
        }


        public void PlaceShip(Ship ship)
        {
            foreach (var shipCell in ship.OccupiedTiles)
            {
                Tiles[shipCell.Row - 'A', shipCell.Column - 1].SetStatus(TileStatus.Occupied);
            }
            Ships.Add(ship);
        }

        public Ship PlaceShipPrompt(int length)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(this.Display());

                AnsiConsole.Write(new Rule("[yellow]Place your ships[/]").RuleStyle("grey"));
                AnsiConsole.MarkupLine("[bold yellow]Ship Length:[/] " + length);

                var orientationPrompt = new SelectionPrompt<Orientation>()
                    .Title("[green]Select the ship's orientation:[/]")
                    .AddChoices(Orientation.Up, Orientation.Down, Orientation.Left, Orientation.Right)
                    .UseConverter(o => $"[blue]{o}[/]");

                var orientation = AnsiConsole.Prompt(orientationPrompt);
                AnsiConsole.MarkupLine("[bold green]Orientation:[/] [blue]" + orientation + "[/]");

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
                char row = Char.ToUpper(coordinates[0]);
                int column = int.Parse(coordinates.Substring(1));

                Ship ship = new Ship(length, orientation, row, column);

                if (CanPlaceShip(ship))
                {
                    PlaceShip(ship);
                    return ship;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Cannot place ship at the specified location. Press [bold]Enter[/] to try again.[/]");
                    Console.ReadLine();
                }
            }
        }

        public bool HasLost()
        {
            foreach (Ship ship in Ships)
            {
                if (!ship.Sunk)
                {
                    return false;
                }
            }
            return true;
        }
        public List<Tile> GetOccupiedTiles()
        {
            List<Tile> tiles = new List<Tile>();
            foreach(Ship ship in Ships)
            {
                tiles.AddRange(ship.OccupiedTiles);
            }
            return tiles;
        }

        private bool CanPlaceShip(Ship ship)
        {
            foreach (var shipCell in ship.OccupiedTiles)
            {
                if (shipCell.Row > 'J' || shipCell.Row < 'A' || shipCell.Column > 10 || shipCell.Column < 1)
                {
                    return false;
                }
                Tile tile = GetCell(shipCell.Row, shipCell.Column);
                if (tile.Status != TileStatus.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        public void ApplyTileStatus(char row, int column, TileStatus status)
        {
            if (status == TileStatus.Sunk)
            {
                Tiles[row - 'A', column - 1].SetStatus(TileStatus.Hit);
            }
            else
            {
                Tiles[row - 'A', column - 1].SetStatus(status);
            }
        }



        public TileStatus AddGuess(Guess guess)
        {
            TileStatus status = TileStatus.Miss;
            foreach(Ship ship in Ships)
            {
                Tile tile = ship.OccupiedTiles.FirstOrDefault(s => s.Row == guess.Row && s.Column == guess.Column);
                if(tile != null)
                {
                    ship.Hit(guess.Row, guess.Column);
                    if (ship.Sunk)
                    {
                        status = TileStatus.Sunk;
                    }
                    status = TileStatus.Hit;
                    break;
                }
                else
                {
                    status = TileStatus.Miss;
                }
            }
            ApplyTileStatus(guess.Row, guess.Column, status);
            return status;

        }

        public virtual Table Display()
        {
            var table = new Table().NoBorder();

            // Add column headers
            table.AddColumn(new TableColumn(" ").Centered().NoWrap());
            for (int col = 1; col <= Size; col++)
            {
                table.AddColumn(new TableColumn($"[lime]{ col }[/]").Centered().NoWrap());
            }
            table.AddEmptyRow();

            // Add rows with row headers and tile values
            for (int row = 0; row < Size; row++)
            {
                var rowHeader = $"[lime]{(char)('A' + row)}[/]";
                var rowData = new string[Size + 1];
                rowData[0] = rowHeader;
                for (int col = 1; col <= Size; col++)
                {
                    rowData[col] = Tiles[row, col - 1].Contents;
                }
                table.AddRow(rowData);

                // Add empty line between rows, except after the last row
                if (row < Size - 1)
                {
                    table.AddEmptyRow();
                }
            }

            // Set a fixed column width for square grid boxes
            table.Columns[0].Width(3);
            for (int col = 1; col <= Size; col++)
            {
                table.Columns[col].Width(3);
            }

            return table;

        }

        public Tile GetCell(char row, int col)
        {
            return Tiles[row - 'A', col - 1];
        }
    }    

}
