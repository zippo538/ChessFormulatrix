using ChessAPI.Models;
using ChessAPI.Services;
using ChessAPI.Helpers;
using ChessAPI.Models.Interfaces;
using ChessAPI.Models.Enums;
using Spectre.Console;

Board board = new();
BoardService.InitializeBoard(board, true);

PieceColor currentTurn = PieceColor.White;
bool isGameOver = false;
string? gameResult = null;
AnsiConsole.MarkupLine("[bold yellow]Custom Chess Board[/]\n");

while (!isGameOver)
{
    Console.Clear();
    DrawBoard(board);
    Console.WriteLine($"\nTurn: {currentTurn}");

    if (MovementService.IsKingInCheck(board, currentTurn))
    {
        Console.WriteLine("CHECK!");
    }

    Console.WriteLine("\nSelect piece to move (row col, e.g., '7 4'):");
    Console.Write("> ");

    var fromInput = Console.ReadLine();
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
    DrawBoard(board, validMoves);
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

    currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

    if (IsCheckmate(board, currentTurn))
    {
        isGameOver = true;
        gameResult = $"Checkmate! {GetOpponent(currentTurn)} wins!";
    }
    else if (IsStalemate(board, currentTurn))
    {
        isGameOver = true;
        gameResult = "Stalemate! Draw.";
    }
}

Console.Clear();
DrawBoard(board);
Console.WriteLine($"\n{gameResult}");
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

static void DrawBoard(Board board, IList<Tile>? validMoves = null)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("   [bold]0  1  2  3  4  5  6  7[/]");

    for (int row = 0; row < board.Size; row++)
    {
        AnsiConsole.Markup($"[bold]{row}[/] ");
        for (int col = 0; col < board.Size; col++)
        {
            var tile = board.Tiles[row, col];
            bool isValidMove = validMoves != null && validMoves.Any(m => m.Row == row && m.Column == col);
            bool isLightSquare = (row + col) % 2 == 0;
            string bgColor = isLightSquare ? "silver" : "grey23";
            if (isValidMove)
            {
                bgColor = isLightSquare ? "lightgreen" : "green";
            }

            string cellContent = "   ";
            string fgColor = "black";
            if (tile.Piece != null)
            {
                cellContent= GetPieceSymbol(tile.Piece);
                fgColor = tile.Piece.Color == PieceColor.White ? "white" : "black";
                if (isValidMove) bgColor = "red";
            }
            else if (isValidMove)
            {
                cellContent = " • ";
                fgColor = "blue";
            }
            AnsiConsole.Markup($"[{fgColor} on {bgColor}]{cellContent}[/]");
            
        }
        AnsiConsole.MarkupLine($" [bold]{row}[/]");
    }

    AnsiConsole.MarkupLine("   [bold]0  1  2  3  4  5  6  7[/]\n");
}

static string GetPieceSymbol(IPiece piece)
{
    return piece.Symbol switch
    {
        PieceType.Pawn => piece.Color == PieceColor.White ? " ♙ " : " ♟ ",
        PieceType.Rook => piece.Color == PieceColor.White ? " ♖ " : " ♜ ",
        PieceType.Knight => piece.Color == PieceColor.White ? " ♘ " : " ♞ ",
        PieceType.Bishop => piece.Color == PieceColor.White ? " ♗ " : " ♝ ",
        PieceType.Queen => piece.Color == PieceColor.White ? " ♕ " : " ♛ ",
        PieceType.King => piece.Color == PieceColor.White ? " ♔ " : " ♚ ",
        _ => "?"
    };
}

static bool IsCheckmate(Board board, PieceColor color)
{
    if (!MovementService.IsKingInCheck(board, color))
        return false;

    return !HasAnyValidMove(board, color);
}

static bool IsStalemate(Board board, PieceColor color)
{
    if (MovementService.IsKingInCheck(board, color))
        return false;

    return !HasAnyValidMove(board, color);
}

static bool HasAnyValidMove(Board board, PieceColor color)
{
    for (int row = 0; row < board.Size; row++)
    {
        for (int col = 0; col < board.Size; col++)
        {
            var piece = BoardHelper.GetPiece(board, row, col);
            if (piece == null || piece.Color != color)
                continue;

            if (piece.GetValidMoves(board).Count > 0)
                return true;
        }
    }
    return false;
}

static PieceColor GetOpponent(PieceColor color)
{
    return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}
