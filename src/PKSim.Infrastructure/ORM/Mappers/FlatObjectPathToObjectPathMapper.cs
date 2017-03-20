using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatObjectPathToObjectPathMapper : IMapper<FlatObjectPath, IObjectPath>
   {
   }

   public class FlatObjectPathToObjectPathMapper : IFlatObjectPathToObjectPathMapper
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatRelativeObjectPathToObjectPathMapper _relativeObjectPathMapper;

      public FlatObjectPathToObjectPathMapper(IFlatContainerRepository flatContainerRepository, IFlatRelativeObjectPathToObjectPathMapper relativeObjectPathMapper)
      {
         _flatContainerRepository = flatContainerRepository;
         _relativeObjectPathMapper = relativeObjectPathMapper;
      }

      public IObjectPath MapFrom(FlatObjectPath flatObjectPath)
      {
         IObjectPath objectPath;

         if (flatObjectPath.IsAbsolutePath)
         {
            objectPath = _flatContainerRepository.ContainerPathFrom(flatObjectPath.ContainerId);
         }
         else
         {
            objectPath = _relativeObjectPathMapper.MapFrom(flatObjectPath.RelativeObjectPathId);
         }

         //for molecule amounts, object name used is just a dummy in the database,
         //so omit it
         if (!flatObjectPath.UseAmount)
         {
            objectPath.Add(adjustRateObjectName(flatObjectPath.RateObjectName));
         }

         return objectPath;
      }

      private static string adjustRateObjectName(string rateObjectName)
      {
         // replace 'DRUG' with reference to the concentration parameter

         if (rateObjectName == CoreConstants.Molecule.Drug)
            return CoreConstants.Parameter.CONCENTRATION;

         return rateObjectName;
      }
   }
}