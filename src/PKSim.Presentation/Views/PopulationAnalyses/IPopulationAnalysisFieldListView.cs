using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldListView : IView<IPopulationAnalysisFieldListPresenter>
   {
      void BindTo(IEnumerable<PopulationAnalysisFieldDTO> populationAnalysisFields);
      string Description { get; set; }
      IPopulationAnalysisFieldsDragDropBinder DragDropBinder { get; }
   }
}