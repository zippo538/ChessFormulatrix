using System.Text.Json.Serialization;

namespace ChessAPI.Models;

public class BoardLocation
{
    private const int BoardSize = 8;
            
    public int Row{ get; set; }
    public int Column { get; set; }        
    
    private static bool IsRange(int position) => position >= 1 && position <= BoardSize;

    public BoardLocation(int row, int column)
    {
        Row = row;
        Column = column;
    }

    [JsonConstructor]
    public BoardLocation(){}
    
    public override string ToString() => $"({Row}, {Column})";

}