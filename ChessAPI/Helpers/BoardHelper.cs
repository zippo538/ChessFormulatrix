using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Services;

namespace ChessAPI.Helpers;

public  class BoardHelper
{
    public static bool IsInBounds(Board board, int row, int col)
    {
        return row >= 0 && row < board.Size && col >= 0 && col < board.Size;
    }

    public static Tile? GetTile(Board board, int row, int col)
    {
        if (!IsInBounds(board, row, col)) return null;
        return board.Tiles[row, col];
    }

    public static IPiece? GetPiece(Board board, int row, int col)
    {
        var tile = GetTile(board, row, col);
        return tile?.Piece;
    }

    public static void AddPiece(Board board, Piece piece)
    {
        ArgumentNullException.ThrowIfNull(piece);
        var location = piece.CurrentLocation;

        if (!IsInBounds(board, location.Row, location.Columns))
            throw new ArgumentOutOfRangeException(nameof(piece), "Piece position is outside the board.");

        board.Tiles[location.Row, location.Columns].Piece = piece;
        MovementHelper.UpdateKingPosition(board, piece);
    }
    public static int EvaluateBoard(Board board)
    {
        int score = 0;

        // Loop through the board and evaluate each piece
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                IPiece piece = GetPiece(board,row, col)!; // Get the piece at the current position

                if (piece == null)
                {
                    continue;
                }

                int pieceValue = 0;

                // Tentukan nilai dasar bidak menggunakan enum PieceType
                switch (piece.Symbol)
                {
                    case PieceType.Pawn:
                        pieceValue = 1; 
                        break;
                    case PieceType.Knight:
                    case PieceType.Bishop:
                        pieceValue = 3; 
                        break;
                    case PieceType.Rook:
                        pieceValue = 5; 
                        break;
                    case PieceType.Queen:
                        pieceValue = 9; 
                        break;
                    case PieceType.King:
                        pieceValue = 100; 
                        break;
                }
                if (piece.Color == PieceColor.White)
                {
                    score += pieceValue;
                }
                else if (piece.Color == PieceColor.Black)
                {
                    score -= pieceValue;
                }

            }
        }

        return score;
    }
    public static Tile? GetTileByPiece(IPiece piece, Board board)
    {
        foreach (Tile tile in board.Tiles)
        {
            if (tile.Row == piece.CurrentLocation.Row && tile.Column == piece.CurrentLocation.Columns)
                return tile;
        }
        return null;
    }
    public static Board Copy(Board originalBoard)
    {
        var copy = new Board(originalBoard.Size);
        BoardService.InitializeBoard(copy, addDefaultPieces: false); // Create empty tiles

        for (int row = 0; row < originalBoard.Size; row++)
        {
            for (int col = 0; col < originalBoard.Size; col++)
            {
                var piece = originalBoard.Tiles[row, col].Piece;
                if (piece == null) continue;

                var clonedPiece = (Piece)piece.Clone();
                clonedPiece.CurrentLocation = new BoardLocation(row, col);
                copy.Tiles[row, col].Piece = clonedPiece;
            }
        }

        copy.WhiteKingLocation = new BoardLocation(originalBoard.WhiteKingLocation.Row, originalBoard.WhiteKingLocation.Columns);
        copy.BlackKingLocation = new BoardLocation(originalBoard.BlackKingLocation.Row, originalBoard.BlackKingLocation.Columns);

        return copy;
    }
}