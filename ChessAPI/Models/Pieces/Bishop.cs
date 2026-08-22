using ChessAPI.Models.Enums;
using ChessAPI.Services;

namespace ChessAPI.Models.Pieces;


public class Bishop : Piece
{
    public Bishop() 
    {
        _symbol = PieceType.Bishop ;
    }

    public Bishop(PieceColor player, int row, int col) : base(player, row, col)
    {
        _symbol = PieceType.Bishop ;
    }

    public readonly static List<int[]> MoveTemplates = new()
    {
        new[] { -1, -1 }, // Kiri Atas
        new[] { -1, 1 },  // Kanan Atas
        new[] { 1, -1 },  // Kiri Bawah
        new[] { 1, 1 }    // Kanan Bawah
    };

    public override IList<Tile> GetValidMoves(Board board)
    {
        return MovementService.GetValidMoves(board,this);
    }

    public override Piece Clone()
    {
        return new Bishop(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Columns);
    }

}