using ChessAPI.Models.Enums;
using ChessAPI.Services;

namespace ChessAPI.Models.Pieces;

public class Pawn : Piece
{
    public Pawn() 
    {
        _symbol = PieceType.Pawn;
    }
    public Pawn(PieceColor player, int row, int col) : base(player, row, col)
    {
        _symbol = PieceType.Pawn ;
    }

 

    public override IList<Tile> GetValidMoves(Board board)
    {
        return MovementService.GetValidMoves(board,this);
    }

    public override Piece Clone()
    {
        return new Pawn(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Column)
        {
            HasMoved = this.HasMoved
        };
    }   
}