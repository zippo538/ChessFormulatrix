using ChessAPI.BackEnd.Chess.Enums;
using ChessAPI.BackEnd.Chess.Board;

namespace ChessAPI.BackEnd.Chess.Pieces;

public abstract class Piece : IPiece
{
    private protected BoardLocation _currentLocation;
    private protected PieceType _symbol;
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
    

    
    public abstract IList<Tile> GetValidMoves(Board.Board board);
    public abstract Piece Clone();

    IPiece IPiece.Clone()
    {
        return Clone();
    }
    
    public static char ToCharPiece(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Pawn => 'p',
            PieceType.Rook => 'r',
            PieceType.Knight => 'n', // 'N' umum digunakan untuk Knight karena 'K' sudah dipakai King
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => ' ' 
        };
    }
    public static char ToCharColor(PieceColor pieceColor)
    {
        return pieceColor switch
        {
            PieceColor.Black => 'b',
            PieceColor.White => 'w',
            _ => ' ' 
        };
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