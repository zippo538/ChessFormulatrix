using ChessAPI.BackEnd.Chess.Board;
using ChessAPI.BackEnd.Chess.Enums;
using ChessAPI.BackEnd.Chess.Pieces;

namespace ChessAPI.BackEnd.Chess.Models;

internal static class Movement
{
   


    // =========================================================
    // PUBLIC ENTRY POINT
    // =========================================================

    internal static IList<Tile> GetValidMoves(
        Board.Board board,
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
        Board.Board board,
        IPiece piece)
    {
        return piece.PieceType switch
        {
            PieceType.Pawn =>
                GeneratePawnMoves(
                    board,
                    piece),

            PieceType.Knight =>
                GenerateJumpMoves(
                    board,
                    piece,
                    KnightDirections),

            PieceType.Bishop =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    BishopDirections),

            PieceType.Rook =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    RookDirections),

            PieceType.Queen =>
                GenerateSlidingMoves(
                    board,
                    piece,
                    QueenDirections),

            PieceType.King =>
                GenerateKingMoves(
                    board,
                    piece),

            _ =>
                throw new InvalidOperationException(
                    $"Unsupported piece type {piece.PieceType}")
        };
    }


    // =========================================================
    // ROOK / BISHOP / QUEEN
    // =========================================================

    private static IList<Tile> GenerateSlidingMoves(
        Board.Board board,
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
                piece.CurrentLocation.Columns +
                colStep;

            /*
             * Terus bergerak sampai:
             *
             * - keluar board
             * - ketemu piece sendiri
             * - capture lawan
             */
            while (board.IsInBounds(row, col))
            {
                var tile =
                    board.GetTile(row, col);

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
        Board.Board board,
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
                piece.CurrentLocation.Columns +
                direction[1];

            if (!board.IsInBounds(row, col))
                continue;

            var tile =
                board.GetTile(row, col);

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
        Board.Board board,
        IPiece king)
    {
        var moves =
            GenerateJumpMoves(
                board,
                king,
                KingDirections);

        /*
         * Castling belum dimasukkan di sini.
         *
         * Bisa ditambahkan nanti:
         *
         * - king belum pernah bergerak
         * - rook belum pernah bergerak
         * - path kosong
         * - king tidak sedang check
         * - king tidak melewati attacked square
         */

        return moves;
    }


    // =========================================================
    // PAWN
    // =========================================================

    private static IList<Tile> GeneratePawnMoves(
        Board.Board board,
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
            pawn.CurrentLocation.Columns;

        if (board.IsInBounds(
                oneStepRow,
                column))
        {
            var oneStep =
                board.GetTile(
                    oneStepRow,
                    column);

            if (oneStep?.Piece == null)
            {
                validMoves.Add(oneStep);


                // ---------------------------------
                // 2 STEPS FROM INITIAL POSITION
                // ---------------------------------

                if (pawn.CurrentLocation.Row ==
                    startRow)
                {
                    int twoStepRow =
                        pawn.CurrentLocation.Row +
                        direction * 2;

                    if (board.IsInBounds(
                            twoStepRow,
                            column))
                    {
                        var twoStep =
                            board.GetTile(
                                twoStepRow,
                                column);

                        if (twoStep?.Piece == null)
                        {
                            validMoves.Add(twoStep);
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


        /*
         * En passant belum dimasukkan.
         * Membutuhkan previous Move.
         */

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
            pawn.CurrentLocation.Columns +
            columnOffset;

        if (!board.IsInBounds(row, col))
            return;

        var tile =
            board.GetTile(row, col);

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
        Board.Board board,
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
                board.Copy();

            var from =
                tempBoard.GetTile(
                    piece.CurrentLocation.Row,
                    piece.CurrentLocation.Columns);

            var to =
                tempBoard.GetTile(
                    destination.Row,
                    destination.Column);

            if (from == null ||
                to == null ||
                from.Piece == null)
            {
                continue;
            }

            tempBoard.MovePiece(
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

    internal static bool IsKingInCheck(
        Board.Board board,
        PieceColor kingColor)
    {
        var kingLocation =
            kingColor == PieceColor.White
                ? board.WhiteKingLocation
                : board.BlackKingLocation;

        var opponentColor =
            GetOpponentColor(
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
                        kingLocation.Columns))
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
        return piece.PieceType switch
        {
            PieceType.Pawn =>
                GeneratePawnAttackSquares(
                    board,
                    piece),

            PieceType.Knight =>
                GenerateJumpAttackSquares(
                    board,
                    piece,
                    KnightDirections),

            PieceType.Bishop =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    BishopDirections),

            PieceType.Rook =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    RookDirections),

            PieceType.Queen =>
                GenerateSlidingAttackSquares(
                    board,
                    piece,
                    QueenDirections),

            PieceType.King =>
                GenerateJumpAttackSquares(
                    board,
                    piece,
                    KingDirections),

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
            pawn.CurrentLocation.Columns - 1;

        int rightColumn =
            pawn.CurrentLocation.Columns + 1;


        if (board.IsInBounds(
                row,
                leftColumn))
        {
            var tile =
                board.GetTile(
                    row,
                    leftColumn);

            if (tile != null)
                attacks.Add(tile);
        }


        if (board.IsInBounds(
                row,
                rightColumn))
        {
            var tile =
                board.GetTile(
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
            Board.Board board,
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
                piece.CurrentLocation.Columns +
                direction[1];

            if (!board.IsInBounds(row, col))
                continue;

            var tile =
                board.GetTile(row, col);

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
            Board.Board board,
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
                piece.CurrentLocation.Columns +
                direction[1];

            while (board.IsInBounds(
                       row,
                       col))
            {
                var tile =
                    board.GetTile(row, col);

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

    internal static bool MoveIsValid(
        Board.Board board,
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
    // COLOR HELPER
    // =========================================================

    private static PieceColor GetOpponentColor(
        PieceColor color)
    {
        return color == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;
    }
}