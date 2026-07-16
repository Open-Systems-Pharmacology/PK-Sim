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
      private readonly IParameterToParameterDTOInContainerMapper<SchemaItemDTO> _parameterInContainerMapper;
      private readonly IParameterToParameterDTOMapper _parameterMapper;

      public SchemaItemToSchemaItemDTOMapper(IParameterToParameterDTOInContainerMapper<SchemaItemDTO> parameterInContainerMapper, IParameterToParameterDTOMapper parameterMapper)
      {
         _parameterInContainerMapper = parameterInContainerMapper;
         _parameterMapper = parameterMapper;
      }

      public SchemaItemDTO MapFrom(SchemaItem schemaItem)
      {
         var schemaItemDTO = new SchemaItemDTO(schemaItem);
         schemaItemDTO.DoseParameter = _parameterInContainerMapper.MapFrom(schemaItem.Dose, schemaItemDTO, x => x.Dose, x => x.DoseParameter);
         schemaItemDTO.StartTimeParameter = _parameterInContainerMapper.MapFrom(schemaItem.StartTime, schemaItemDTO, x => x.StartTime, x => x.StartTimeParameter);

         //the infusion time only exists for infusion application types
         if (schemaItem.InfusionTime != null)
            schemaItemDTO.InfusionTimeParameter = _parameterMapper.MapFrom(schemaItem.InfusionTime);

         return schemaItemDTO;
      }
   }
}