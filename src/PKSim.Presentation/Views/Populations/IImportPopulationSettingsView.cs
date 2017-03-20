
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Populations
{
   public interface IImportPopulationSettingsView : IView<IImportPopulationSettingsPresenter>
   {
      void BindTo(ImportPopulationSettingsDTO importPopulationSettingsDTO);
      bool CreatingPopulation { get; set; }
      void UpdateLayoutForEditing();
   }
}