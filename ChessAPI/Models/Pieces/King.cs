using ChessAPI.Models.Enums;
using ChessAPI.Services;


namespace ChessAPI.Models.Pieces;

public class King : Piece
{
    public King() 
    {
        _symbol = PieceType.King;
    }
    public King(PieceColor player, int row, int col) : base(player, row, col)
    {
        _symbol = PieceType.King ;
    }

    public readonly static List<int[]> MoveTemplates = new()
    {
        new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 },
        new[] { -1, -1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { 1, 1 }
    };

    public override IList<Tile> GetValidMoves(Board board)
    {
        return MovementService.GetValidMoves(board,this);
    }

    public override Piece Clone()
    {
        return new King(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Column)
        {
            HasMoved = this.HasMoved
        };
    }
}