using Spectre.Console;
using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Helpers;

namespace ChessAPI.Services;

public class GameService
{
    private static void DrawBoard(Board board, IList<Tile>? validMoves = null)
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
                    cellContent= GameHelper.GetPieceSymbol(tile.Piece);
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
    public static bool IsCheckmate(Board board, PieceColor color)
    {
        if (!MovementService.IsKingInCheck(board, color))
            return false;
    
        return !HasAnyValidMove(board, color);
    }
    
    public static bool IsStalemate(Board board, PieceColor color)
    {
        if (MovementService.IsKingInCheck(board, color))
            return false;
    
        return !HasAnyValidMove(board, color);
    }
    
    private static bool HasAnyValidMove(Board board, PieceColor color)
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


    public void GetDrawBoard(Board board, IList<Tile>? validMoves = null)
    {
        DrawBoard(board, validMoves);
    }
    
    public static void RenderBoard(Board board, TimerService timer, PieceColor turn)
    {
        Console.Clear();
        GameHelper.DrawTimerTable(timer,turn);
        DrawBoard(board);
    
        AnsiConsole.MarkupLine(
            $"\nTurn: [bold]{turn}[/]"
        );
        
    }
}