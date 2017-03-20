using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToGeneralContainerMapper : FlatContainerIdToContainerMapperBase<IContainer>, IFlatContainerIdToContainerMapperSpecification
   {
      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var container = MapCommonPropertiesFrom(flatContainerId);

         if(string.Equals(container.Name,Constants.MOLECULE_PROPERTIES))
            container.ContainerType = ContainerType.Molecule;

         return container;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.General;
      }
   }

}
