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
        // Ubah Header menjadi A sampai H
        AnsiConsole.MarkupLine("   [bold]A  B  C  D  E  F  G  H[/]");
    
        for (int row = 0; row < board.Size; row++)
        {
            // Konversi baris internal (0-7) menjadi baris catur visual (8-1)
            int displayRow = 8 - row;
            
            AnsiConsole.Markup($"[bold]{displayRow}[/] ");
            for (int col = 0; col < board.Size; col++)
            {
                var tile = board.Tiles[row, col];
                bool isValidMove = validMoves != null && validMoves.Any(m => m.Row == row && m.Column == col);
                bool isLightSquare = (row + col) % 2 == 0;
                string bgColor = isLightSquare ? "silver" : "green";
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
                AnsiConsole.Markup($"[{fgColor} on {bgColor}][bold]{cellContent}[/][/]");
                
            }
            // Cetak juga di sisi kanan
            AnsiConsole.MarkupLine($" [bold]{displayRow}[/]");
        }
    
        // Ubah Footer menjadi A sampai H
        AnsiConsole.MarkupLine("   [bold]A  B  C  D  E  F  G  H[/]\n");
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
    
        // 1. Gambar tabel timer dan papan catur
        GameHelper.DrawTimerTable(timer, turn);
        DrawBoard(board);

        // 2. Tentukan lawan main untuk teks status
        PieceColor opponent = turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
        string turnColor = turn == PieceColor.White ? "white" : "red";


        // 3. Gunakan 'Rule' dengan metode LeftAligned() atau properti yang benar
        var rule = new Rule($"[{turnColor} bold]► {turn}'s Turn ◄[/]")
        {
            Style = Style.Parse(turnColor)
        };

        AnsiConsole.Write(rule);

        // 4. Tampilkan status waiting menggunakan ikon dan tag markup
        AnsiConsole.MarkupLine(
            $"⏳ [italic]Status:[/] [blink bold green]Waiting for {turn} to move...[/] " +
            $"[dim]({opponent} is currently waiting)[/]\n"
        );
        
    }
}