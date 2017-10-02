using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class BinsGroupingDefinition : IntervalGroupingDefinition
   {
      protected abstract int CalculateNumberOfBins(IPopulationDataCollector populationDataCollector);

      [Obsolete("For serialization")]
      protected BinsGroupingDefinition() : this(null)
      {
      }

      protected BinsGroupingDefinition(string fieldName) : base(fieldName)
      {
      }

      public virtual void CreateLimits(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField)
      {
         int numberOfBins = CalculateNumberOfBins(populationDataCollector);

         var sortedValues = numericField.GetValues(populationDataCollector)
            .Where(x => x.IsValid())
            .OrderBy(x => x).ToFloatArray();

         var limits = new List<double>();
         for (var i = 1; i < numberOfBins; i++)
            limits.Add(sortedValues.Quantile((double) i / numberOfBins));

         Limits = limits;
      }
   }

   /// <summary>
   ///    This class separates a list of values into n groups.
   /// </summary>
   /// <remarks>
   ///    Same as Limits with
   ///    Limits = 1*100/Bins Percentile(Field), 2*100/Bins Percentile(Field).. (Bins-1)*100/Bins Percentile(Field)
   /// </remarks>
   public class NumberOfBinsGroupingDefinition : BinsGroupingDefinition
   {
      private readonly IGroupingLabelGenerator _groupingLabelGenerator;
      public int NumberOfBins { get; set; }
      public string NamingPattern { get; set; }
      public Color StartColor { get; set; }
      public Color EndColor { get; set; }
      public LabelGenerationStrategy Strategy { get; set; }

      [Obsolete("For serialization")]
      public NumberOfBinsGroupingDefinition()
      {
         Strategy = LabelGenerationStrategies.Numeric;
         _groupingLabelGenerator = new GroupingLabelGenerator();
      }

      public NumberOfBinsGroupingDefinition(string fieldName) : base(fieldName)
      {
         StartColor = PKSimColors.StartGroupingColor;
         EndColor = PKSimColors.EndGroupingColor;
         NamingPattern = LabelGenerationOptions.DEFAULT_NAMING_PATTERN;
         Strategy = LabelGenerationStrategies.Numeric;
         _groupingLabelGenerator = new GroupingLabelGenerator();
      }

      protected override int CalculateNumberOfBins(IPopulationDataCollector populationDataCollector)
      {
         return NumberOfBins;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var numberOfBinsGroupingDefinition = source as NumberOfBinsGroupingDefinition;
         if (numberOfBinsGroupingDefinition == null) return;
         NumberOfBins = numberOfBinsGroupingDefinition.NumberOfBins;
         NamingPattern = numberOfBinsGroupingDefinition.NamingPattern;
         StartColor = numberOfBinsGroupingDefinition.StartColor;
         EndColor = numberOfBinsGroupingDefinition.EndColor;
         Strategy = numberOfBinsGroupingDefinition.Strategy;
      }

      public override void CreateLimits(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField)
      {
         base.CreateLimits(populationDataCollector, numericField);
         recreateLabels(populationDataCollector, numericField);
      }

      private void recreateLabels(IPopulationDataCollector populationDataCollector, PopulationAnalysisNumericField numericField)
      {
         var labels = _groupingLabelGenerator.GenerateLabels(populationDataCollector, numericField, this);
         GroupingItems.Each((item, i) => item.Label = labels[i]);
      }
   }
}