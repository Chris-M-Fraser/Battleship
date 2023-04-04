using Spectre.Console;
using Battleship;

// proxy gameboard
public class HiddenGameBoard : Gameboard
{
    public Gameboard gameboard;

    public HiddenGameBoard()
    {
        gameboard = new Gameboard();
        for (int row = 0; row < gameboard.Size; row++)
        {
            for (int col = 0; col < gameboard.Size; col++)
            {
                Tiles[row, col] = new Tile((char)('A' + row), col, TileStatus.Unknown);
            }
        }
    }
}
