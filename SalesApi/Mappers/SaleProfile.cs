using AutoMapper;
using SalesApi.Models.DTOs;
using SalesApi.Models.Entities;

namespace SalesApi.Mappers;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleProcessingData, Sale>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.DiscountApplied, opt => opt.MapFrom(src => src.DiscountPercentage));

        CreateMap<Sale, SaleResponseDto>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(_ => true));
    }
}
