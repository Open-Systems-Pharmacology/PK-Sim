using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToOrganismMapper : FlatContainerIdToContainerMapperBase<Organism>, IFlatContainerIdToContainerMapperSpecification
   {
      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var organism = base.MapCommonPropertiesFrom(flatContainerId);

         return organism;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.Organism;
      }
   }
}