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
        CellStatus status = gameboard.AddGuess(guess);
        Cells[guess.Row - 'A', guess.Column - 1].SetStatus(status);

    }
    public bool HasLost()
    {
        return false;
    }

}
