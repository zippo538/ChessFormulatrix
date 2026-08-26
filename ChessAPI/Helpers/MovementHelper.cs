using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Services;

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
        var movedPiece = from.Piece;
        var capturedPiece = to.Piece;
        
        // -----------------------------------------
        // EN Passant
        // -----------------------------------------
        bool isEnpassant = movedPiece.Symbol == PieceType.Pawn && from.Column != to.Column && to.Piece == null;
        if (isEnpassant)

        {

            var enPassantTile = BoardHelper.GetTile(board, from.Row, to.Column);

            if (enPassantTile != null)

            {

                capturedPiece = enPassantTile.Piece;

                enPassantTile.Piece = null;

            }

        }
        
        // -----------------------------------------
        // CASTLING (ROKADE)
        // -----------------------------------------
        //KINGSIDE
        int row=  from.Row;

        if (to.Column == 6 || to.Column == 7)
        {
            var rookTile = BoardHelper.GetTile(board, row, 7); // Posisi asli Benteng
            var targetRookTile = BoardHelper.GetTile(board, row, 5); // Target Benteng
            

            if (rookTile?.Piece != null && targetRookTile != null)
            {
                var rook = rookTile.Piece;
                targetRookTile.Piece = rook;       // Pindahkan Benteng
                rookTile.Piece = null;             // Kosongkan petak asal Benteng
                rook.CurrentLocation = new BoardLocation(row, 5);
                rook.HasMoved = true;              // Hanguskan hak castling
            }
        }
        // QUEENSIDE    
        else if(to.Column == 2)
        {
            var rookTile = BoardHelper.GetTile(board, row, 0); // Posisi asli Benteng
            var targetRookTile = BoardHelper.GetTile(board, row, 3); // Target Benteng

            if (rookTile?.Piece != null && targetRookTile != null)
            {
                var rook = rookTile.Piece;
                targetRookTile.Piece = rook;       // Pindahkan Benteng
                rookTile.Piece = null;             // Kosongkan petak asal Benteng
                rook.CurrentLocation = new BoardLocation(row, 3);
                rook.HasMoved = true;              // Hanguskan hak castling
            }
        }
        
        
        // -----------------------------------------
        // PROMOTION
        // -----------------------------------------

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
        
        // 5. HasMoved true
        movedPiece.HasMoved = true;

        // 6. Jika bidak yang dipindah adalah Raja, perbarui status lokasinya di Board
        UpdateKingPosition(board, movedPiece);
    }
    public static void UpdateKingPosition(Board board, Piece piece)
    {
        // Abaikan jika bidak yang dimasukkan bukan Raja
        if (piece.Symbol != PieceType.King)
        {
            return;
        }

        var newLocation = new BoardLocation(piece.CurrentLocation.Row, piece.CurrentLocation.Column);

        if (piece.Color == PieceColor.White)
        {
            board.WhiteKingLocation = newLocation;
        }
        else
        {
            board.BlackKingLocation = newLocation;
        }
    }
    // check specific tile 
    public static bool IsSquareAttacked(Board board, Tile targetTile, PieceColor defendingColor)
    {
        var opponentColor = PieceHelper.GetOpponentColor(defendingColor);

        foreach (var tile in board.Tiles)
        {
            var attacker = tile.Piece;
            if (attacker == null || attacker.Color != opponentColor)
                continue;

            // Generate serangan dari lawan
            var attackedTiles = MovementService.GetGenerateAttackSquares(board, attacker);

            // Jika targetTile ada di dalam daftar serangan, berarti petak tersebut tidak aman
            if (attackedTiles.Any(attacked => attacked.Row == targetTile.Row && attacked.Column == targetTile.Column))
            {
                return true;
            }
        }
        return false;
    }

   
}