using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IMultipleNumericFieldsView : IView<IMultipleNumericFieldsPresenter>
   {
      void BindTo(IEnumerable<FieldSelectionDTO> fieldSelectionDTOs);
      bool UpEnabled { get; set; }
      bool DownEnabled { get; set; }
      FieldSelectionDTO SelectedItem { get; set; }
   }
}