using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisObservedDataSettingsView : IView<IPopulationAnalysisObservedDataSettingsPresenter>
   {
      void BindTo(IEnumerable<ObservedDataCurveOptionsDTO> observedDataCurveOptions);
   }
}