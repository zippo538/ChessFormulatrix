using ChessAPI.Helpers;
using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Pieces;


namespace ChessAPI.Services;

public class BoardService
{
    // ================================
    // INITIALIZATION
    // ================================
    public static void InitializeBoard(Board board, bool addDefaultPieces = true)
    {
        // Create Tiles
        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                board.Tiles[row, col] = new Tile(row, col);
            }
        }

        if (addDefaultPieces)
        {
            AddDefaultPieces(board);
        }
    }

    private static void AddDefaultPieces(Board board)
    {
        if (board.Size != 8)
            throw new InvalidOperationException("Default chess setup requires an 8x8 board.");

        // Black & White Pawns
        for (int col = 0; col < 8; col++)
        {
            BoardHelper.AddPiece(board, new Pawn(PieceColor.Black, 1, col));
            BoardHelper.AddPiece(board, new Pawn(PieceColor.White, 6, col));
        }

        // Black Backrow
        BoardHelper.AddPiece(board, new Rook(PieceColor.Black, 0, 0));
        BoardHelper.AddPiece(board, new Knight(PieceColor.Black, 0, 1));
        BoardHelper.AddPiece(board, new Bishop(PieceColor.Black, 0, 2));
        BoardHelper.AddPiece(board, new Queen(PieceColor.Black, 0, 3));
        BoardHelper.AddPiece(board, new King(PieceColor.Black, 0, 4));
        BoardHelper.AddPiece(board, new Bishop(PieceColor.Black, 0, 5));
        BoardHelper.AddPiece(board, new Knight(PieceColor.Black, 0, 6));
        BoardHelper.AddPiece(board, new Rook(PieceColor.Black, 0, 7));

        // White Backrow
        BoardHelper.AddPiece(board, new Rook(PieceColor.White, 7, 0));
        BoardHelper.AddPiece(board, new Knight(PieceColor.White, 7, 1));
        BoardHelper.AddPiece(board, new Bishop(PieceColor.White, 7, 2));
        BoardHelper.AddPiece(board, new Queen(PieceColor.White, 7, 3));
        BoardHelper.AddPiece(board, new King(PieceColor.White, 7, 4));
        BoardHelper.AddPiece(board, new Bishop(PieceColor.White, 7, 5));
        BoardHelper.AddPiece(board, new Knight(PieceColor.White, 7, 6));
        BoardHelper.AddPiece(board, new Rook(PieceColor.White, 7, 7));
    }
}
