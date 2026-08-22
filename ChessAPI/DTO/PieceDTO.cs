using ChessAPI.Models.Enums;
namespace ChessAPI.DTO;

public record PieceDto(
    PieceType Type,
    PieceColor Color,
    char Symbol,
    int Row,
    int Column
);