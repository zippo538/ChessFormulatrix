
using ChessAPI.Models.Enums;

namespace ChessAPI.Models.Interfaces;

public interface IPiece
{
    PieceColor Color { get; set; }

    BoardLocation CurrentLocation { get; set; }
    
    PieceType Symbol { get; }
    
    IList<Tile> GetValidMoves(Board board);
    
    IPiece Clone();
    
    
}