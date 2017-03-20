using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PopulationAnalysisChartXmlSerializer<T, U> : BaseXmlSerializer<T> where T : PopulationAnalysisChart<U> where U : PopulationAnalysis
   {
      public override void PerformMapping()
      {
         Map(x => x.Name);
         Map(x => x.Id);
         Map(x => x.Description);
         Map(x => x.ChartSettings);
         Map(x => x.PopulationAnalysis);
         Map(x => x.ObservedDataCollection);
         Map(x => x.FontAndSize);
         Map(x => x.OriginText);
         Map(x => x.IncludeOriginData);
         Map(x => x.PrimaryXAxisSettings);
         Map(x => x.PrimaryYAxisSettings);
         MapEnumerable(x => x.SecondaryYAxisSettings, x => x.AddSecondaryAxis);
      }
   }

   public class BoxWhiskerAnalysisChartXmlSerializer : PopulationAnalysisChartXmlSerializer<BoxWhiskerAnalysisChart, PopulationBoxWhiskerAnalysis>
   {
   }

   public class ScatterAnalysisChartXmlSerializer : PopulationAnalysisChartXmlSerializer<ScatterAnalysisChart, PopulationPivotAnalysis>
   {
   }

   public class RangeAnalysisChartXmlSerializer : PopulationAnalysisChartXmlSerializer<RangeAnalysisChart, PopulationPivotAnalysis>
   {
   }

   public class TimeProfileAnalysisChartXmlSerializer : PopulationAnalysisChartXmlSerializer<TimeProfileAnalysisChart, PopulationStatisticalAnalysis>
   {
   }
}