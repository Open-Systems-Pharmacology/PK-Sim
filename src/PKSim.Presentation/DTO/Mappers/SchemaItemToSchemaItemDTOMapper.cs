using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISchemaItemToSchemaItemDTOMapper : IMapper<SchemaItem, SchemaItemDTO>
   {
   }

   public class SchemaItemToSchemaItemDTOMapper : ISchemaItemToSchemaItemDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<SchemaItemDTO> _parameterDTOMapper;

      public SchemaItemToSchemaItemDTOMapper(IParameterToParameterDTOInContainerMapper<SchemaItemDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public SchemaItemDTO MapFrom(SchemaItem schemaItem)
      {
         var schemaItemDTO = new SchemaItemDTO(schemaItem);
         schemaItemDTO.DoseParameter = _parameterDTOMapper.MapFrom(schemaItem.Dose, schemaItemDTO, x => x.Dose, x => x.DoseParameter);
         schemaItemDTO.StartTimeParameter = _parameterDTOMapper.MapFrom(schemaItem.StartTime, schemaItemDTO, x => x.StartTime, x => x.StartTimeParameter);
         return schemaItemDTO;
      }
   }
}