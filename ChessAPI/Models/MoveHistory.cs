namespace ChessAPI.Models;

public class MoveHistory
{
    public BoardLocation From { get; }
    public BoardLocation To { get; }
    public Piece CapturedPiece { get; }

    public MoveHistory(BoardLocation from, BoardLocation to, Piece capturedPiece)
    {
        From = from;
        To = to;
        CapturedPiece = capturedPiece;
    }
}