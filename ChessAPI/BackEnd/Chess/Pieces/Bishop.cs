using ChessAPI.BackEnd.Chess.Board;
using ChessAPI.BackEnd.Chess.Enums;
using ChessAPI.BackEnd.Chess.Models;

namespace ChessAPI.BackEnd.Chess.Pieces;


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

    private readonly static int[][] MoveTemplates = new int[][]
    {
        new [] { 1, -1 }, // down right
        new [] { 1, 1 }, // up right
        new [] { -1, -1 }, // down left
        new [] { -1, 1 }, // up left
    };

    public override IList<Tile> GetValidMoves(Board.Board board)
    {
        return Movement.GetMoves(board, this, board.Size, MoveTemplates);
    }

    public override Piece Clone()
    {
        return new Bishop(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Columns);
    }

}