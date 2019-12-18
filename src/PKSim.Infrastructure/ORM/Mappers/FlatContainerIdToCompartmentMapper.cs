using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToCompartmentMapper : FlatContainerIdToContainerMapperBase<Compartment>, IFlatContainerIdToContainerMapperSpecification
   {
      public FlatContainerIdToCompartmentMapper(IObjectBaseFactory objectBaseFactory, IFlatContainerRepository flatContainerRepository, IFlatContainerTagRepository flatContainerTagRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
      }

      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var compartment = MapCommonPropertiesFrom(flatContainerId);
         compartment.Visible = FlatContainer.Visible;
         return compartment;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.Compartment;
      }
   }
}