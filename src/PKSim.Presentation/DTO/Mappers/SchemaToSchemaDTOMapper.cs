using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISchemaToSchemaDTOMapper : IMapper<Schema, SchemaDTO>
   {
   }

   public class SchemaToSchemaDTOMapper : ISchemaToSchemaDTOMapper
   {
      private readonly ISchemaItemToSchemaItemDTOMapper _schemaItemDtoMapper;
      private readonly IParameterToParameterDTOInContainerMapper<SchemaDTO> _parameterDTOMapper;

      public SchemaToSchemaDTOMapper(ISchemaItemToSchemaItemDTOMapper schemaItemDTOMapper, IParameterToParameterDTOInContainerMapper<SchemaDTO> parameterDTOMapper)
      {
         _schemaItemDtoMapper = schemaItemDTOMapper;
         _parameterDTOMapper = parameterDTOMapper;
      }

      public SchemaDTO MapFrom(Schema schema)
      {
         var schemaDTO = new SchemaDTO(schema, schema.TimeBetweenRepetitions.Dimension);
         schemaDTO.TimeBetweenRepetitionsParameter = _parameterDTOMapper.MapFrom(schema.TimeBetweenRepetitions, schemaDTO, x => x.TimeBetweenRepetitions, x => x.TimeBetweenRepetitionsParameter);
         schemaDTO.StartTimeParameter = _parameterDTOMapper.MapFrom(schema.StartTime, schemaDTO, x => x.StartTime, x => x.StartTimeParameter);
         schemaDTO.NumberOfRepetitionsParameter = _parameterDTOMapper.MapFrom(schema.NumberOfRepetitions, schemaDTO, x => x.NumberOfRepetitions, x => x.NumberOfRepetitionsParameter);
         schema.SchemaItems.OrderBy(x => x.Name).Each(item => schemaDTO.AddSchemaItem(_schemaItemDtoMapper.MapFrom(item)));
         return schemaDTO;
      }
   }
}