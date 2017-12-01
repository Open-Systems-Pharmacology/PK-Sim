using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisOutputField : PopulationAnalysisDataRefField<QuantityValues>, IQuantityField
   {
      private IDimension _dimension;
      public virtual Scalings Scaling { get; set; }
      public virtual string QuantityPath { get; set; }
      public virtual QuantityType QuantityType { get; set; }
      public virtual Color Color { get; set; }

      /// <summary>
      ///    Unit in which the field should be displayed
      /// </summary>
      public virtual Unit DisplayUnit { get; set; }

      public PopulationAnalysisOutputField()
      {
         Dimension = Constants.Dimension.NO_DIMENSION;
      }

      public override string Id => QuantityPath;

      public override IReadOnlyList<QuantityValues> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return populationDataCollector.AllOutputValuesFor(QuantityPath);
      }

      /// <summary>
      ///    Dimension of the underlying
      /// </summary>
      public virtual IDimension Dimension
      {
         get => _dimension;
         set => _dimension = this.UpdateDimension(value);
      }

      public override bool DerivedFieldAllowed => false;

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var outputField = source as PopulationAnalysisOutputField;
         if (outputField == null) return;
         QuantityPath = outputField.QuantityPath;
         QuantityType = outputField.QuantityType;
         Color = outputField.Color;
         Scaling = outputField.Scaling;
         Dimension = outputField.Dimension;
         DisplayUnit = outputField.DisplayUnit;
      }
   }

   public class NullOutputField : PopulationAnalysisOutputField
   {
      public override string Id => string.Empty;

      public override IReadOnlyList<QuantityValues> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return new List<QuantityValues>();
      }
   }
}