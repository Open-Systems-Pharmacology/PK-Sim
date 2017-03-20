using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldsView : IView<IPopulationAnalysisFieldsPresenter>
   {
      void BindTo(IEnumerable<PopulationAnalysisFieldDTO> parameterFields);
      PopulationAnalysisFieldDTO SelectedField { get; set; }
      bool CreateGroupingButtonEnabled { get; set; }
      bool ColorSelectionVisible { get; set; }
      bool ScalingVisible { get; set; }
      bool CreateGroupingButtonVisible { get; set; }
   }

}