using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Extensions
{
   public static class SimpleProtocolExtensions
   {
      public static bool ParameterShouldBeExported(this ISchemaItem schemaItem, IParameter parameter)
      {
         if (parameter.IsNamed(Constants.Parameters.START_TIME)) return false;

         return true;
      }

      public static IEnumerable<IParameter> AllParametersToExport(this ISchemaItem schemaItem)
      {
         return schemaItem.AllVisibleParameters().Where(schemaItem.ParameterShouldBeExported);
      }

      public static bool ParameterShouldBeExported(this SimpleProtocol simpleProtocol, IParameter parameter)
      {
         if (!ParameterShouldBeExported((ISchemaItem) simpleProtocol, parameter)) return false;
         if (parameter.IsNamed(Constants.Parameters.END_TIME) && simpleProtocol.IsSingleDosing) return false;
         return true;
      }
   }
}