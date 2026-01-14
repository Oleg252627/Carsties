using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Auction, AuctionDto>()
            .ForMember(d => d.Make, o => o.MapFrom(s => s.Item.Make))
            .ForMember(d => d.Model, o => o.MapFrom(s => s.Item.Model))
            .ForMember(d => d.Year, o => o.MapFrom(s => s.Item.Year))
            .ForMember(d => d.Color, o => o.MapFrom(s => s.Item.Color))
            .ForMember(d => d.Mileage, o => o.MapFrom(s => s.Item.Mileage))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Item.ImageUrl));
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));
        CreateMap<CreateAuctionDto, Item>();
    }
}