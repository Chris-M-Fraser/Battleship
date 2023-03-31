using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Battleship
{
    public enum CellState
    {
        Empty,
        Ship,
        Hit,
        Miss
    }

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
            if (!CanPlaceShip(ship))
            {
                throw new InvalidOperationException("Ship cannot be placed at the specified location.");
            }

            foreach (var shipCell in ship.OccupiedCells)
            {
                Cells[shipCell.Row - 'A', shipCell.Column - 1].SetContents(CellStatus.Occupied);
            }
            Ships.Add(ship);
        }

        private bool CanPlaceShip(Ship ship)
        {
            foreach (var shipCell in ship.OccupiedCells)
            {
                Cell cell = GetCell(shipCell.Row, shipCell.Column);
                if (cell.Status == CellStatus.Empty)
                {
                    return true;
                }
            }

            return false;
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

        public void ApplyGuessResult(int x, int y, GuessResult result)
        {
            // Implement Guess result application logic here
        }

        public void AddGuess(Guess guess)
        {
            Guesses.Add(guess);
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
