//using AutoMapper;
//using Yact.Application.Responses;
//using Yact.Domain.Entities;

//namespace Yact.Application.Mapping;

//public class ActivityMapper : Profile
//{
//    public ActivityMapper ()
//    {
//        CreateMap<Activity, ActivityResponse>()
//            .ForMember(dest => dest.Id,
//                opt => opt.MapFrom(src => src.Id.Value))
//            .ForMember(dest => dest.CyclistId,
//                opt => opt.MapFrom(src => src.CyclistId.Value))
//            .ForMember(dest => dest.Name,
//                opt => opt.MapFrom(src => src.Summary.Name))
//            .ForMember(dest => dest.Description,
//                opt => opt.MapFrom(src => src.Summary.Description))
//            .ForMember(dest => dest.Path,
//                opt => opt.MapFrom(src => src.Path.Value))
//            .ForMember(dest => dest.StartDate,
//                opt => opt.MapFrom(src => src.Summary.StartDate))
//            .ForMember(dest => dest.EndDate,
//                opt => opt.MapFrom(src => src.Summary.EndDate))
//            .ForMember(dest => dest.DistanceMeters,
//                opt => opt.MapFrom(src => src.Summary.DistanceMeters))
//            .ForMember(dest => dest.ElevationMeters,
//                opt => opt.MapFrom(src => src.Summary.ElevationMeters))
//            .ForMember(dest => dest.Type,
//                opt => opt.MapFrom(src => src.Summary.Type))
//            .ForMember(dest => dest.CreateDate,
//                opt => opt.MapFrom(src => src.Summary.CreateDate))
//            .ForMember(dest => dest.UpdateDate,
//                opt => opt.MapFrom(src => src.Summary.UpdateDate));
//    }
//}
