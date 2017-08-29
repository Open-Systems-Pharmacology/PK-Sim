using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public enum StatisticalAggregationType
   {
      ArithmeticMean,
      ArithmeticStandardDeviation,
      GeometricMean,
      GeometricStandardDeviation,
      Min,
      Max,
      Median,
      Range90,
      Range95
   }

   public abstract class StatisticalAggregation : IUpdatable
   {
      public abstract string Id { get; }
      public bool Selected { get; set; }
      public LineStyles LineStyle { get; set; }

      protected StatisticalAggregation()
      {
         LineStyle = LineStyles.Solid;
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var curveSelection = source as StatisticalAggregation;
         if (curveSelection == null) return;
         LineStyle = curveSelection.LineStyle;
         Selected = curveSelection.Selected;
      }
   }

   /// <summary>
   ///    One of the predefined selected curved such as median, mean etc..
   /// </summary>
   public class PredefinedStatisticalAggregation : StatisticalAggregation
   {
      public StatisticalAggregationType Method { get; set; }

      public override string Id => Method.ToString();

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var singleCurveSelection = source as PredefinedStatisticalAggregation;
         if (singleCurveSelection == null) return;
         Method = singleCurveSelection.Method;
      }
   }

   /// <summary>
   ///    One of the dynamic percentile selection curved
   /// </summary>
   public class PercentileStatisticalAggregation : StatisticalAggregation
   {
      public double Percentile { get; set; }

      public override string Id => $"Percentile_{Percentile}";

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var percentileCurveSelection = source as PercentileStatisticalAggregation;
         if (percentileCurveSelection == null) return;
         Percentile = percentileCurveSelection.Percentile;
      }
   }
}