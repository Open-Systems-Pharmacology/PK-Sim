using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model.PopulationAnalyses
{
   /// <summary>
   ///    Represents a field that can be used in the analyses. This is required to ensure that the numeric type
   ///    used throughout the analysis are consistent
   /// </summary>
   public abstract class PopulationAnalysisNumericField : PopulationAnalysisDataField<double>, INumericValueField
   {
      private IDimension _dimension;
      public virtual Scalings Scaling { get; set; }

      /// <summary>
      ///    Unit in which the field should be displayed
      /// </summary>
      public Unit DisplayUnit { get; set; }

      protected PopulationAnalysisNumericField()
      {
         Dimension = Constants.Dimension.NO_DIMENSION;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var field = source as PopulationAnalysisNumericField;
         if (field == null) return;
         Dimension = field.Dimension;
         DisplayUnit = field.DisplayUnit;
         Scaling = field.Scaling;
      }

      /// <summary>
      ///    Dimension of the underlying
      /// </summary>
      public IDimension Dimension
      {
         get => _dimension;
         set => _dimension = this.UpdateDimension(value);
      }

      public virtual bool CanBeUsedForGroupingIn(IPopulationDataCollector populationDataCollector)
      {
         var values = GetValues(populationDataCollector);
         if (values.Count == 0)
            return false;

         var min = values.Min();
         var max = values.Max();

         return !ValueComparer.AreValuesEqual(min, max);
      }
   }

   public class NullNumericField : PopulationAnalysisNumericField
   {
      public NullNumericField()
      {
         Name = PKSimConstants.UI.None;
      }

      public override string Id => string.Empty;

      public override IReadOnlyList<double> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return new List<double>();
      }
   }
}