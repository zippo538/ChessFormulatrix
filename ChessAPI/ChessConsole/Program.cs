using ChessAPI.Models;
using ChessAPI.Services;
using ChessAPI.Helpers;
using ChessAPI.Models.Interfaces;
using ChessAPI.Models.Enums;
using Spectre.Console;

Board board = new();
GameService gameService = new GameService();
BoardService.InitializeBoard(board, true);
TimerService timerService = new TimerService(TimeSpan.FromMinutes(10));

// White Turn
PieceColor currentTurn = PieceColor.White;
bool isGameOver = false;
string? gameResult = null;


// start timer
timerService.Start();

timerService.TimeExpired += color =>
{
    isGameOver = true;
    gameResult = $"{color} time out. " +
                 $"{GameHelper.GetOpponent(color)} wins!";
};



AnsiConsole.MarkupLine("[bold yellow]Custom Chess Board[/]\n");
while (!isGameOver)
{
    GameService.RenderBoard(board, timerService,currentTurn);
    

    if (MovementService.IsKingInCheck(board, currentTurn))
    {
        Console.WriteLine("CHECK!");
    }

    Console.WriteLine("\nSelect piece to move (row col, e.g., '7 4'):");
    Console.Write("> ");

    var fromInput = Console.ReadLine() ;
    if (string.IsNullOrWhiteSpace(fromInput))
        continue;

    var fromParts = fromInput.Split(' ');
    if (fromParts.Length != 2 ||
        !int.TryParse(fromParts[0], out int fromRow) ||
        !int.TryParse(fromParts[1], out int fromCol))
    {
        Console.WriteLine("Invalid input. Press any key...");
        Console.ReadKey();
        continue;
    }
    var piece = BoardHelper.GetPiece(board, fromRow, fromCol);
    if (piece == null)
    {
        Console.WriteLine("No piece at that position. Press any key...");
        Console.ReadKey();
        continue;
    }

    if (piece.Color != currentTurn)
    {
        Console.WriteLine("That's not your piece. Press any key...");
        Console.ReadKey();
        continue;
    }

    var validMoves = piece.GetValidMoves(board);
    if (validMoves.Count == 0)
    {
        Console.WriteLine("No valid moves for this piece. Press any key...");
        Console.ReadKey();
        continue;
    }

    Console.Clear();
    gameService.GetDrawBoard(board, validMoves);
    Console.WriteLine($"\nSelected: {piece.Color} {piece.Symbol}");
    Console.WriteLine("Enter destination (row col, or 'x' to cancel):");
    Console.Write("> ");

    var toInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(toInput) || toInput.Trim().ToLower() == "x")
        continue;

    var toParts = toInput.Split(' ');
    if (toParts.Length != 2 ||
        !int.TryParse(toParts[0], out int toRow) ||
        !int.TryParse(toParts[1], out int toCol))
    {
        Console.WriteLine("Invalid input. Press any key...");
        Console.ReadKey();
        continue;
    }

    var fromTile = BoardHelper.GetTile(board, fromRow, fromCol);
    var toTile = BoardHelper.GetTile(board, toRow, toCol);

    if (fromTile == null || toTile == null)
    {
        Console.WriteLine("Invalid position. Press any key...");
        Console.ReadKey();
        continue;
    }

    if (!MovementService.MoveIsValid(board, fromTile, toTile))
    {
        Console.WriteLine("Invalid move. Press any key...");
        Console.ReadKey();
        continue;
    }

    MovementHelper.MovePiece(board, fromTile, toTile);

    timerService.SwitchTurn();
    currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

    if (GameService.IsCheckmate(board, currentTurn))
    {
        isGameOver = true;
        gameResult = $"Checkmate! {GameHelper.GetOpponent(currentTurn)} wins!";
    }
    else if (GameService.IsStalemate(board, currentTurn))
    {
        isGameOver = true;
        gameResult = "Stalemate! Draw.";
    }
}

Console.Clear();
gameService.GetDrawBoard(board);
Console.WriteLine($"\n{gameResult}");
timerService.Pause();
AnsiConsole.WriteLine(
    $"\nFinal Time - " +
    $"White: {timerService.WhiteTime:mm\\:ss} | " +
    $"Black: {timerService.BlackTime:mm\\:ss}"
);
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();




