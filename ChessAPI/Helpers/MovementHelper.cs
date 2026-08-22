using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;

namespace ChessAPI.Helpers;

public class MovementHelper
{
    public static void MovePiece(Board board, Tile from, Tile to)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        if (from.Piece == null)
        {
            throw new InvalidOperationException("Source tile does not contain a piece.");
        }

        // Casting ke Piece karena MoveHistory dan logika papan menggunakan base class Piece
        var movedPiece = (Piece)from.Piece;
        var capturedPiece = (Piece?)to.Piece;

        // 1. Simpan riwayat pergerakan ke Stack sebelum papan diubah
        board.MoveStack.Push(new MoveHistory(
            new BoardLocation(from.Row, from.Column),
            new BoardLocation(to.Row, to.Column),
            capturedPiece! // Boleh null, ditangani oleh MoveHistory
        ));

        // 2. Pindahkan bidak ke petak tujuan
        to.Piece = movedPiece;
        
        // 3. Kosongkan petak asal
        from.Piece = null;

        // 4. Perbarui koordinat internal bidak tersebut
        movedPiece.CurrentLocation = new BoardLocation(to.Row, to.Column);

        // 5. Jika bidak yang dipindah adalah Raja, perbarui status lokasinya di Board
        UpdateKingPosition(board, movedPiece);
    }
    public static void UpdateKingPosition(Board board, Piece piece)
    {
        // Abaikan jika bidak yang dimasukkan bukan Raja
        if (piece.Symbol != PieceType.King)
        {
            return;
        }

        var newLocation = new BoardLocation(piece.CurrentLocation.Row, piece.CurrentLocation.Columns);

        if (piece.Color == PieceColor.White)
        {
            board.WhiteKingLocation = newLocation;
        }
        else
        {
            board.BlackKingLocation = newLocation;
        }
    }

   
}