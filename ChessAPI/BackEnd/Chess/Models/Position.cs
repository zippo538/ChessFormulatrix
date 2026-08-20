namespace ChessAPI.BackEnd.Chess.Models;
public readonly struct Position : IEquatable<Position>
{
    public int Row { get; }
    public int Column  { get;  }

    public Position(int row, int column)
    {
        if (row < 0 || row > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(row),"Row must be between 0 and 7");
        }

        if (column < 0 || column > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(column),"Column must be between 0 and 7");
        }
        Row = row;
        Column = column;
    }

    public bool IsValid()
    {
        return Row >= 0 && Row < 7 && Column >= 0 && Column < 7;
    }
    private int RowDistance(Position other)
    {
        return Math.Abs(Row - other.Row);
    }

    private int ColumnDistance(Position other)
    {
        return Math.Abs(Column - other.Column);
    }

    private bool IsSameRow(Position other)
    {
        return Row == other.Row;
    }

    private bool IsSameColumn(Position other)
    {
        return Column == other.Column;
    }

    private bool IsDiagonalTo(Position other)
    {
        return RowDistance(other) ==
               ColumnDistance(other);
    }

    private string ToChessNotation()
    {
        char file = (char)('a' + Column);
        int rank = 8 - Row;

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
    public bool Equals(Position other)
    {
        return Row == other.Row &&
               Column == other.Column;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    public static bool operator ==(
        Position left,
        Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Position left,
        Position right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return ToChessNotation();
    }
}