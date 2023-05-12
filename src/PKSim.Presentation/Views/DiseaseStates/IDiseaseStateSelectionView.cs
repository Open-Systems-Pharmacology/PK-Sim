using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.Presenters.DiseaseStates;

namespace PKSim.Presentation.Views.DiseaseStates
{
   public interface IDiseaseStateSelectionView: IView<IDiseaseStateSelectionPresenter>
   {
      void BindTo(DiseaseStateDTO diseaseStateDTO);
   }
}