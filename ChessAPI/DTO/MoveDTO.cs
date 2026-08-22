namespace ChessAPI.DTO;

public record MoveDTO(
    int FromRow,
    int FromColumn,
    int ToRow,
    int ToColumn
    );