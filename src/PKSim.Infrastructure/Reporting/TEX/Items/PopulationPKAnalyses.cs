using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Reporting.TeX.Items
{
   public class PopulationPKAnalyses
   {
      public IPopulationDataCollector DataCollector { get;  }
      public ChartData<TimeProfileXValue, TimeProfileYValue> ChartData { get; }
      public PopulationAnalysisChart PopulationAnalysisChart { get; }

      public PopulationPKAnalyses(IPopulationDataCollector dataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> chartData, PopulationAnalysisChart populationAnalysisChart)
      {
         DataCollector = dataCollector;
         ChartData = chartData;
         PopulationAnalysisChart = populationAnalysisChart;
      }
   }
}
