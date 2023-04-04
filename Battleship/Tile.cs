using System;
using System.Runtime.Serialization;

[DataContract]
public class Tile
{
    [DataMember]
    public char Row { get; set; }

    [DataMember]
    public int Column { get; set; }

    [DataMember]
    public TileStatus Status { get; set; }

    [DataMember]
    public string Contents { get; set; }

    public Tile() { } // Add a parameterless constructor for serialization

    public Tile(char row, int column, TileStatus status)
    {
        Row = row;
        Column = column;
        SetStatus(status);
    }

    public void SetStatus(TileStatus status)
    {
        Status = status;
        switch (status)
        {
            case TileStatus.Empty:
                Contents = "[teal]~[/]";
                return;
            case TileStatus.Occupied:
                Contents = "[silver]S[/]";
                return;
            case TileStatus.Sunk:
            case TileStatus.Hit:
                Contents = "[red]X[/]";
                return;
            case TileStatus.Miss:
                Contents = "[white]O[/]";
                return;
            case TileStatus.Unknown:
                Contents = "[grey]?[/]";
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(Status), "Invalid tile status");
        }
    }
}
[DataContract]
public enum TileStatus
{
    [EnumMember]
    Empty,
    [EnumMember]
    Occupied,
    [EnumMember]
    Hit,
    [EnumMember]
    Miss,
    [EnumMember]
    Sunk,
    [EnumMember]
    Unknown
}
