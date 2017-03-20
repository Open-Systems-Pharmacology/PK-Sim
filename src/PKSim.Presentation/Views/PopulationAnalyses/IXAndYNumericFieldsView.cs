using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IXAndYNumericFieldsView : IView<IXAndYNumericFieldsPresenter>
   {
      void BindTo(XandYFieldsSelectionDTO fieldsSelectionDTO);
   }
}