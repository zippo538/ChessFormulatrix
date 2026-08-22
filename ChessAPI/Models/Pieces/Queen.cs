using ChessAPI.Models.Enums;
using ChessAPI.Services;


namespace ChessAPI.Models.Pieces;

public class Queen : Piece
{
    public Queen()
    {
        _symbol = PieceType.Queen;
    }
    public Queen(PieceColor player, int row, int col) : base(player, row, col)
    {
        _symbol = PieceType.Queen ;
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
        return new Queen(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Columns);
    }      
}