using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;

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
         var quantity = objectBase as IQuantity;
         if (quantity != null)
            return _quantityDisplayPathMapper.DisplayPathAsStringFor(quantity, addSimulationName);

         var entity = objectBase as IEntity;
         if (entity != null)
            return entity.EntityPath();

         return _representationInfoRepository.DisplayNameFor(objectBase);
      }
   }
}