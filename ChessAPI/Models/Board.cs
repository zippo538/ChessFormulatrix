namespace ChessAPI.Models;


public class Board 
{
    public const int DefaultSize = 8;

    public int Size { get; set; }
    
    public Tile[,] Tiles { get; set; }
    
    public Stack<MoveHistory> MoveStack { get; set; } = new();

    public BoardLocation WhiteKingLocation { get; set; }
    
    public BoardLocation BlackKingLocation { get; set; }

    // Constructor murni hanya untuk menyiapkan state awal
    public Board(int size = DefaultSize)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        
        Size = size;
        Tiles = new Tile[size, size];
        WhiteKingLocation = new BoardLocation(7, 4);
        BlackKingLocation = new BoardLocation(0, 4);
    }


    
    
}