using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class StatisticalAggregationXmlSerializer<T> : BaseXmlSerializer<T> where T : StatisticalAggregation
   {
      public override void PerformMapping()
      {
         Map(x => x.LineStyle);
         Map(x => x.Selected);
      }
   }
   public class SingleStatisticalAggregationXmlSerializer : StatisticalAggregationXmlSerializer<PredefinedStatisticalAggregation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Method);
      }
   }
   public class PercentileStatisticalAggregationXmlSerializer : StatisticalAggregationXmlSerializer<PercentileStatisticalAggregation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Percentile);
      }
   }
}