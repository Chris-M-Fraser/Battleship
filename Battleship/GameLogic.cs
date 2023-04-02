using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Battleship
{
    public class Gameboard
    {
        public int Size { get; }
        public Cell[,] Cells { get; private set; }
        public List<Ship> Ships { get; private set; }
        public List<Guess> Guesses { get; set; }

        public Gameboard(int size = 10)
        {
            Size = size;
            Cells = new Cell[size, size];
            Ships = new List<Ship>();
            Guesses = new List<Guess>();
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    Cells[row, col] = new Cell((char)('A' + row), col, CellStatus.Empty);
                }
            }
        }


        public void PlaceShip(Ship ship)
        {
            foreach (var shipCell in ship.OccupiedCells)
            {
                Cells[shipCell.Row - 'A', shipCell.Column - 1].SetStatus(CellStatus.Occupied);
            }
            Ships.Add(ship);
        }

        public void PlaceShipPrompt(int length)
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
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Cannot place ship at the specified location. Press [bold]Enter[/] to try again.[/]");
                    Console.ReadLine();
                }
            }
        }


        private bool CanPlaceShip(Ship ship)
        {
            foreach (var shipCell in ship.OccupiedCells)
            {
                if (shipCell.Row > 'J' || shipCell.Row < 'A' || shipCell.Column > 10 || shipCell.Column < 1)
                {
                    return false;
                }
                Cell cell = GetCell(shipCell.Row, shipCell.Column);
                if (cell.Status != CellStatus.Empty)
                {
                    return false;
                }
            }

            return true;
        }


        public bool IsValidGuess(char row, int column)
        {
            if(row > 'J' || row < 'A' || column > 10 || column < 1)
            {
                return false;
            }
            else if(Guesses.Contains(new Guess(row, column)))
            {
                return false;
            }
            return true;
        }

        public void ApplyGuessResult(char row, int column, GuessResult result)
        {
            if (result == GuessResult.Hit)
            {
                Cells[row - 'A', column].SetStatus(CellStatus.Hit);
                AnsiConsole.Write("Hit!");
            }
            else if (result == GuessResult.Miss)
            {
                Cells[row - 'A', column].SetStatus(CellStatus.Miss);
                AnsiConsole.Write("Miss!");

            }
        }


        public GuessResult AddGuess(Guess guess)
        {
            foreach(Ship ship in Ships)
            {
                Cell cell = ship.OccupiedCells.First(s => s.Row == guess.Row && s.Column == guess.Column);
                if(cell != null)
                {
                    ship.Hit();
                    return GuessResult.Hit;
                }
            }
            return GuessResult.Miss;

        }

        public virtual Table Display()
        {
            var table = new Table().NoBorder();

            // Add column headers
            table.AddColumn(new TableColumn(" ").Centered().NoWrap());
            for (int col = 1; col <= Size; col++)
            {
                table.AddColumn(new TableColumn($"[green]{ col }[/]").Centered().NoWrap());
            }
            table.AddEmptyRow();

            // Add rows with row headers and cell values
            for (int row = 0; row < Size; row++)
            {
                var rowHeader = $"[green]{(char)('A' + row)}[/]";
                var rowData = new string[Size + 1];
                rowData[0] = rowHeader;
                for (int col = 1; col <= Size; col++)
                {
                    rowData[col] = Cells[row, col - 1].Contents;
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

        public Cell GetCell(char row, int col)
        {
            return Cells[row - 'A', col - 1];
        }
    }    

}
