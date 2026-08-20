using ChessAPI.BackEnd.Chess.Pieces;
using ChessAPI.BackEnd.Chess.Enums;
namespace ChessAPI.BackEnd.Chess.Game;

public record GameStateDTO(
    Guid GameId,
    PieceColor Turn,
    bool IsGameOver,
    PieceColor? Winner,
    IReadOnlyList<PieceDto> Pieces
    );