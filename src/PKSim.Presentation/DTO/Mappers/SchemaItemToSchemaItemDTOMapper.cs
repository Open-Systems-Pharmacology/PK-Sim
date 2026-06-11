using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
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

         //the infusion time only exists for infusion application types. It is a read-only value source for the
         //protocol chart (it is edited through the dynamic parameter grid), so it is not bound for editing here.
         if (schemaItem.InfusionTime != null)
            schemaItemDTO.InfusionTimeParameter = new ParameterDTO(schemaItem.InfusionTime);

         return schemaItemDTO;
      }
   }
}