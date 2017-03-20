using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IProtocolToProtocolPropertiesDTOMapper : IMapper<Protocol, ObjectBaseDTO>
   {
   }

   public class ProtocolToProtocolPropertiesDTOMapper : IProtocolToProtocolPropertiesDTOMapper
   {
      private readonly IObjectBaseDTOFactory _objectBaseDTOFactory;

      public ProtocolToProtocolPropertiesDTOMapper(IObjectBaseDTOFactory objectBaseDTOFactory)
      {
         _objectBaseDTOFactory = objectBaseDTOFactory;
      }

      public ObjectBaseDTO MapFrom(Protocol protocol)
      {
         var objectBaseDTO = _objectBaseDTOFactory.CreateFor<Protocol>();
         objectBaseDTO.Name = protocol.Name;
         objectBaseDTO.Description = protocol.Description;
         return objectBaseDTO;
      }
   }
}