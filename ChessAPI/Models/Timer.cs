using ChessAPI.Models.Enums;

namespace ChessAPI.Models;

public class TimerModel
{
    public TimeSpan _remainingTime {get; set;}
    public readonly TimeSpan _increment;
    public PieceColor _pieceColor {get;}

    public TimerModel(TimeSpan initialTime, PieceColor pieceColor)
    {
        _remainingTime = initialTime;
        _pieceColor = pieceColor;
    }
}