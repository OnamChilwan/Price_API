using AutoMapper;
using Price.Api.Models.Responses;
using Price.Application.DTOs;

namespace Price.Api.Configuration;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ItemPrice, ItemPriceDto>().ReverseMap();
        CreateMap<Option, OptionDto>().ReverseMap();
        CreateMap<Models.Responses.Price, PriceDto>().ReverseMap();
        CreateMap<PriceHistory, PriceHistoryDto>().ReverseMap();
    }
}