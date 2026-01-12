//using Yact.Domain.ValueObjects.Cyclist;
//using Yact.Infrastructure.Persistence.Models;
//using Entities = Yact.Domain.Entities;

//namespace Yact.Infrastructure.Persistence.Mappers;

//internal static class CyclistMapper
//{
//    internal static Entities.Cyclist ToDomain(this Cyclist model)
//    {
//        var fitnessList = model.Fitnesss?
//            .Select(f => f.ToDomain())
//            .ToList();
        
//        return Entities.Cyclist.Load(
//            CyclistId.From(model.Id),
//            model.Name,
//            model.LastName,
//            model.BirthDate
//        );
//    }

//    internal static Cyclist ToModel(this Entities.Cyclist entity)
//    {
//        return new Cyclist()
//        {
//            Id = entity.Id.Value,
//            Name = entity.Name != null ? entity.Name : "Unknown",
//            LastName = entity.LastName != null ? entity.LastName : "Unknown",
//            BirthDate = entity.BirthDate,
//            Fitnesss = entity.FitnessHistory?
//                .Select(f => f.ToModel())
//                .ToList()
//        };
//    }
//}
