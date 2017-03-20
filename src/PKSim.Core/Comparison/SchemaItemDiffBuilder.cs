using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;

namespace PKSim.Core.Comparison
{
   public abstract class SchemaItemDiffBuilder<TSchemaItem> : DiffBuilder<TSchemaItem> where TSchemaItem : class, ISchemaItem
   {
      private readonly ContainerDiffBuilder _containerDiffBuilder;

      protected SchemaItemDiffBuilder(ContainerDiffBuilder containerDiffBuilder)
      {
         _containerDiffBuilder = containerDiffBuilder;
      }

      public override void Compare(IComparison<TSchemaItem> comparison)
      {
         _containerDiffBuilder.Compare(comparison);
         CompareValues(x => x.ApplicationType, PKSimConstants.UI.ApplicationType, comparison);
         CompareStringValues(x => x.FormulationKey, PKSimConstants.UI.PlaceholderFormulation, comparison);
         CompareStringValues(x => x.TargetOrgan, PKSimConstants.UI.TargetOrgan, comparison);
         CompareStringValues(x => x.TargetCompartment, PKSimConstants.UI.TargetCompartment, comparison);
      }
   }

   public class SchemaItemDiffBuilder : SchemaItemDiffBuilder<SchemaItem>
   {
      public SchemaItemDiffBuilder(ContainerDiffBuilder containerDiffBuilder) : base(containerDiffBuilder)
      {
      }
   }
}