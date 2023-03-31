using System;

public class Cell
{
    public char Row { get; }
    public int Column { get; }
    public CellStatus Status { get; set; }
    public string Contents { get; set; }

    public Cell(char row, int column, CellStatus status)
    {
        Row = row;
        Column = column;
        Status = status;
        SetContents(status);
    }
    public void SetContents(CellStatus status)
    {
        switch (status)
        {
            case CellStatus.Empty:
                Contents = "[teal]~[/]";
                return;
            case CellStatus.Occupied:
                Contents = "[grey]S[/]";
                return;
            case CellStatus.Hit:
                Contents = "[red]X[/]";
                return;
            case CellStatus.Unknown:
                Contents = "[grey]?[/]";
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(Status), "Invalid cell status");
        }
    }
}
public enum CellStatus
{
    Empty,
    Occupied,
    Hit,
    Miss,
    Unknown
}
