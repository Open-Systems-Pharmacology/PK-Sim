using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.Services
{
   public class FullPathDisplayResolver : IFullPathDisplayResolver
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;

      public FullPathDisplayResolver(IRepresentationInfoRepository representationInfoRepository, IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper)
      {
         _representationInfoRepository = representationInfoRepository;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
      }

      public string FullPathFor(IObjectBase objectBase, bool addSimulationName = false)
      {
         switch (objectBase)
         {
            case IQuantity quantity:
               return _quantityDisplayPathMapper.DisplayPathAsStringFor(quantity, addSimulationName);
            case ParameterAlternativeGroup parameterAlternativeGroup:
               return displayFor(parameterAlternativeGroup);
            case IEntity entity:
               return entity.EntityPath();
         }

         return displayFor(objectBase);
      }

      private string displayFor(IObjectBase objectBase) => _representationInfoRepository.DisplayNameFor(objectBase);
   }
}