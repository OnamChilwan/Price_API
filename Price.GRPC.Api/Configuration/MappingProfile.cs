using AutoMapper;
using Price.Application.DTOs;

namespace Price.GRPC.Api.Configuration;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ItemPrice, ItemPriceDto>()
            .ReverseMap()
            .AddTransform<string>(str => string.IsNullOrWhiteSpace(str) ? string.Empty : str);
        CreateMap<Option, OptionDto>().ReverseMap();
        CreateMap<Pricing, PriceDto>().ReverseMap();
        CreateMap<History, PriceHistoryDto>().ReverseMap();
    }
}