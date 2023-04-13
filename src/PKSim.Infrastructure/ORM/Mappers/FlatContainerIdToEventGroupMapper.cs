using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToEventGroupMapper : FlatContainerIdToContainerMapperBase<EventGroupBuilder>, IFlatContainerIdToContainerMapperSpecification
   {
      public FlatContainerIdToEventGroupMapper(IObjectBaseFactory objectBaseFactory, IFlatContainerRepository flatContainerRepository, IFlatContainerTagRepository flatContainerTagRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
      }

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