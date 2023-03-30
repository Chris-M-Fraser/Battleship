using Spectre.Console;
public class HiddenGameBoard : GameBoard, IGameboard
{
    public GameBoard gameboard;

    public HiddenGameBoard(GameBoard board)
    {
        gameboard = board;
        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cells[row, col] = new Cell((char)('A' + row), col, CellStatus.Unknown);
            }
        }
    }

    public override void Display()
    {
        var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.DarkCyan);

        // Add column headers
        table.AddColumn(new TableColumn(" ").Centered().NoWrap());
        for (int col = 1; col <= Size; col++)
        {
            table.AddColumn(new TableColumn(col.ToString()).Centered().NoWrap());
        }

        // Add rows with row headers and cell values
        for (int row = 0; row < Size; row++)
        {
            var rowHeader = ((char)('A' + row)).ToString();
            var rowData = new string[Size + 1];
            rowData[0] = rowHeader;
            for (int col = 1; col <= Size; col++)
            {
                rowData[col] = Cells[row, col - 1].Contents;
            }
            table.AddRow(rowData);

            // Add a rule (line) between rows, except after the last row
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

        AnsiConsole.Write(table);
    }

    public void RevealCell(char row, int col)
    {
        gameboard.GetCell(row, col);
    }

}
