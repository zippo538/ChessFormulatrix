using AutoMapper;
using ChessAPI.Models;
using ChessAPI.DTO;
using ChessAPI.Helpers;
using ChessAPI.Models.Interfaces;

namespace ChessAPI.AutoMapper;

public class ChessMappingProfile : Profile
{
    public ChessMappingProfile()
    {
        // BoardLocation -> LocationDto
        CreateMap<BoardLocation, BoardDto.LocationDto>()
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.Row))
            .ForMember(dest => dest.Column, opt => opt.MapFrom(src => src.Column));

        // Piece -> PieceDto (record)
        CreateMap<Piece, PieceDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.CurrentLocation.Row))
            .ForMember(dest => dest.Column, opt => opt.MapFrom(src => src.CurrentLocation.Column));

        // IPiece -> PieceDto (record)
        CreateMap<IPiece, PieceDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.CurrentLocation.Row))
            .ForMember(dest => dest.Column, opt => opt.MapFrom(src => src.CurrentLocation.Column));

        // Tile -> TileDto
        CreateMap<Tile, BoardDto.TileDto>()
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.Row))
            .ForMember(dest => dest.Column, opt => opt.MapFrom(src => src.Column))
            .ForMember(dest => dest.IsEmpty, opt => opt.MapFrom(src => src.IsEmptySpace))
            .ForMember(dest => dest.Piece, opt => opt.MapFrom(src => src.Piece));

        // Move -> MoveDTO
        CreateMap<Move, MoveDTO>()
            .ForMember(dest => dest.FromRow, opt => opt.MapFrom(src => src.From.Row))
            .ForMember(dest => dest.FromColumn, opt => opt.MapFrom(src => src.From.Column))
            .ForMember(dest => dest.ToRow, opt => opt.MapFrom(src => src.To.Row))
            .ForMember(dest => dest.ToColumn, opt => opt.MapFrom(src => src.To.Column));

        // Board -> BoardDto
        CreateMap<Board, BoardDto>()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
            .ForMember(dest => dest.Tiles, opt => opt.MapFrom(src =>
                src.Tiles.Cast<Tile>().ToList()))
            .ForMember(dest => dest.WhiteKingLocation, opt => opt.MapFrom(src => src.WhiteKingLocation))
            .ForMember(dest => dest.BlackKingLocation, opt => opt.MapFrom(src => src.BlackKingLocation))
            .ForMember(dest => dest.KingInCheck, opt => opt.Ignore());
    }
}
