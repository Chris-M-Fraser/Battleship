using System;
using System.Runtime.Serialization;
using Spectre.Console;

[DataContract]
public class Ship
{
    [DataMember]
    public int Length { get; private set; }

    [DataMember]
    public Orientation ShipOrientation { get; private set; }

    [DataMember]
    public int Hits { get; private set; }

    [DataMember]
    public bool Sunk { get; private set; }

    [DataMember]
    public Tile[] OccupiedTiles { get; private set; }

    public Ship(int length, Orientation orientation, char row, int column)
    {
        Length = length;
        ShipOrientation = orientation;
        Hits = 0;

        OccupiedTiles = new Tile[length];
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

            OccupiedTiles[i] = new Tile((char)(currentRow + 'A'), currentColumn, TileStatus.Occupied);
        }
    }

    public void Hit(char row, int column)
    {
        foreach (Tile tile in OccupiedTiles)
        {
            if (tile.Row == row && tile.Column == column && tile.Status == TileStatus.Occupied)
            {
                Hits++;
                Sunk = Hits >= Length;
                tile.SetStatus(TileStatus.Hit);
            }
        }
    }

}

[DataContract]
public enum Orientation
{
    [EnumMember]
    Up,
    [EnumMember]
    Down,
    [EnumMember]
    Left,
    [EnumMember]
    Right
}
