using ChessAPI.DTO;

namespace ChessAPI.DTO;


public record BoardDto
{
    public int Size { get; set; }

    public List<TileDto> Tiles { get; set; } = new();

    public LocationDto WhiteKingLocation { get; set; } = new();

    public LocationDto BlackKingLocation { get; set; } = new();

    public string? KingInCheck { get; set; }


    public class TileDto
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsEmpty { get; set; }

        public PieceDto? Piece { get; set; }
    }


    public class LocationDto
    {
        public int Row { get; set; }

        public int Column { get; set; }
    }
}