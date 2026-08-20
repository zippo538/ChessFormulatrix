namespace ChessAPI.BackEnd.Chess.Game;

public interface IGameService
{
    GameStateDTO CreateGame();

    GameStateDTO GetGame(Guid gameId);

    MoveResultDto MakeMove(
        Guid gameId,
        MakeMoveRequest request);
}