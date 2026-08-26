using ChessAPI.Models.Enums;

namespace ChessAPI.Helpers;

public class PieceHelper
{
    public static char ToCharPiece(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Pawn => 'p',
            PieceType.Rook => 'r',
            PieceType.Knight => 'n', // 'N' umum digunakan untuk Knight karena 'K' sudah dipakai King
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => ' ' 
        };
    }
    public static char ToCharColor(PieceColor pieceColor)
    {
        return pieceColor switch
        {
            PieceColor.Black => 'b',
            PieceColor.White => 'w',
            _ => ' ' 
        };
    }
    public static PieceColor GetOpponentColor(
        PieceColor color)
    {
        return color == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;
    }
}