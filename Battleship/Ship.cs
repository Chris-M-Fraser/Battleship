using Spectre.Console;
public class Ship
{
    public int Length { get; }
    public Orientation ShipOrientation { get; }
    public int Hits { get; private set; }
    public Cell[] OccupiedCells { get; }

    public Ship(int length, Orientation orientation, char row, int column, GameBoard board)
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

    public static Ship CreateShipPrompt()
    {
        var lengthPrompt = new TextPrompt<int>("Enter the length of the ship:")
            .Validate(n => n > 0)
            .DefaultValue(1);
        var length = AnsiConsole.Prompt(lengthPrompt);

        var orientationPrompt = new SelectionPrompt<Orientation>()
            .Title("Select the ship's orientation:")
            .AddChoices(Orientation.Up, Orientation.Down, Orientation.Left, Orientation.Right);
        var orientation = AnsiConsole.Prompt(orientationPrompt);

        var rowPrompt = new TextPrompt<char>("Enter the row coordinate:")
            .Validate(c => char.IsLetter(c));
        var row = AnsiConsole.Prompt(rowPrompt);
        

        var columnPrompt = new TextPrompt<int>("Enter the column coordinate:")
            .Validate(n => n >= 1 && n <= 10);
        var column = AnsiConsole.Prompt(columnPrompt);

        return new Ship(length, orientation, row, column, null);
    }

}




public enum Orientation
{
    Up,
    Down,
    Left,
    Right
}