using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Populations
{
   public interface IRandomPopulationSettingsView : IView<IRandomPopulationSettingsPresenter>
   {
      void BindTo(PopulationSettingsDTO populationSettingsDTO);
      bool CreatingPopulation { get; set; }
      bool GenderSelectionVisible { get; set; }
      void UpdateLayoutForEditing();
      string Population { get; set; }
   }
}