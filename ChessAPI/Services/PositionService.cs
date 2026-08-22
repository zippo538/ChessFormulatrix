using ChessAPI.Models;

namespace ChessAPI.Services;

public class PositionService : IEquatable<Position>
{
    private readonly Position _position;
    public bool IsValid()
    {
        return _position.Row >= 0 && _position.Row < 7 && _position.Column >= 0 && _position.Column < 7;
    }
    private int RowDistance(Position other)
    {
        return Math.Abs(_position.Row - other.Row);
    }

    private int ColumnDistance(Position other)
    {
        return Math.Abs(_position.Column - other.Column);
    }

    private bool IsSameRow(Position other)
    {
        return _position.Row == other.Row;
    }

    private bool IsSameColumn(Position other)
    {
        return _position.Column == other.Column;
    }

    private bool IsDiagonalTo(Position other)
    {
        return RowDistance(other) ==
               ColumnDistance(other);
    }

    private string ToChessNotation()
    {
        char file = (char)('a' + _position.Column);
        int rank = 8 - _position.Row;

        return $"{file}{rank}";
    }

    public static Position FromChessNotation(string notation)
    {
        if (string.IsNullOrWhiteSpace(notation) ||
            notation.Length != 2)
        {
            throw new ArgumentException(
                "Invalid chess notation.");
        }

        char file = char.ToLower(notation[0]);
        char rankChar = notation[1];

        if (file < 'a' || file > 'h')
            throw new ArgumentException(
                "File must be between a and h.");

        if (rankChar < '1' || rankChar > '8')
            throw new ArgumentException(
                "Rank must be between 1 and 8.");

        int column = file - 'a';
        int rank = rankChar - '0';
        int row = 8 - rank;

        return new Position(row, column);
    }
    public bool Equals(Position? other)
    {
        return _position.Row == other?.Row &&
               _position.Column == other.Column;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_position.Row, _position.Column);
    }

    public static bool operator ==(
        PositionService left,
        PositionService right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        PositionService left,
        PositionService right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return ToChessNotation();
    }
}