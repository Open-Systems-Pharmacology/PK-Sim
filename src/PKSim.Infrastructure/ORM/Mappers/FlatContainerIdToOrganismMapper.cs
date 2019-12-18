using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToOrganismMapper : FlatContainerIdToContainerMapperBase<Organism>, IFlatContainerIdToContainerMapperSpecification
   {
      public FlatContainerIdToOrganismMapper(IObjectBaseFactory objectBaseFactory, IFlatContainerRepository flatContainerRepository, IFlatContainerTagRepository flatContainerTagRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
      }

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