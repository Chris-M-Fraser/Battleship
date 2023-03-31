using Spectre.Console;
public interface IGameboard
{
    int Size { get; }
    Cell[,] Cells { get; }

    Table Display();
}
