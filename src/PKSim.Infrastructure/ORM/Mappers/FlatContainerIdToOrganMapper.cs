using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public class FlatContainerIdToOrganMapper : FlatContainerIdToContainerMapperBase<Organ>, IFlatContainerIdToContainerMapperSpecification
   {
      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var organ = MapCommonPropertiesFrom(flatContainerId);
         organ.OrganType = organTypeFromName(organ.Name);
         return organ;
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.Organ;
      }

      /// <summary>
      ///    Return organ type from organ name
      /// </summary>
      /// <param name="organName">Name of organ</param>
      private static OrganType organTypeFromName(string organName)
      {
         return EnumHelper.ParseValue<OrganType>(organName);
      }
   }
}