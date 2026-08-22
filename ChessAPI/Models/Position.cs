namespace ChessAPI.Models;
public class Position 
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

    
}