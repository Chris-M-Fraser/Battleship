using Spectre.Console;
using Battleship;
public class HiddenGameBoard : Gameboard
{
    public Gameboard gameboard;

    public HiddenGameBoard(Gameboard board)
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

    public void ApplyGuess(Guess guess)
    {
        Guesses.Add(guess);
    }
    public bool HasLost()
    {
        return false;
    }

    public void RevealCell(char row, int col)
    {
        //Cell cell = gameboard.CheckCell(row, col);
        //Cells[row - 'A', col - 1].Status = cell.Status;
    }

}
