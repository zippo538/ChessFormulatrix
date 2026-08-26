using ChessAPI.Models.Enums;
using ChessAPI.Services;

    namespace ChessAPI.Models.Pieces;

    public class Rook : Piece
    {
        public Rook() 
        {
            _symbol = PieceType.Rook;
        }
        public Rook(PieceColor player, int row, int col) : base(player, row, col)
        {
            _symbol = PieceType.Rook;
        }

        public readonly static List<int[]> MoveTemplates = new()
        {
            new[] { -1, 0 }, 
            new[] { 1, 0 },  
            new[] { 0, -1 }, 
            new[] { 0, 1 }
        };

        public override IList<Tile> GetValidMoves(Board board)
        {
            return MovementService.GetValidMoves(board,this);
        }

        public override Piece Clone()
        {
            return new Rook(this.Color, this.CurrentLocation.Row, this.CurrentLocation.Column)
            {
                HasMoved = this.HasMoved
            };
        }    
    }