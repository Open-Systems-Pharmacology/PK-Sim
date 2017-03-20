using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisAvailablePKParametersView : IView<IPopulationAnalysisAvailablePKParametersPresenter>
   {
      void BindTo(IReadOnlyCollection<QuantityPKParameterDTO> allPKParameters);
      IEnumerable<QuantityPKParameterDTO> SelectedPKParameters { get; }
   }
}