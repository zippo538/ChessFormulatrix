using ChessAPI.BackEnd.Chess.Board;
using ChessAPI.BackEnd.Chess.Enums;

namespace ChessAPI.BackEnd.Chess.Pieces;

public interface IPiece
{
    PieceColor Color { get; set; }

    BoardLocation CurrentLocation { get; set; }
    
    PieceType Symbol { get; }
    
    IList<Tile> GetValidMoves(Board.Board board);
    
    IPiece Clone();
    
    
}