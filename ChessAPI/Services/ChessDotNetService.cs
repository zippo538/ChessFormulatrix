
using System.Text;
using ChessDotNet;
using ChessAPI.Models;
using System.Collections.ObjectModel;
using ChessAPI.Models.Enums;
using ChessAPI.Helpers;
using Move = ChessDotNet.Move;

namespace ChessAPI.Services;

public class ChessDotNetService
{
    public (Tile? From, Tile? To) GetBestMove(Board board, PieceColor currentTurn)
    {
        // 1. Konversi Board internal Anda ke format string FEN
        string fen = ConvertBoardToFen(board, currentTurn);

        // 2. Inisialisasi game ChessDotNet dari FEN
        var game = new ChessGame(fen);

        // 3. Dapatkan semua langkah sah yang dimiliki bot
        Player playerToMove = currentTurn == PieceColor.White ? Player.White : Player.Black;
        var validMoves = game.GetValidMoves(playerToMove);

        if (validMoves.Count == 0)
            return (null, null);

        // 4. Pilih langkah terbaik (Logika sederhana: Prioritaskan memakan bidak, jika tidak ada pilih acak)
        Move chosenMove = SelectSmartMove(game, validMoves);

        // 5. Konversi koordinat Move milik ChessDotNet kembali ke Tile milik Anda
        return ConvertMoveToTiles(board, chosenMove);
    }

    private Move SelectSmartMove(ChessGame game, ReadOnlyCollection<Move> validMoves)
    {
        // Cari langkah yang memakan bidak lawan (Capture)
        foreach (var move in validMoves)
        {
            var targetPiece = game.GetPieceAt(move.NewPosition);
            if (targetPiece != null)
            {
                return move; // Utamakan memakan bidak
            }
        }

        // Jika tidak ada yang bisa dimakan, pilih langkah acak
        var random = new Random();
        return validMoves[random.Next(validMoves.Count)];
    }

    private (Tile? From, Tile? To) ConvertMoveToTiles(Board board, Move move)
    {
        // ChessDotNet menggunakan Position (misal: File.E, 4)
        int fromCol = (int)move.OriginalPosition.File;
        int fromRow = 8 - move.OriginalPosition.Rank;

        int toCol = (int)move.NewPosition.File;
        int toRow = 8 - move.NewPosition.Rank;

        var fromTile = BoardHelper.GetTile(board, fromRow, fromCol);
        var toTile = BoardHelper.GetTile(board, toRow, toCol);

        return (fromTile, toTile);
    }

    private string ConvertBoardToFen(Board board, PieceColor currentTurn)
    {
        var sb = new StringBuilder();

        for (int r = 0; r < board.Size; r++)
        {
            int emptyCount = 0;
            for (int c = 0; c < board.Size; c++)
            {
                var piece = BoardHelper.GetPiece(board, r, c);
                if (piece == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        sb.Append(emptyCount);
                        emptyCount = 0;
                    }

                    char symbol = piece.Symbol switch
                    {
                        PieceType.Pawn => 'p',
                        PieceType.Knight => 'n',
                        PieceType.Bishop => 'b',
                        PieceType.Rook => 'r',
                        PieceType.Queen => 'q',
                        PieceType.King => 'k',
                        _ => 'p'
                    };

                    sb.Append(piece.Color == PieceColor.White ? char.ToUpper(symbol) : symbol);
                }
            }

            if (emptyCount > 0) sb.Append(emptyCount);
            if (r < board.Size - 1) sb.Append('/');
        }

        sb.Append(currentTurn == PieceColor.White ? " w " : " b ");
        sb.Append("- - 0 1"); // Hak castling & en passant dasar

        return sb.ToString();
    }
}