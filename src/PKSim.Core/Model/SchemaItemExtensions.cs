namespace PKSim.Core.Model
{
   public static class SchemaItemExtensions
   {
      public static bool DoseIsInMass(this ISchemaItem schemaItem) => doseIsIn(schemaItem, CoreConstants.Units.mg);
      public static bool DoseIsPerBodyWeight(this ISchemaItem schemaItem) => doseIsIn(schemaItem, CoreConstants.Units.MgPerKg);
      public static bool DoseIsPerBodySurfaceArea(this ISchemaItem schemaItem) => doseIsIn(schemaItem, CoreConstants.Units.MgPerM2);

      private static bool doseIsIn(ISchemaItem schemaItem, string unit) => string.Equals(schemaItem.Dose.DisplayUnit.Name, unit);
   }
}