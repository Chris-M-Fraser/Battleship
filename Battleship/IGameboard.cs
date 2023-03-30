public interface IGameboard
{
    int Size { get; }
    Cell[,] Cells { get; }

    void Display();
}
