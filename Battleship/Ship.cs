using Spectre.Console;
using System;

public class Ship
{
    public int Length { get; }
    public Orientation ShipOrientation { get; }
    public int Hits { get; private set; }
    public Cell[] OccupiedCells { get; }

    public Ship(int length, Orientation orientation, char row, int column)
    {
        Length = length;
        ShipOrientation = orientation;
        Hits = 0;

        OccupiedCells = new Cell[length];
        for (int i = 0; i < length; i++)
        {
            int currentRow = row - 'A';
            int currentColumn = column;

            switch (ShipOrientation)
            {
                case Orientation.Up:
                    currentRow -= i;
                    break;
                case Orientation.Down:
                    currentRow += i;
                    break;
                case Orientation.Left:
                    currentColumn -= i;
                    break;
                case Orientation.Right:
                    currentColumn += i;
                    break;
            }

            OccupiedCells[i] = new Cell((char)(currentRow + 'A'), currentColumn, CellStatus.Occupied);
        }
    }

    public bool IsSunk()
    {
        return Hits >= Length;
    }

    public void Hit()
    {
        Hits++;
    }

}

public enum Orientation
{
    Up,
    Down,
    Left,
    Right
}