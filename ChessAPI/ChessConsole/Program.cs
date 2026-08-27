using ChessAPI.Models;
using ChessAPI.Services;
using ChessAPI.Helpers;
using ChessAPI.Models.Interfaces;
using ChessAPI.Models.Enums;
using Spectre.Console;

Board board = new();
GameService gameService = new GameService();
BoardService.InitializeBoard(board);
TimerService timerService = new TimerService(TimeSpan.FromMinutes(10));
ChessDotNetService botService = new ChessDotNetService();

// White Turn
PieceColor currentTurn = PieceColor.White;
bool isGameOver = false;
string? gameResult = null;

Console.Clear();
var titleFiglet = new FigletText("CHESS CONSOLE")
{
    Justification = Justify.Center
}.Color(Color.Yellow);
AnsiConsole.Write(titleFiglet);

// ==========================================
// PILIH MODE PERMAINAN
// ==========================================
var modeChoice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("\n[bold yellow]Pilih Mode Permainan:[/]")
        .PageSize(5)
        .HighlightStyle(new Style(foreground: Color.Green))
        .AddChoices(
            "🎮 1. Player vs Player",
            "🤖 2. Player vs Bot"
        )
);

bool isVsBot = modeChoice.Contains("Bot");
PieceColor botColor = PieceColor.Black; // Bot secara default bermain sebagai Hitam (Black)


// start timer
timerService.Start();
GameHelper.StartLiveTitleTimer(timerService);

timerService.TimeExpired += color =>
{
    isGameOver = true;
    gameResult = $"{color} time out. " +
                 $"{GameHelper.GetOpponent(color)} wins!";
};

AnsiConsole.MarkupLine("[bold yellow]Chess Console[/]\n");
while (!isGameOver)
{
    GameService.RenderBoard(board, currentTurn);

    if (MovementService.IsKingInCheck(board, currentTurn))
    {
        AnsiConsole.MarkupLine("[bold red blink]CHECK![/]");
    }

    Tile? fromTile = null;
    Tile? toTile = null;

    // ==========================================
    // LOGIKA PERGERAKAN (BOT vs MANUSIA)
    // ==========================================
    if (isVsBot && currentTurn == botColor)
    {
        // --------------------------------------
        // JIKA GILIRAN BOT
        // --------------------------------------
        AnsiConsole.MarkupLine("[bold yellow]🤖 Bot sedang menganalisis langkah...[/]");
        Thread.Sleep(800); // Jeda singkat agar terasa alami

        var botMove = botService.GetBestMove(board, currentTurn);
        fromTile = botMove.From;
        toTile = botMove.To;

        if (fromTile == null || toTile == null)
        {
            // Jika bot tidak punya langkah valid
            break;
        }
    }
    else
    {
        // --------------------------------------
        // JIKA GILIRAN PEMAIN MANUSIA
        // --------------------------------------
        var movablePieces = new Dictionary<string, IPiece>();
        for (int r = 0; r < board.Size; r++)
        {
            for (int c = 0; c < board.Size; c++)
            {
                var p = BoardHelper.GetPiece(board, r, c);
                
                if (p != null && p.Color == currentTurn)
                {
                    if (p.GetValidMoves(board).Count > 0)
                    {
                        char displayCol = (char)('A' + c);
                        int displayRow = 8 - r;
                        
                        string label = $"{GameHelper.GetPieceSymbol(p)} {p.Symbol} at {displayCol}{displayRow}";
                        movablePieces[label] = p;
                    }
                }
            }
        }

        if (movablePieces.Count == 0) break;

        var selectedPieceLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"\n[bold {currentTurn}]Select piece to move:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(foreground: Color.Green))
                .AddChoices(movablePieces.Keys)
        );

        var piece = movablePieces[selectedPieceLabel];
        var validMoves = piece.GetValidMoves(board);

        Console.Clear();
        GameService.RenderBoard(board, currentTurn,validMoves);

        var moveOptions = new Dictionary<string, Tile>();
        var moveLabels = new List<string> { "🔙 Cancel / Back" }; 

        foreach (var tile in validMoves)
        {
            char destCol = (char)('A' + tile.Column);
            int destRow = 8 - tile.Row;

            string label = tile.Piece != null
                ? $"⚔️ Capture {GameHelper.GetPieceSymbol(tile.Piece)} {tile.Piece.Symbol} at {destCol}{destRow}"
                : $"➡️ Move to {destCol}{destRow}";
            
            moveOptions[label] = tile;
            moveLabels.Add(label);
        }

        var selectedMoveLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"\nSelected: [bold yellow]{GameHelper.GetPieceSymbol(piece)} {piece.Symbol}[/]. Choose destination:")
                .PageSize(10)
                .HighlightStyle(new Style(foreground: Color.Cyan1))
                .AddChoices(moveLabels)
        );

        if (selectedMoveLabel == "🔙 Cancel / Back")
        {
            continue; 
        }

        fromTile = BoardHelper.GetTile(board, piece.CurrentLocation.Row, piece.CurrentLocation.Column);
        toTile = moveOptions[selectedMoveLabel];
    }

    // ==========================================
    // EKSEKUSI PERGERAKAN (PEMAIN / BOT)
    // ==========================================
    if (fromTile != null && toTile != null)
    {
        MovementHelper.MovePiece(board, fromTile, toTile, promotionTile =>
        {
            if (isVsBot && currentTurn == botColor)
            {
                // Bot otomatis memilih Queen saat promosi
                MovementHelper.Promote(promotionTile, PieceType.Queen);
            }
            else
            {
                Console.Clear();
                gameService.GetDrawBoard(board);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"\n[bold yellow]♟ PROMOTION![/] Pilih piece untuk promosi:")
                        .AddChoices("♕ Queen", "♖ Rook", "♗ Bishop", "♘ Knight")
                );

                PieceType chosenType = choice switch
                {
                    "♕ Queen"  => PieceType.Queen,
                    "♖ Rook"   => PieceType.Rook,
                    "♗ Bishop" => PieceType.Bishop,
                    "♘ Knight" => PieceType.Knight,
                    _          => PieceType.Queen  
                };

                MovementHelper.Promote(promotionTile, chosenType);
                AnsiConsole.MarkupLine($"[green]Pawn berhasil dipromosikan menjadi [bold]{chosenType}[/]![/]");
            }
        });
    }

    // ==========================================
    // PERGANTIAN GILIRAN & CEK STATUS
    // ==========================================
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
    else if (timerService.BlackTime == TimeSpan.Zero || timerService.WhiteTime == TimeSpan.Zero)
    {
        isGameOver = true;
        gameResult = $"Time Is Over! Draw.";
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
if (board.MoveStack.Count > 0)
{
    GameHelper.HistoryMovePiece(board);
}
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();




