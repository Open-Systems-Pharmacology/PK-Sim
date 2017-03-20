using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatRelativeObjectPathToObjectPathMapper : IMapper<long, IObjectPath>
   {
   }

   public class FlatRelativeObjectPathToObjectPathMapper : IFlatRelativeObjectPathToObjectPathMapper
   {
      private readonly IFlatRelativeObjectPathRepository _flatObjectPathRepo;
      private readonly IObjectPathFactory _objectPathFactory;

      public FlatRelativeObjectPathToObjectPathMapper(IFlatRelativeObjectPathRepository flatRelativeObjectPathRepo,IObjectPathFactory objectPathFactory)
      {
         _flatObjectPathRepo = flatRelativeObjectPathRepo;
         _objectPathFactory = objectPathFactory;
      }

      public IObjectPath MapFrom(long flatObjectPathId)
      {
         FlatRelativeObjectPath flatObjectPath = _flatObjectPathRepo.FlatRelativeObjectPathFor(flatObjectPathId);
         
         var relativePath = new List<string>();

         //first entry
         if (flatObjectPath.ContainerName != CoreConstants.ORM.ContainerMe)
            relativePath.Add(flatObjectPath.ContainerName);

         while (flatObjectPath.PathId != flatObjectPath.ParentPathId)
         {
            flatObjectPath = _flatObjectPathRepo.FlatRelativeObjectPathFor(flatObjectPath.ParentPathId);

            if (flatObjectPath.ContainerName != CoreConstants.ORM.ContainerMe)
               relativePath.Add(flatObjectPath.ContainerName);
         }
         relativePath.Reverse();
         return _objectPathFactory.CreateObjectPathFrom(relativePath.ToArray());
      }
   }
}