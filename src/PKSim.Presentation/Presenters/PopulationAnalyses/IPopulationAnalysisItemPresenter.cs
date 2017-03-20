using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisPresenter : IPresenter
   {
      void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis);
   }

   public interface IPopulationAnalysisItemPresenter : IPopulationAnalysisPresenter,ISubPresenter
   {
   }
}