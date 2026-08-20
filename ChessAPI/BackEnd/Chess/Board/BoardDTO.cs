namespace ChessAPI.BackEnd.Chess.Board;


public class BoardDto
{
    public int Size { get; set; }

    public List<TileDto> Tiles { get; set; } = new();

    public LocationDto WhiteKingLocation { get; set; } = new();

    public LocationDto BlackKingLocation { get; set; } = new();

    public string? KingInCheck { get; set; }


    public static BoardDto FromBoard(Board board)
    {
        var dto = new BoardDto
        {
            Size = board.Size,

            WhiteKingLocation = new LocationDto
            {
                Row = board.WhiteKingLocation.Row,
                Column = board.WhiteKingLocation.Columns
            },

            BlackKingLocation = new LocationDto
            {
                Row = board.BlackKingLocation.Row,
                Column = board.BlackKingLocation.Columns
            },

            KingInCheck = board.KingInCheck?.ToString()
        };

        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                var tile = board.GetTile(row, col);

                if (tile == null)
                    continue;

                dto.Tiles.Add(new TileDto
                {
                    Row = tile.Row,
                    Column = tile.Column,
                    IsEmpty = tile.IsEmptySpace,

                    Piece = tile.Piece == null
                        ? null
                        : new PieceDto
                        {
                            Type = tile.Piece.GetType().Name,
                            Color = tile.Piece.Color.ToString(),

                            CurrentLocation = new LocationDto
                            {
                                Row = tile.Piece.CurrentLocation.Row,
                                Column = tile.Piece.CurrentLocation.Columns
                            }
                        }
                });
            }
        }

        return dto;
    }


    public class TileDto
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsEmpty { get; set; }

        public PieceDto? Piece { get; set; }
    }


    public class PieceDto
    {
        public string Type { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public LocationDto CurrentLocation { get; set; } = new();
    }


    public class LocationDto
    {
        public int Row { get; set; }

        public int Column { get; set; }
    }
}