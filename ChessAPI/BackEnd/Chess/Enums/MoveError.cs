namespace ChessAPI.BackEnd.Chess.Enums;

public enum MoveError
{
    None = 0,
    NoPiece =1,
    WrongTurn = 2,
    InvalidMovement = 3,
    OwnPieceOnDestination = 4,
    KingWouldBeInCheck =5,
    GameAlreadyFinished =6,
}