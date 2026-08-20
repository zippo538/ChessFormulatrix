using System.Text.Json.Serialization;

namespace ChessAPI.BackEnd.Chess.Board;

public class BoardLocation
{
    private const int BoardSize = 8;
            
    public int Row{ get; set; }
    public int Columns { get; set; }        
    
    private static bool IsRange(int position) => position >= 1 && position <= BoardSize;

    public BoardLocation(int row, int columns)
    {
        Row = row;
        Columns = columns;
    }

    [JsonConstructor]
    public BoardLocation(){}
    
    public override string ToString() => $"({Row}, {Columns})";

}