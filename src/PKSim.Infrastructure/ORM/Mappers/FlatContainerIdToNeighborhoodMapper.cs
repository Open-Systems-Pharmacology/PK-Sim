using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToNeighborhoodMapper : FlatContainerIdToContainerMapperBase<IPKSimNeighborhood>, IFlatContainerIdToContainerMapperSpecification
   {
      //for the moment, map neighborhoods as compartments
      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var neighborhood = MapCommonPropertiesFrom(flatContainerId);

         neighborhood.Visible = FlatContainer.Visible;

         return neighborhood;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.Neighborhood;
      }
   }
}