using Spectre.Console;

Console.WindowHeight = Console.LargestWindowHeight;
Console.WindowWidth = Console.LargestWindowWidth;

var gameBoard = new GameBoard();
var hiddenGameBoard = new HiddenGameBoard(gameBoard);

gameBoard.Display();

AnsiConsole.WriteLine();
Ship ship = Ship.CreateShipPrompt();

gameBoard.PlaceShip(ship);

AnsiConsole.Clear();
gameBoard.Display();
