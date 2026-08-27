using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;

namespace ChessAPI.Models;

public class MoveHistory
{
    public BoardLocation From { get; set; }
    public BoardLocation To { get; set; }
    public Piece? CapturedPiece { get; set; }
    public IPiece? MovedPiece { get; set; }
    public PieceColor Color { get; set; }
    public PieceType PieceType { get; set; }
    public bool IsCastling { get; set; }
    public bool IsEnPassant { get; set; }
    public bool IsPromotion { get; set; }
    public PieceType? PromotedType { get; set; }

    public MoveHistory(BoardLocation from, BoardLocation to, Piece? capturedPiece = null, IPiece? movedPiece = null)
    {
        From = from;
        To = to;
        CapturedPiece = capturedPiece;
        MovedPiece = movedPiece;
        if (movedPiece != null)
        {
            Color = movedPiece.Color;
            PieceType = movedPiece.Symbol;
        }
    }
}