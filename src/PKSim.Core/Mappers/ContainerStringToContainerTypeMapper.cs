using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IContainerStringToContainerTypeMapper : IMapper<string, PKSimContainerType>
   {
   }

   public class ContainerStringToContainerTypeMapper : IContainerStringToContainerTypeMapper
   {
      public PKSimContainerType MapFrom(string containerType)
      {
         if (containerType.ToUpper().Equals("SIMULATION"))
            return PKSimContainerType.Root;

         return EnumHelper.ParseValue<PKSimContainerType>(containerType);
      }
   }
}