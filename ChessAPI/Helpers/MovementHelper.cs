using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Models.Pieces;
using ChessAPI.Services;

namespace ChessAPI.Helpers;

public class MovementHelper
{
    public static void MovePiece(Board board, Tile from, Tile to,
        Action<Tile>? onPromotion = null)
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
        bool isCastling = movedPiece.Symbol == PieceType.King && Math.Abs(from.Column - to.Column) == 2;
        if (isCastling)
        {
        int row=  from.Row;
        if (to.Column == 6 )
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
        }
        

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

        // -----------------------------------------
        // PROMOTION
        // -----------------------------------------
        bool isPromotion = movedPiece.Symbol == PieceType.Pawn && 
                           ((movedPiece.Color == PieceColor.White && to.Row == 0) || 
                            (movedPiece.Color == PieceColor.Black && to.Row == 7));
        
        // Jika pawn mencapai baris terakhir, panggil callback agar caller
        // dapat menampilkan UI pilihan piece dan memanggil SpecialMove.Promote().
        if (isPromotion)
        {
            onPromotion?.Invoke(to);
        }

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
    public static void Promote(Tile promotionTile, PieceType chosenType)
    {
        if (promotionTile.Piece == null)
            throw new InvalidOperationException("Tidak ada piece di promotionTile.");

        var color = promotionTile.Piece.Color;
        int row   = promotionTile.Row;
        int col   = promotionTile.Column;

        Piece newPiece = chosenType switch
        {
            PieceType.Queen  => new Queen(color, row, col)  { HasMoved = true },
            PieceType.Rook   => new Rook(color, row, col)   { HasMoved = true },
            PieceType.Bishop => new Bishop(color, row, col) { HasMoved = true },
            PieceType.Knight => new Knight(color, row, col) { HasMoved = true },
            _ => throw new ArgumentException(
                $"Piece type '{chosenType}' tidak valid untuk promosi. " +
                "Pilih: Queen, Rook, Bishop, atau Knight.")
        };

        promotionTile.Piece = newPiece;
    }
    

   
}