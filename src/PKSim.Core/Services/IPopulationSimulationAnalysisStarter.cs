using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPopulationSimulationAnalysisStarter
   {
      PopulationAnalysisChart CreateAnalysisForPopulationSimulation(IPopulationDataCollector populationDataCollector, PopulationAnalysisType populationAnalysisType);

      /// <summary>
      ///    Edits the <paramref name="populationAnalysisChartToEdit" /> and returns the edited analysis. This might be another
      ///    reference
      ///    if the user confirmed the edition or the same reference if the edition action was canceled
      /// </summary>
      /// <param name="populationDataCollector">Simulation containing the analysis to edit</param>
      /// <param name="populationAnalysisChartToEdit">Analysis that will be edited</param>
      PopulationAnalysisChart EditAnalysisForPopulationSimulation(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart populationAnalysisChartToEdit);
   }
}