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
    private static string GetBoardString(Board board, IList<Tile>? validMoves = null)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine();
        sb.AppendLine("    [bold]A  B  C  D  E  F  G  H[/]\n");
    
        for (int row = 0; row < board.Size; row++)
        {
            int displayRow = 8 - row;
            sb.Append($"[bold]{displayRow}[/]  ");
            
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
                    cellContent = GameHelper.GetPieceSymbol(tile.Piece);
                    fgColor = tile.Piece.Color == PieceColor.White ? "white" : "black";
                    if (isValidMove) bgColor = "red";
                }
                else if (isValidMove)
                {
                    cellContent = " • ";
                    fgColor = "blue";
                }
                sb.Append($"[{fgColor} on {bgColor}][bold]{cellContent}[/][/]");
                
                
            }
            sb.AppendLine($"  [bold]{displayRow}[/]");
            
          
        }
        sb.AppendLine("    [bold]A  B  C  D  E  F  G  H[/]\n");
        return sb.ToString();
    }
    private static void DrawBoardSideBySide(Board board, IList<Tile>? validMoves = null)
    {
        // 1. Render papan catur ke dalam Panel agar rapi
        var boardMarkup = GetBoardString(board, validMoves);
        var boardPanel = new Panel(boardMarkup)
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("[bold yellow] ♟ CHESS BOARD ♟ [/]", Justify.Center)
        };

        // 2. Buat tabel riwayat menggunakan helper yang sudah ada
        var historyTable = GameHelper.CreateHistoryMovePieceTable(board.MoveStack);

        // 3. Gabungkan Papan Catur dan Tabel Riwayat ke dalam Grid (2 Kolom Berdampingan)
        var grid = new Grid();
        grid.AddColumn(); // Kolom 0: Papan Catur
        grid.AddColumn(); // Kolom 1: Tabel Riwayat
        
        // Masukkan ke dalam baris grid dengan padding/jarak antar kolom (PadRight)
        grid.AddRow(
            boardPanel, 
            historyTable
        );

        // Cetak grid ke tengah layar terminal
        AnsiConsole.Write(
            new Align(
                grid,
                HorizontalAlignment.Center
            )
        );
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
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
    
    public static void RenderBoard(Board board, PieceColor turn)
    {
        Console.Clear();
        var figlet = new FigletText("CHESS CONSOLE")
        {
            Justification = Justify.Center


        }.Color(Color.Yellow);
        AnsiConsole.Write(figlet);
    
        // 1. Gambar tabel timer, papan catur, dan riwayat langkah
        // GameHelper.DrawTimerTable(timer, turn);
        var grid = new Grid();
        grid.AddColumn(); // Kolom 0: Papan Catur
        grid.AddColumn(); // Kolom 1: Tabel Riwayat
        var historyTable = GameHelper.CreateHistoryMovePieceTable(board.MoveStack);
        
        DrawBoardSideBySide(board);

        // if (board.MoveStack.Count > 0)
        // {
        //     GameHelper.HistoryMovePiece(board);
        // }

        // 2. Tentukan lawan main untuk teks status
        PieceColor opponent = turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
        string turnColor = turn == PieceColor.White ? "white" : "red";


        // 3. Gunakan 'Rule' dengan metode LeftAligned() atau properti yang benar
        AnsiConsole.MarkupLine($"       [{turnColor} bold]► {turn}'s Turn ◄[/]");

        // 4. Tampilkan status waiting menggunakan ikon dan tag markup
        AnsiConsole.MarkupLine(
            $"⏳ [italic]Status:[/] [blink bold green]Waiting for {turn} to move...[/] " +
            $"[dim]({opponent} is currently waiting)[/]\n"
        );
        
    }
}