using ChessAPI.BackEnd.Chess.Enums;
namespace ChessAPI.BackEnd.Chess.Pieces;

public record PieceDto(
    PieceType Type,
    PieceColor Color,
    char Symbol,
    int Row,
    int Column
);