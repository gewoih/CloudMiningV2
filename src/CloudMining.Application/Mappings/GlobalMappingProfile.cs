using AutoMapper;
using CloudMining.Application.DTO.Payments;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Mappings;

public sealed class GlobalMappingProfile : Profile
{
    public GlobalMappingProfile()
    {
        CreateMap<ShareablePayment, PaymentDto>()
            .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
            .ForMember(dest => dest.Caption, act => act.MapFrom(src => src.Caption))
            .ForMember(dest => dest.Date, act => act.MapFrom(src => src.Date))
            .ForMember(dest => dest.Amount, act => act.MapFrom(src => src.Amount))
            .ForMember(dest => dest.IsCompleted, act => act.MapFrom(src => src.IsCompleted));
    }
}