using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;

namespace ChessAPI.Models;

public abstract class Piece : IPiece
{
    private protected BoardLocation _currentLocation;
    private protected PieceType _symbol;
    private bool _hasMoved = false;
    public PieceColor Color { get; set; }
    public PieceType Symbol
    {
        get
        {
            if (Color == PieceColor.Black)
            {
                return _symbol;
            }
            else
            {
                return _symbol;
            }
        }
        protected set => _symbol = value;
    }
     

    public BoardLocation CurrentLocation
    {
        get => _currentLocation;
         set =>
            _currentLocation = value
                               ?? throw new ArgumentNullException(nameof(value));
    }
    public bool HasMoved 
    { 
        get { return _hasMoved; }
        set { _hasMoved = value; }
    }
    

    
    public abstract IList<Tile> GetValidMoves(Board board);
    public abstract Piece Clone();

    IPiece IPiece.Clone()
    {
        return Clone();
    }
    
    

    public Piece() { }
    public Piece(PieceColor player, int row, int col)
    {
        if (player != PieceColor.White && player != PieceColor.Black)
        {
            throw new ArgumentException($"Invalid player {player}!");
        }

        Color = player;
        _currentLocation = new BoardLocation(row,col);
    }

    public override string ToString()
    {
        return $"{Color} {Symbol} at {CurrentLocation}";
    }
}