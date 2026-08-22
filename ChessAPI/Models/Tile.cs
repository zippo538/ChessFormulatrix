using System.Text.Json.Serialization;

namespace ChessAPI.Models;

public class Tile
{
    public Piece? Piece { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsEmptySpace { get { return Piece == null; } }

    public Tile(int row, int col)
    {
        Row = row;
        Column = col;
        Piece = null;
    }  
    public Tile(int row, int col,Piece? piece = null)
    {
        Row = row;
        Column = col;
        Piece = piece;
    }  
    [JsonConstructor]
    public Tile() {}
    
    public string GetDisplayCoordinates()
    {
        // 0 + 65 is the start of ascii uppercase characters
        // 65 + 32 is the start of ascii lowercase characters
        char rowCoordinate = Convert.ToChar(Row + 65 + 32);

        return rowCoordinate + Column.ToString();
    }

    public override string ToString()
    {
        if (Piece != null)
            return Piece.ToString()!;
        else
            return $"Empty tile at {Row}, {Column}";
    } 
}