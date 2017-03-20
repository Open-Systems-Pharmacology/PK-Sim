using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IBoxWhiskerNumericFieldsView : IView<IBoxWhiskerNumericFieldsPresenter>
   {
      void AddFieldSelectionView(IView view);
      void BindTo(BoxWhiskerNumericFieldDTO dto);
   }
}