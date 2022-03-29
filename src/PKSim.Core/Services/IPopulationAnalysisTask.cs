using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPopulationAnalysisTask
   {
      void ExportToExcel<TXValue, TYValue>(ChartData<TXValue, TYValue> chartsData, string analysisName) where TXValue : IXValue where TYValue : IYValue;
      void SetOriginText(PopulationAnalysisChart populationAnalysisChart, string simulationName);
   }
}