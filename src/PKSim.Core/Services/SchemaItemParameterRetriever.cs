using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ISchemaItemParameterRetriever
   {
      IEnumerable<IParameter> AllParametersFor(ApplicationType appplicationType);
      IEnumerable<IParameter> AllDynamicParametersFor(ApplicationType appplicationType);
      IEnumerable<IParameter> AllDynamicParametersFor(ISchemaItem schemaItem);
      IEnumerable<IParameter> AllStaticParameters(ISchemaItem schemaItem);
   }

   public class SchemaItemParameterRetriever : ISchemaItemParameterRetriever
   {
      private readonly ISchemaItemRepository _schemaItemRepository;
      private readonly ICloner _cloner;

      public SchemaItemParameterRetriever(ISchemaItemRepository schemaItemRepository, ICloner cloner)
      {
         _schemaItemRepository = schemaItemRepository;
         _cloner = cloner;
      }

      public IEnumerable<IParameter> AllParametersFor(ApplicationType appplicationType)
      {
         return (from schemaItemParam in _schemaItemRepository.SchemaItemBy(appplicationType).AllParameters()
                 select _cloner.Clone(schemaItemParam));
      }

      public IEnumerable<IParameter> AllDynamicParametersFor(ApplicationType appplicationType)
      {
         return AllParametersFor(appplicationType).Where(isDynamicSchemaItemParameter);
      }

      public IEnumerable<IParameter> AllDynamicParametersFor(ISchemaItem schemaItem)
      {
         return schemaItem.AllParameters(isDynamicSchemaItemParameter);
      }

      public IEnumerable<IParameter> AllStaticParameters(ISchemaItem schemaItem)
      {
         return schemaItem.AllParameters(isStaticSchemaItemParameter);
      }

      private bool isDynamicSchemaItemParameter(IParameter parameter)
      {
         if (parameter == null) return false;
         if (string.Equals(parameter.Name, Constants.Parameters.START_TIME)) return false;
         if (string.Equals(parameter.Name, CoreConstants.Parameter.INPUT_DOSE)) return false;
         if (string.Equals(parameter.Name, Constants.Parameters.END_TIME)) return false;
         return true;
      }

      private bool isStaticSchemaItemParameter(IParameter parameter)
      {
         return !isDynamicSchemaItemParameter(parameter);
      }
   }
}