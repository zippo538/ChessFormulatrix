using ChessAPI.BackEnd.Chess.Enums;
using ChessAPI.BackEnd.Chess.Pieces;

namespace ChessAPI.BackEnd.Chess.Board;


public class Board 
{
    public const int DefaultSize = 8;

    private readonly Tile[,] _tiles;

    private readonly Stack<MoveHistory> _moveStack = new();

    public int Size { get; }

    /*
     * Untuk sementara tetap diberikan karena
     * Movement kamu menggunakan:
     *
     * foreach (var tile in board.Tiles)
     *
     * Tidak diberikan setter supaya array board
     * tidak bisa diganti dari luar.
     */
    public Tile[,] Tiles => _tiles;

    public BoardLocation WhiteKingLocation { get; private set; }

    public BoardLocation BlackKingLocation { get; private set; }

    /*
     * Null = tidak ada king yang sedang check.
     */
    public PieceColor? KingInCheck { get; internal set; }


    // ================================
    // CONSTRUCTOR
    // ================================

    public Board()
        : this(DefaultSize, true)
    {
    }

    public Board(
        int size,
        bool addDefaultPieces = true)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size));
        }

        Size = size;

        _tiles = new Tile[size, size];

        CreateTiles();

        WhiteKingLocation =
            new BoardLocation(7, 4);

        BlackKingLocation =
            new BoardLocation(0, 4);

        if (addDefaultPieces)
        {
            AddDefaultPieces();
        }
    }


    // ================================
    // CREATE BOARD
    // ================================

    private void CreateTiles()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                _tiles[row, col] =
                    new Tile(row, col);
            }
        }
    }


    // ================================
    // DEFAULT PIECES
    // ================================

    private void AddDefaultPieces()
    {
        if (Size != 8)
        {
            throw new InvalidOperationException(
                "Default chess setup requires an 8x8 board.");
        }

        // Black Pawns
        for (int col = 0; col < 8; col++)
        {
            AddPiece(
                new Pawn(
                    Piece.ToCharColor(PieceColor.Black),
                    1,
                    col));
        }

        // White Pawns
        for (int col = 0; col < 8; col++)
        {
            AddPiece(
                new Pawn(
                    Piece.ToCharColor(PieceColor.White),
                    6,
                    col));
        }


        // ==========================
        // BLACK PIECES
        // ==========================

        AddPiece(
            new Rook(
                Piece.ToCharColor(PieceColor.Black),
                0,
                0));

        AddPiece(
            new Knight(
                Piece.ToCharColor(PieceColor.Black),
                0,
                1));

        AddPiece(
            new Bishop(
                Piece.ToCharColor(PieceColor.Black),
                0,
                2));

        AddPiece(
            new Queen(
                Piece.ToCharColor(PieceColor.Black),
                0,
                3));

        AddPiece(
            new King(
                Piece.ToCharColor(PieceColor.Black),
                0,
                4));

        AddPiece(
            new Bishop(
                Piece.ToCharColor(PieceColor.Black),
                0,
                5));

        AddPiece(
            new Knight(
                Piece.ToCharColor(PieceColor.Black),
                0,
                6));

        AddPiece(
            new Rook(
                Piece.ToCharColor(PieceColor.Black),
                0,
                7));


        // ==========================
        // WHITE PIECES
        // ==========================

        AddPiece(
            new Rook(
                Piece.ToCharColor(PieceColor.White),
                7,
                0));

        AddPiece(
            new Knight(
                Piece.ToCharColor(PieceColor.White),
                7,
                1));

        AddPiece(
            new Bishop(
                Piece.ToCharColor(PieceColor.White),
                7,
                2));

        AddPiece(
            new Queen(
                Piece.ToCharColor(PieceColor.White),
                7,
                3));

        AddPiece(
            new King(
                Piece.ToCharColor(PieceColor.White),
                7,
                4));

        AddPiece(
            new Bishop(
                Piece.ToCharColor(PieceColor.White),
                7,
                5));

        AddPiece(
            new Knight(
                Piece.ToCharColor(PieceColor.White),
                7,
                6));

        AddPiece(
            new Rook(
                Piece.ToCharColor(PieceColor.White),
                7,
                7));
    }


    // ================================
    // BOUNDARY
    // ================================

    public bool IsInBounds(
        int row,
        int col)
    {
        return row >= 0 &&
               row < Size &&
               col >= 0 &&
               col < Size;
    }

    public bool IsInBounds(
        BoardLocation location)
    {
        return IsInBounds(
            location.Row,
            location.Columns);
    }


    // ================================
    // GET TILE
    // ================================

    public Tile? GetTile(
        int row,
        int col)
    {
        if (!IsInBounds(row, col))
        {
            return null;
        }

        return _tiles[row, col];
    }

    public Tile? GetTile(
        BoardLocation location)
    {
        return GetTile(
            location.Row,
            location.Columns);
    }


    // ================================
    // GET PIECE
    // ================================

    public IPiece? GetPiece(
        int row,
        int col)
    {
        var tile = GetTile(row, col);

        return tile?.Piece;
    }

    public IPiece? GetPiece(
        BoardLocation location)
    {
        return GetPiece(
            location.Row,
            location.Columns);
    }


    // ================================
    // ADD PIECE
    // ================================

    internal void AddPiece(Piece piece)
    {
        if (piece == null)
        {
            throw new ArgumentNullException(
                nameof(piece));
        }

        var location =
            piece.CurrentLocation;

        if (!IsInBounds(location))
        {
            throw new ArgumentOutOfRangeException(
                nameof(piece),
                "Piece position is outside the board.");
        }

        _tiles[
            location.Row,
            location.Columns
        ].Piece = piece;

        UpdateKingPosition(piece);
    }


    // ================================
    // MOVE PIECE
    // ================================

    internal void MovePiece(
        Tile from,
        Tile to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        if (from.Piece == null)
        {
            throw new InvalidOperationException(
                "Source tile does not contain a piece.");
        }

        var movedPiece =
            from.Piece;

        var capturedPiece =
            to.Piece;

        /*
         * Simpan history sebelum board diubah.
         */
        _moveStack.Push(
            new MoveHistory(
                new BoardLocation(
                    from.Row,
                    from.Column),

                new BoardLocation(
                    to.Row,
                    to.Column),

                capturedPiece!
            )
        );

        // pindahkan piece
        to.Piece = movedPiece;

        from.Piece = null;

        movedPiece.CurrentLocation =
            new BoardLocation(
                to.Row,
                to.Column);

        UpdateKingPosition(movedPiece);
    }


    internal void MovePiece(
        BoardLocation from,
        BoardLocation to)
    {
        var fromTile =
            GetTile(from)
            ?? throw new ArgumentException(
                "Source location does not exist.",
                nameof(from));

        var toTile =
            GetTile(to)
            ?? throw new ArgumentException(
                "Destination location does not exist.",
                nameof(to));

        MovePiece(
            fromTile,
            toTile);
    }


    // ================================
    // UNDO
    // ================================

    internal void UndoMove()
    {
        if (_moveStack.Count == 0)
        {
            return;
        }

        var move =
            _moveStack.Pop();

        var fromTile =
            GetTile(move.From)
            ?? throw new InvalidOperationException(
                "Invalid move history.");

        var toTile =
            GetTile(move.To)
            ?? throw new InvalidOperationException(
                "Invalid move history.");

        var movedPiece =
            toTile.Piece;

        if (movedPiece == null)
        {
            throw new InvalidOperationException(
                "Moved piece does not exist.");
        }

        // pindahkan kembali
        fromTile.Piece =
            movedPiece;

        movedPiece.CurrentLocation =
            new BoardLocation(
                move.From.Row,
                move.From.Columns);

        // restore captured piece
        toTile.Piece =
            move.CapturedPiece;

        UpdateKingPosition(
            movedPiece);

        KingInCheck = null;
    }


    // ================================
    // KING LOCATION
    // ================================

    private void UpdateKingPosition(
        Piece piece)
    {
        if (piece is not King)
        {
            return;
        }

        var newLocation =
            new BoardLocation(
                piece.CurrentLocation.Row,
                piece.CurrentLocation.Columns);

        if (piece.Color == Piece.ToCharColor(PieceColor.White))
        {
            WhiteKingLocation =
                newLocation;
        }
        else
        {
            BlackKingLocation =
                newLocation;
        }
    }


    // ================================
    // DEEP COPY
    // ================================

    internal Board Copy()
    {
        /*
         * FALSE:
         * jangan membuat default pieces,
         * karena kita akan clone posisi saat ini.
         */
        var copy =
            new Board(Size, false);

        for (int row = 0;
             row < Size;
             row++)
        {
            for (int col = 0;
                 col < Size;
                 col++)
            {
                var piece =
                    _tiles[row, col].Piece;

                if (piece == null)
                {
                    continue;
                }

                var clonedPiece =
                    piece.Clone();

                clonedPiece.CurrentLocation =
                    new BoardLocation(
                        row,
                        col);

                copy._tiles[
                    row,
                    col
                ].Piece = clonedPiece;
            }
        }

        copy.WhiteKingLocation =
            new BoardLocation(
                WhiteKingLocation.Row,
                WhiteKingLocation.Columns);

        copy.BlackKingLocation =
            new BoardLocation(
                BlackKingLocation.Row,
                BlackKingLocation.Columns);

        copy.KingInCheck =
            KingInCheck;

        return copy;
    }


    // ================================
    // INTERNAL MOVE HISTORY
    // ================================

    private sealed class MoveHistory
    {
        public BoardLocation From { get; }

        public BoardLocation To { get; }

        public Piece CapturedPiece { get; }

        public MoveHistory(
            BoardLocation from,
            BoardLocation to,
            Piece capturedPiece)
        {
            From = from;
            To = to;
            CapturedPiece = capturedPiece;
        }
    }
}