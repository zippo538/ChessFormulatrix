using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Services;
using Spectre.Console;

namespace ChessAPI.Helpers;

public class GameHelper
{
    public static string GetPieceSymbol(PieceType type, PieceColor color)
    {
        return type switch
        {
            PieceType.Pawn => color == PieceColor.White ? " ♙ " : " ♟ ",
            PieceType.Rook => color == PieceColor.White ? " ♖ " : " ♜ ",
            PieceType.Knight => color == PieceColor.White ? " ♘ " : " ♞ ",
            PieceType.Bishop => color == PieceColor.White ? " ♗ " : " ♝ ",
            PieceType.Queen => color == PieceColor.White ? " ♕ " : " ♛ ",
            PieceType.King => color == PieceColor.White ? " ♔ " : " ♚ ",
            _ => "?"
        };
    }

    public static string GetPieceSymbol(IPiece piece)
    {
        if (piece == null) return "?";
        return GetPieceSymbol(piece.Symbol, piece.Color);
    }

    public static PieceColor GetOpponent(PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public static void DrawTimerTable(TimerService timer, PieceColor currentTurn)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        
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

    public static void StartLiveTitleTimer(TimerService timer)
    {
        // Menjalankan proses di background thread agar tidak memblokir input terminal
        Task.Run(async () =>
        {
            // Background thread ini akan otomatis mati ketika aplikasi console ditutup
            while (true) 
            {
                Console.Title = $"♔ White: {timer.WhiteTime:mm\\:ss}  |  ♚ Black: {timer.BlackTime:mm\\:ss}  -  Chess Console";
                await Task.Delay(1000); // Update setiap 1 detik
            }
        });
    }

    public static string FormatMove(MoveHistory move)
    {
        if (move == null) return "-";

        char fromCol = (char)('A' + move.From.Column);
        int fromRow = 8 - move.From.Row;
        string fromSquare = $"{fromCol}{fromRow}";

        char toCol = (char)('A' + move.To.Column);
        int toRow = 8 - move.To.Row;
        string toSquare = $"{toCol}{toRow}";

        string pieceSymbol = GetPieceSymbol(move.PieceType, move.Color).Trim();
        string colorTag = move.Color == PieceColor.White ? "white" : "white";

        if (move.IsCastling)
        {
            string castleType = move.To.Column == 6 ? "O-O" : "O-O-O";
            return $"[{colorTag}]{pieceSymbol} {castleType}[/] [dim]({fromSquare}→{toSquare})[/]";
        }

        if (move.IsPromotion)
        {
            string promoSymbol = move.PromotedType.HasValue 
                ? GetPieceSymbol(move.PromotedType.Value, move.Color).Trim() 
                : "♕";
            return $"[{colorTag}]{pieceSymbol} {fromSquare}→{toSquare}[/] [bold green]={promoSymbol}[/]";
        }

        if (move.IsEnPassant)
        {
            return $"[{colorTag}]{pieceSymbol} {fromSquare} ⚔️ {toSquare}[/] [italic yellow](e.p.)[/]";
        }

        if (move.CapturedPiece != null)
        {
            string capturedSymbol = GetPieceSymbol(move.CapturedPiece).Trim();
            return $"[{colorTag}]{pieceSymbol} {fromSquare} ⚔️ {toSquare}[/] [red]({capturedSymbol})[/]";
        }

        return $"[{colorTag}]{pieceSymbol} {toSquare}[/]";
    }

    public static Table CreateHistoryMovePieceTable(IEnumerable<MoveHistory> moveHistory)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.Title = new TableTitle("[bold yellow]⚔️ Move History / Riwayat Pergerakan[/]");

        table.AddColumn(new TableColumn("[bold yellow]#[/]").Centered());
        table.AddColumn(new TableColumn("[bold white]♔ White[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold white]♚ Black[/]").LeftAligned());

        var movesList = moveHistory is Stack<MoveHistory> stack
            ? stack.Reverse().ToList()
            : moveHistory.ToList();

        if (movesList.Count == 0)
        {
            table.AddRow("[grey]-[/]", "[grey]Belum ada pergerakan[/]", "[grey]Belum ada pergerakan[/]");
            return table;
        }

        int turnNumber = 1;
        for (int i = 0; i < movesList.Count; i += 2)
        {
            var whiteMove = movesList[i];
            string whiteText = FormatMove(whiteMove);

            string blackText = "-";
            if (i + 1 < movesList.Count)
            {
                var blackMove = movesList[i + 1];
                blackText = FormatMove(blackMove);
            }
            else
            {
                blackText = "[grey]...[/]";
            }

            table.AddRow(
                $"[bold yellow]{turnNumber}.[/]",
                whiteText,
                blackText
            );
            turnNumber++;
        }

        return table;
    }

    public static void HistoryMovePiece(Board board)
    {
        if (board == null) return;
        var table = CreateHistoryMovePieceTable(board.MoveStack);
        AnsiConsole.Write(table);
    }



    
}