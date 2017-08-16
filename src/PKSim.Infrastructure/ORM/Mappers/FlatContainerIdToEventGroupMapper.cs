using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToEventGroupMapper : FlatContainerIdToContainerMapperBase<IEventGroupBuilder>, IFlatContainerIdToContainerMapperSpecification
   {
      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var eventGroupBuilder = MapCommonPropertiesFrom(flatContainerId);

         return eventGroupBuilder;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.EventGroup;
      }
   }
}