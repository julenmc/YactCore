using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Cyclist;

namespace Yact.Application.Mapping;

public class CyclistMapper : Profile
{
    public CyclistMapper()
    {
        CreateMap<Cyclist, CyclistDto>();
        CreateMap<CyclistDto, Cyclist>();
    }
}
