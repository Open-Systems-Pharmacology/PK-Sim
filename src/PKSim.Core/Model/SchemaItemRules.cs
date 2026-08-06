using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;

namespace PKSim.Core.Model
{
   public static class SchemaItemRules
   {
      public static IEnumerable<IBusinessRule> All()
      {
         yield return formulationValid;
         yield return eventKeyValid;
      }

      private static IBusinessRule formulationValid { get; } = CreateRule.For<ISchemaItem>()
         .Property(item => item.FormulationKey)
         .WithRule(formulationIsValid)
         .WithError(formulationErrorMessage);

      private static IBusinessRule eventKeyValid { get; } = CreateRule.For<ISchemaItem>()
         .Property(item => item.EventKey)
         .WithRule(eventKeyIsValid)
         .WithError(PKSimConstants.Error.EventKeyRequired);

      private static bool formulationIsValid(ISchemaItem schemaItem, string formulation)
      {
         if (schemaItem.IsEvent) return true;
         return !schemaItem.ApplicationType.NeedsFormulation || !string.IsNullOrEmpty(formulation);
      }

      private static string formulationErrorMessage(ISchemaItem schemaItem, string formulation)
      {
         return PKSimConstants.Error.FormulationIsRequiredForType(schemaItem.ApplicationType.ToString());
      }

      private static bool eventKeyIsValid(ISchemaItem schemaItem, string eventKey)
      {
         if (!schemaItem.IsEvent) return true;
         return !string.IsNullOrEmpty(eventKey);
      }
   }
}