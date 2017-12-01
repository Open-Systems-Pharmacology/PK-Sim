using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToOrganMapper : FlatContainerIdToContainerMapperBase<Organ>, IFlatContainerIdToContainerMapperSpecification
   {
      private readonly IOrganTypeRepository _organTypeRepository;

      public FlatContainerIdToOrganMapper(IObjectBaseFactory objectBaseFactory,
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerTagRepository flatContainerTagRepository,
         IOrganTypeRepository organTypeRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
         _organTypeRepository = organTypeRepository;
      }

      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var organ = MapCommonPropertiesFrom(flatContainerId);
         organ.OrganType = organTypeFromName(organ.Name);
         return organ;
      }

      public bool IsSatisfiedBy(PKSimContainerType containerType) => containerType == PKSimContainerType.Organ;

      private OrganType organTypeFromName(string organName) => _organTypeRepository.OrganTypeFor(organName);
   }
}