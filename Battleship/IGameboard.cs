using Spectre.Console;
public interface IGameboard
{
    int Size { get; }
    Tile[,] Tiles { get; }

    Table Display();
}
