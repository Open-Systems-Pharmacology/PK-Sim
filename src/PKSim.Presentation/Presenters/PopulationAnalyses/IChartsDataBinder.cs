using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IChartsDataBinder<TX, TY> where TX : IXValue where TY : IYValue
   {
      void Bind(ChartData<TX, TY> chartData, PopulationAnalysisChart analysisChart);
      void UpdateSettings(PopulationAnalysisChart populationAnalysisChart);
      void ClearPlot();
      void UpdateAxesSettings(PopulationAnalysisChart analysisChart);
   }
}