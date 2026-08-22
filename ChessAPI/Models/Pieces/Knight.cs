using ChessAPI.Models.Enums;
using ChessAPI.Services;
namespace ChessAPI.Models.Pieces;

public class Knight : Piece
{
    public Knight() 
    {
        _symbol = PieceType.Knight;
    }
    public Knight(PieceColor player, int row, int col) : base(player, row, col)
    {
        _symbol = PieceType.Knight ;
    }

    public readonly static List<int[]> MoveTemplates = new()
    {
        new[] { -2, -1 }, new[] { -2, 1 }, // 2 Atas, 1 Kiri/Kanan
        new[] { -1, -2 }, new[] { -1, 2 }, // 1 Atas, 2 Kiri/Kanan
        new[] { 1, -2 },  new[] { 1, 2 },  // 1 Bawah, 2 Kiri/Kanan
        new[] { 2, -1 },  new[] { 2, 1 }   // 2 Bawah, 1 Kiri/Kanan
    };

    public override IList<Tile> GetValidMoves(Board board)
    {
        return MovementService.GetValidMoves(board,this);
    }

    public override Piece Clone()
    {
        return new Knight(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Columns);
    }
    
}