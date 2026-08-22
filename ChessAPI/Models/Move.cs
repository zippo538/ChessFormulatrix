
namespace ChessAPI.Models;

public class Move
{
    public Tile From { get; set; }
    public Tile To { get; set; }

    public Move(Tile from, Tile to)
    {
        From = from;
        To = to;
    }

    
}