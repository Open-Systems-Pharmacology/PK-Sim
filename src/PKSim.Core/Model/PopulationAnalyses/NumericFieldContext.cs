using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model.PopulationAnalyses
{
   /// <summary>
   /// This is required to retrieve Molweight in Merge Dimensions. We avoid that way a back reference from an analysis to its populationDataCollector
   /// </summary>
   public class NumericFieldContext : IWithDimension
   {
      public INumericValueField NumericValueField { get; }
      public IPopulationDataCollector PopulationDataCollector { get; }

      public NumericFieldContext(INumericValueField numericValueField, IPopulationDataCollector populationDataCollector)
      {
         NumericValueField = numericValueField;
         PopulationDataCollector = populationDataCollector;
      }

      public IDimension Dimension
      {
         get => NumericValueField.Dimension;
         set => NumericValueField.Dimension = value;
      }
   }
}