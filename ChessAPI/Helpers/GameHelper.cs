using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Services;
using Spectre.Console;

namespace ChessAPI.Helpers;

public class GameHelper
{
    public static string GetPieceSymbol(IPiece piece)
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
    public static PieceColor GetOpponent(PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public static void DrawTimerTable(TimerService timer, PieceColor currentTurn)
    {
        var table = new Table();
        table.Border =  TableBorder.Rounded;
        
        table.AddColumn("[bold yellow]Player[/]");
        table.AddColumn("[bold yellow]Time[/]");
        table.AddColumn("[bold yellow]Status[/]");

        table.AddRow(
            "[white]♔ White[/]",
            $"[green]{timer.WhiteTime:mm\\:ss}[/]",
            currentTurn == PieceColor.White
                ? "[bold green]Thinking[/]"
                : "[grey]Waiting[/]"
            );
        table.AddRow(
            "[white]♚ Black[/]",
            $"[red]{timer.BlackTime:mm\\:ss}[/]",
            currentTurn == PieceColor.Black
                ? "[bold green]Thinking[/]"
                : "[grey]Waiting[/]"
            );
        AnsiConsole.Write(table);
    }
    
}