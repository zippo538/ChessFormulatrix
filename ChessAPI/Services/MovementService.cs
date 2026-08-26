using ChessAPI.Helpers;
using ChessAPI.Models;
using ChessAPI.Models.Enums;
using ChessAPI.Models.Interfaces;
using ChessAPI.Models.Pieces;

namespace ChessAPI.Services;

public static class MovementService
{
    // =========================================================
    // PUBLIC ENTRY POINT
    // =========================================================

    public static IList<Tile> GetValidMoves(
        Board board,
        IPiece piece)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(piece);

        /*
         * Pertama generate gerakan berdasarkan
         * pola dasar bidak.
         *
         * Ini disebut pseudo legal moves.
         */
        var pseudoLegalMoves =
            GeneratePseudoLegalMoves(
                board,
                piece);

        /*
         * Kemudian buang semua move
         * yang menyebabkan King sendiri check.
         */
        return FilterKingSafety(
            board,
            piece,
            pseudoLegalMoves);
    }


    // =========================================================
    // PSEUDO LEGAL MOVES
    // =========================================================

    private static IList<Tile> GeneratePseudoLegalMoves(
        Board board,
        IPiece piece)
    {
        return piece.Symbol switch
        {
            PieceType.Pawn =>
                GeneratePawnMoves(
                    board,
                    piece),

            PieceType.Knight =>
                GenerateJumpMoves(
                    board,
                    piece,
                    Knight.MoveTemplates),

            PieceType.Bishop =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    Bishop.MoveTemplates),

            PieceType.Rook =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    Rook.MoveTemplates),

            PieceType.Queen =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    Queen.MoveTemplates),

            PieceType.King =>
                GenerateKingMoves(
                    board,
                    piece),

            _ =>
                throw new InvalidOperationException(
                    $"Unsupported piece type {piece.Symbol}")
        };
    }


    // =========================================================
    // ROOK / BISHOP / QUEEN
    // =========================================================

    private static IList<Tile> GenerateSlidingMoves(
        Board board,
        IPiece piece,
        IEnumerable<int[]> directions)
    {
        var validMoves =
            new List<Tile>();

        foreach (var direction in directions)
        {
            int rowStep =
                direction[0];

            int colStep =
                direction[1];

            int row =
                piece.CurrentLocation.Row +
                rowStep;

            int col =
                piece.CurrentLocation.Column +
                colStep;

            /*
             * Terus bergerak sampai:
             *
             * - keluar board
             * - ketemu piece sendiri
             * - capture lawan
             */
            while (BoardHelper.IsInBounds(board,row, col))
            {
                var tile =
                    BoardHelper.GetTile(board,row, col);

                if (tile == null)
                    break;

                // kosong
                if (tile.Piece == null)
                {
                    validMoves.Add(tile);

                    row += rowStep;
                    col += colStep;

                    continue;
                }

                /*
                 * Ada piece.
                 *
                 * Kalau lawan → boleh capture.
                 */
                if (tile.Piece.Color != piece.Color)
                {
                    validMoves.Add(tile);
                }

                /*
                 * Entah piece sendiri atau lawan,
                 * movement berhenti di sini.
                 */
                break;
            }
        }

        return validMoves;
    }


    // =========================================================
    // KNIGHT
    // =========================================================

    private static IList<Tile> GenerateJumpMoves(
        Board board,
        IPiece piece,
        IEnumerable<int[]> directions)
    {
        var validMoves =
            new List<Tile>();

        foreach (var direction in directions)
        {
            int row =
                piece.CurrentLocation.Row +
                direction[0];

            int col =
                piece.CurrentLocation.Column +
                direction[1];

            if (!BoardHelper.IsInBounds(board,row, col))
                continue;

            var tile =
                BoardHelper.GetTile(board,row, col);

            if (tile == null)
                continue;

            /*
             * Knight boleh ke:
             *
             * - empty tile
             * - opponent tile
             *
             * Tidak boleh ke own piece.
             */
            if (tile.Piece == null ||
                tile.Piece.Color != piece.Color)
            {
                validMoves.Add(tile);
            }
        }

        return validMoves;
    }


    // =========================================================
    // KING
    // =========================================================
    private static IList<Tile> GenerateKingMoves(
        Board board,
        IPiece king)
    {
        var moves =
            GenerateJumpMoves(
                board,
                king,
                King.MoveTemplates);

        //castling
        var castlingMoves = GenerateCastlingMoves(board, king);
        foreach (var move in castlingMoves)
        {
            moves.Add(move);
        }

        return moves;
    }

    // =========================================================
    // PAWN
    // =========================================================

    private static IList<Tile> GeneratePawnMoves(
        Board board,
        IPiece pawn)
    {
        var validMoves =
            new List<Tile>();

        int direction =
            GetPawnDirection(
                pawn.Color);

        int startRow =
            pawn.Color == PieceColor.White
                ? 6
                : 1;


        // -----------------------------------------
        // 1 STEP FORWARD
        // -----------------------------------------

        int oneStepRow =
            pawn.CurrentLocation.Row +
            direction;

        int column =
            pawn.CurrentLocation.Column;

        if (BoardHelper.IsInBounds(
                board,
                oneStepRow,
                column))
        {
            var oneStep =
                BoardHelper.GetTile(
                    board,
                    oneStepRow,
                    column);

            if (oneStep?.Piece == null)
            {
                validMoves.Add(oneStep!);


                // ---------------------------------
                // 2 STEPS FROM INITIAL POSITION
                // ---------------------------------

                if (pawn.CurrentLocation.Row ==
                    startRow)
                {
                    int twoStepRow =
                        pawn.CurrentLocation.Row +
                        direction * 2;

                    if (BoardHelper.IsInBounds(
                            board,
                            twoStepRow,
                            column))
                    {
                        var twoStep =
                            BoardHelper.GetTile(
                                board,
                                twoStepRow,
                                column);

                        if (twoStep?.Piece == null)
                        {
                            validMoves.Add(twoStep!);
                        }
                    }
                }
            }
        }


        // -----------------------------------------
        // CAPTURE LEFT
        // -----------------------------------------

        AddPawnCapture(
            board,
            pawn,
            direction,
            -1,
            validMoves);


        // -----------------------------------------
        // CAPTURE RIGHT
        // -----------------------------------------

        AddPawnCapture(
            board,
            pawn,
            direction,
            1,
            validMoves);
        
        return validMoves;
    }


    private static void AddPawnCapture(
        Board board,
        IPiece pawn,
        int direction,
        int columnOffset,
        IList<Tile> moves)
    {
        int row =
            pawn.CurrentLocation.Row +
            direction;

        int col =
            pawn.CurrentLocation.Column +
            columnOffset;

        if (!BoardHelper.IsInBounds(board,row, col))
            return;

        var tile =
            BoardHelper.GetTile(board,row, col);

        if (tile?.Piece == null)
            return;

        if (tile.Piece.Color != pawn.Color)
        {
            moves.Add(tile);
        }
    }


    private static int GetPawnDirection(
        PieceColor color)
    {
        return color == PieceColor.White
            ? -1
            : 1;
    }


    // =========================================================
    // SELF CHECK VALIDATION
    // =========================================================

    private static IList<Tile> FilterKingSafety(
        Board board,
        IPiece piece,
        IEnumerable<Tile> moves)
    {
        var validMoves =
            new List<Tile>();

        foreach (var destination in moves)
        {
            /*
             * Penting:
             * gunakan deep clone,
             * jangan ubah board original.
             */
            var tempBoard =
                BoardHelper.Copy(board);

            var from =
                BoardHelper.GetTile(
                    tempBoard,
                    piece.CurrentLocation.Row,
                    piece.CurrentLocation.Column);

            var to =
                BoardHelper.GetTile(
                    tempBoard,
                    destination.Row,
                    destination.Column);

            if (from == null ||
                to == null ||
                from.Piece == null)
            {
                continue;
            }

            MovementHelper.MovePiece(
                tempBoard,
                from,
                to);

            /*
             * Setelah move:
             * apakah King sendiri terkena check?
             */
            if (!IsKingInCheck(
                    tempBoard,
                    piece.Color))
            {
                validMoves.Add(
                    destination);
            }
        }

        return validMoves;
    }


    // =========================================================
    // KING CHECK
    // =========================================================

    public static bool IsKingInCheck(
        Board board,
        PieceColor kingColor)
    {
        var kingLocation =
            kingColor == PieceColor.White
                ? board.WhiteKingLocation
                : board.BlackKingLocation;

        var opponentColor =
            PieceHelper.GetOpponentColor(
                kingColor);

        foreach (var tile in board.Tiles)
        {
            var attacker =
                tile.Piece;

            if (attacker == null)
                continue;

            if (attacker.Color !=
                opponentColor)
            {
                continue;
            }

            /*
             * Jangan panggil:
             *
             * attacker.GetValidMoves(board)
             *
             * di sini!
             *
             * Itu dapat menyebabkan recursive loop.
             */
            var attackedTiles =
                GenerateAttackSquares(
                    board,
                    attacker);

            if (attackedTiles.Any(
                    attacked =>
                        attacked.Row ==
                        kingLocation.Row &&
                        attacked.Column ==
                        kingLocation.Column))
            {
                return true;
            }
        }

        return false;
    }


    // =========================================================
    // ATTACK MAP
    // =========================================================

    private static IList<Tile> GenerateAttackSquares(
        Board board,
        IPiece piece)
    {
        return piece.Symbol switch
        {
            PieceType.Pawn =>
                GeneratePawnAttackSquares(
                    board,
                    piece),

            PieceType.Knight =>
                GenerateJumpAttackSquares(
                    board,
                    piece,
                    Knight.MoveTemplates),

            PieceType.Bishop =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    Bishop.MoveTemplates),

            PieceType.Rook =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    Rook.MoveTemplates),

            PieceType.Queen =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    
                    Queen.MoveTemplates),

            PieceType.King =>
                GenerateJumpAttackSquares(
                    board,
                    piece,
                    King.MoveTemplates),

            _ =>
                new List<Tile>()
        };
    }


    // =========================================================
    // PAWN ATTACK
    // =========================================================

    private static IList<Tile>
        GeneratePawnAttackSquares(
            Board board,
            IPiece pawn)
    {
        var attacks =
            new List<Tile>();

        int direction =
            GetPawnDirection(
                pawn.Color);

        int row =
            pawn.CurrentLocation.Row +
            direction;

        int leftColumn =
            pawn.CurrentLocation.Column - 1;

        int rightColumn =
            pawn.CurrentLocation.Column + 1;


        if (BoardHelper.IsInBounds(
                board,
                row,
                leftColumn))
        {
            var tile =
                BoardHelper.GetTile(
                    board,
                    row,
                    leftColumn);

            if (tile != null)
                attacks.Add(tile);
        }


        if (BoardHelper.IsInBounds(
                board,
                row,
                rightColumn))
        {
            var tile =
                BoardHelper.GetTile(
                    board,
                    row,
                    rightColumn);

            if (tile != null)
                attacks.Add(tile);
        }

        return attacks;
    }


    // =========================================================
    // KNIGHT / KING ATTACK MAP
    // =========================================================

    private static IList<Tile>
        GenerateJumpAttackSquares(
            Board board,
            IPiece piece,
            IEnumerable<int[]> directions)
    {
        var attacks =
            new List<Tile>();

        foreach (var direction in directions)
        {
            int row =
                piece.CurrentLocation.Row +
                direction[0];

            int col =
                piece.CurrentLocation.Column +
                direction[1];

            if (!BoardHelper.IsInBounds(board,row, col))
                continue;

            var tile =
                BoardHelper.GetTile(board,row, col);

            if (tile != null)
            {
                attacks.Add(tile);
            }
        }

        return attacks;
    }


    // =========================================================
    // ROOK / BISHOP / QUEEN ATTACK MAP
    // =========================================================

    private static IList<Tile>
        GenerateSlidingAttackSquares(
            Board board,
            IPiece piece,
            IEnumerable<int[]> directions)
    {
        var attacks =
            new List<Tile>();

        foreach (var direction in directions)
        {
            int row =
                piece.CurrentLocation.Row +
                direction[0];

            int col =
                piece.CurrentLocation.Column +
                direction[1];

            while (BoardHelper.IsInBounds(
                       board,
                       row,
                       col))
            {
                var tile =
                    BoardHelper.GetTile(board,row, col);

                if (tile == null)
                    break;

                /*
                 * Square ini diserang.
                 */
                attacks.Add(tile);

                /*
                 * Kalau ketemu piece,
                 * serangan tidak bisa melewati.
                 */
                if (tile.Piece != null)
                {
                    break;
                }

                row += direction[0];
                col += direction[1];
            }
        }

        return attacks;
    }


    // =========================================================
    // MOVE VALIDATION
    // =========================================================

    public static bool MoveIsValid(
        Board board,
        Tile from,
        Tile to)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        var piece =
            from.Piece;

        if (piece == null)
            return false;

        /*
         * Tidak boleh capture piece sendiri.
         */
        if (to.Piece != null &&
            to.Piece.Color == piece.Color)
        {
            return false;
        }

        var validMoves =
            GetValidMoves(
                board,
                piece);

        return validMoves.Any(
            move =>
                move.Row == to.Row &&
                move.Column == to.Column);
    }

    
    // =========================================================
    // CASTLING 
    // =========================================================
    private static IList<Tile> GenerateCastlingMoves(Board board, IPiece king)
    {
        var castlingMoves = new List<Tile>();
        
        // 1. king belum pernah bergerak
        if (king.HasMoved) 
            return castlingMoves;
        // 2. king tidak kondisi check
        if (IsKingInCheck(board, king.Color))
        {
            return castlingMoves;
        }

        int row = king.CurrentLocation.Row;
        
        // ==========================================
        // KINGSIDE CASTLING (Rokade Pendek - Sayap Raja)
        // Mengecek ke arah kanan (kolom 5, 6, dan Benteng di kolom 7)
        // ==========================================
        var kingsideRookTile = BoardHelper.GetTile(board, row, 7);
        var kingsideRook = kingsideRookTile?.Piece;

        if (kingsideRook != null && kingsideRook.Symbol == PieceType.Rook && !kingsideRook.HasMoved)
        {
            // 3. path harus kosong (kolom 5 dan 6) 
            var fTile = BoardHelper.GetTile(board, row, 5);
            var gTile = BoardHelper.GetTile(board, row, 6);
            if (fTile?.Piece == null && gTile?.Piece == null)
            {
                // 4 & 5 petak yang dilewati dan dituju tidak diserang
                if(!IsSquareAttacked(board,fTile!,king.Color) && 
                   !IsSquareAttacked(board,gTile!,king.Color))
                    castlingMoves.Add(gTile!);
            }
        }
        // ==========================================
        // QUEENSIDE CASTLING (Rokade Panjang - Sayap Ratu)
        // Mengecek ke arah kiri (kolom 1, 2, 3, dan Benteng di kolom 0)
        // ==========================================
        var queensideRookTile = BoardHelper.GetTile(board, row, 0);
        var queensideRook = queensideRookTile?.Piece;

        if (queensideRook != null && queensideRook.Symbol == PieceType.Rook && !queensideRook.HasMoved)
        {
            var bTile = BoardHelper.GetTile(board, row, 1);
            var cTile = BoardHelper.GetTile(board, row, 2);
            var dTile = BoardHelper.GetTile(board, row, 3);
             // 3. path harus kosong (kolom 1,2, dan 3)
            if (bTile?.Piece == null && cTile?.Piece == null && dTile?.Piece == null)
            {
                // Syarat 4 & 5: Petak yang dilewati (c dan d) dan dituju (c) tidak boleh diserang.
                // Catatan: Petak b tidak dilewati oleh Raja, jadi tidak perlu dicek serangannya.
                if (!IsSquareAttacked(board, dTile!, king.Color) && 
                    !IsSquareAttacked(board, cTile!, king.Color))
                {
                    castlingMoves.Add(cTile!); // Tambahkan petak c (kolom 2) sebagai langkah valid
                }
            }
        }

        return castlingMoves;
    }
    

    // check specific tile 
    public static bool IsSquareAttacked(Board board, Tile targetTile, PieceColor defendingColor)
    {
        var opponentColor = PieceHelper.GetOpponentColor(defendingColor);

        foreach (var tile in board.Tiles)
        {
            var attacker = tile.Piece;
            if (attacker == null || attacker.Color != opponentColor)
                continue;

            // Generate serangan dari lawan
            var attackedTiles = GenerateAttackSquares(board, attacker);

            // Jika targetTile ada di dalam daftar serangan, berarti petak tersebut tidak aman
            if (attackedTiles.Any(attacked => attacked.Row == targetTile.Row && attacked.Column == targetTile.Column))
            {
                return true;
            }
        }
        return false;
    }
 
}