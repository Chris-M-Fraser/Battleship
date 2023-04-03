using System;
using System.Runtime.Serialization;

[DataContract]
public class Cell
{
    [DataMember]
    public char Row { get; set; }

    [DataMember]
    public int Column { get; set; }

    [DataMember]
    public CellStatus Status { get; set; }

    [DataMember]
    public string Contents { get; set; }

    public Cell() { } // Add a parameterless constructor for serialization

    public Cell(char row, int column, CellStatus status)
    {
        Row = row;
        Column = column;
        SetStatus(status);
    }

    public void SetStatus(CellStatus status)
    {
        Status = status;
        switch (status)
        {
            case CellStatus.Empty:
                Contents = "[teal]~[/]";
                return;
            case CellStatus.Occupied:
                Contents = "[silver]S[/]";
                return;
            case CellStatus.Hit:
                Contents = "[red]X[/]";
                return;
            case CellStatus.Miss:
                Contents = "[white]O[/]";
                return;
            case CellStatus.Unknown:
                Contents = "[grey]?[/]";
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(Status), "Invalid cell status");
        }
    }
}
[DataContract]
public enum CellStatus
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
    Unknown
}
